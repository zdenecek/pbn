using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using pbn.dds;
using pbn.model;
using pbn.service;
using pbn.tokens;
using pbn.utils;

namespace pbn.manipulators;

public class PbnBoardAnalyzer
{
    public enum AnalysisType
    {
        /// <summary>
        ///     Analysis made in format of one made with Deep Finesse.
        ///     Contains following tags:
        ///     <list>
        ///         <item> Minimax </item>
        ///         <item> Ability </item>
        ///     </list>
        /// </summary>
        AbilityAnalysis,

        /// <summary>
        ///     Analysis in format of one made with Bridge Composer. Generated file usually is version 2.1.
        ///     Contains following tags:
        ///     <list>
        ///         <item> OptimumResultTable </item>
        ///         <item> OptimumScore </item>
        ///         <item> DoubleDummyTricks </item>
        ///     </list>
        ///     Does not contain information about minimax contract, only score, so it cannot be used to generate
        ///     <see cref="AbilityAnalysis" /> like syntax.
        ///     See bridgecomposer.pbn file for example.
        ///     Note that BridgeComposer generated file is usually bloated.
        /// </summary>
        OptimumResultTableAnalysis
    }

    private static readonly HashSet<string> OptimumResultTableTokenNames =
        new() { "OptimumResultTable", "OptimumScore", "DoubleDummyTricks" };

    private static readonly HashSet<string> AbilityAnalysisTokenNames = new() { "Minimax", "Ability" };

    private readonly IAnalysisService analysisService;
    private readonly TagFactory tagFactory;

    public PbnBoardAnalyzer(DdsAnalysisService analysisService, TagFactory tagFactory)
    {
        this.analysisService = analysisService;
        this.tagFactory = tagFactory;
    }

    private HashSet<string> GetAnalysisTagNames(AnalysisType type)
    {
        return type switch
        {
            AnalysisType.AbilityAnalysis => AbilityAnalysisTokenNames,
            AnalysisType.OptimumResultTableAnalysis => OptimumResultTableTokenNames,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    public void AddAnalyses(PbnFile file)
    {
        PurgeAnalysesTokens(file, AnalysisType.OptimumResultTableAnalysis);

        var toBeAnalyzed = new List<PbnFile.BoardContext>();

        foreach (var board in file.Boards)
        {
            if (HasAnalysis(board, AnalysisType.AbilityAnalysis))
                continue;

            PurgeAnalysesTokens(file, board, AnalysisType.AbilityAnalysis);
            toBeAnalyzed.Add(board);
        }

        AddAnalyses(toBeAnalyzed);
    }

    private void AddAnalyses(List<PbnFile.BoardContext> boardContexts)
    {
        var boards = boardContexts.Select(context => context.AsBoard()).ToList();
        var tables = analysisService.AnalyzeBoards(boards);

        foreach (var (context, analysisTable) in boardContexts.Zip(tables))
        {
            var tokens = CreateAnalysisTokens(analysisTable);
            foreach (var token in tokens)
            {
                context.AppendToken(token);
            }
        }
    }

    private IEnumerable<SemanticPbnToken> CreateAnalysisTokens(AnalysisTable table)
    {
        // [Ability "N:34425 E:A98B7 S:34425 W:A98B7"]
        var tokenString = new StringBuilder();
        foreach (var position in new [] { Position.North , Position.East, Position.South, Position.West})
        {
            if(position != Position.North) tokenString.Append(' ');    
            tokenString.Append(position.ToLetter()).Append(':');
            foreach (var suit in new[] { Suit.Notrump, Suit.Spades, Suit.Hearts, Suit.Diamonds, Suit.Clubs })
            {
                var value = table.GetDoubleDummyTricks(suit, position);
                tokenString.Append(value.ToString("X"));
            }
        }

        var abilityTag = tagFactory.CreateTag("Ability", tokenString.ToString());

        tokenString = new StringBuilder();

        // [Minimax "3NW-430"]

        tokenString.Append(table.MinimaxContract.Level);
        tokenString.Append(table.MinimaxContract.Suit.ToLetter());
        tokenString.Append(table.MinimaxContract.Declarer.ToLetter());
        tokenString.Append(table.MinimaxScore);

        var minimaxTag = tagFactory.CreateTag("Minimax", tokenString.ToString());

        return new [] {abilityTag, minimaxTag};
    }
    

    private void PurgeAnalysesTokens(PbnFile file, AnalysisType type)
    {
        var names = GetAnalysisTagNames(type);
        var toDelete = file.Tokens.GetAllTagsByNames(names);

        foreach (var tag in toDelete) file.DeleteToken(tag);
    }

    private void PurgeAnalysesTokens(PbnFile file, PbnFile.BoardContext context, AnalysisType type)
    {
        var names = GetAnalysisTagNames(type);
        var toDelete = context.Tokens.GetAllTagsByNames(names);

        foreach (var tag in toDelete) file.DeleteToken(tag);
    }


    private bool HasAnalysis(PbnFile.BoardContext context, AnalysisType type)
    {
        var names = GetAnalysisTagNames(type);
        return context.Tokens.GetAllTagsByNames(names).Any();
    }
}