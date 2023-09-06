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

/// <summary>
/// Class for adding double dummy and par score analysis to PBN file.
/// </summary>
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
        this.analysisService = analysisService ?? throw new ArgumentNullException(nameof(analysisService));
        this.tagFactory = tagFactory ?? throw new ArgumentNullException(nameof(tagFactory));
    }

    /// <summary>
    /// Get tag names corresponding to given analysis type.
    /// </summary>
    /// <remarks>
    /// There are more ways to encode analysis into a Pbn file, see <see cref="AnalysisType"/>.
    /// </remarks>
    private HashSet<string> GetAnalysisTagNames(AnalysisType type)
    {
        return type switch
        {
            AnalysisType.AbilityAnalysis => AbilityAnalysisTokenNames,
            AnalysisType.OptimumResultTableAnalysis => OptimumResultTableTokenNames,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    /// <summary>
    /// Adds analysis to given file. Removes or reuses existing analysis tokens.
    /// </summary>
    /// <remarks>
    /// Uses the <see cref="AnalysisType.AbilityAnalysis"/> type of analysis.
    /// </remarks>
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

    /// <summary>
    /// Adds analyses to given board contexts.
    /// </summary>
    /// <remarks>
    /// Uses the <see cref="AnalysisType.AbilityAnalysis"/> type of analysis.
    /// </remarks>
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
    
    /// <summary>
    /// Creates ability tag and minimax tag from a given analysis table.
    /// </summary>
    private IEnumerable<SemanticPbnToken> CreateAnalysisTokens(AnalysisTable table) =>
        new[] { MakeAbilityTag(table), MakeMinimaxTag(table) };

    /// <summary>
    /// Creates a minimax tag from a given analysis table.
    /// </summary>
    private Tag MakeMinimaxTag(AnalysisTable table)
    {
        var tokenString = new StringBuilder();

        tokenString.Append(table.MinimaxContract.Level);
        tokenString.Append(table.MinimaxContract.Suit.ToLetter());
        tokenString.Append(table.MinimaxContract.Declarer.ToLetter());
        tokenString.Append(table.MinimaxScore);

        var minimaxTag = tagFactory.CreateTag("Minimax", tokenString.ToString());
        return minimaxTag;
    }
    
    /// <summary>
    /// Creates ability tag from a given analysis table.
    /// </summary>
    private Tag MakeAbilityTag(AnalysisTable table)
    {
        var tokenString = new StringBuilder();
        foreach (var position in new[] { Position.North, Position.East, Position.South, Position.West })
        {
            if (position != Position.North) tokenString.Append(' ');
            tokenString.Append(position.ToLetter()).Append(':');
            foreach (var suit in new[] { Suit.Notrump, Suit.Spades, Suit.Hearts, Suit.Diamonds, Suit.Clubs })
            {
                var value = table.GetDoubleDummyTricks(suit, position);
                tokenString.Append(value.ToString("X"));
            }
        }

        var abilityTag = tagFactory.CreateTag("Ability", tokenString.ToString());
        return abilityTag;
    }


    /// <summary>
    /// Removes all tokens corresponding to given analysis type from a <see cref="PbnFile"/>.
    /// </summary>
    private void PurgeAnalysesTokens(PbnFile file, AnalysisType type)
    {
        var names = GetAnalysisTagNames(type);
        var toDelete = file.Tokens.GetAllTagsByNames(names);

        foreach (var tag in toDelete) file.DeleteToken(tag);
    }

    /// <summary>
    /// Removes all tokens corresponding to given analysis type from a board context.
    /// </summary>
    private void PurgeAnalysesTokens(PbnFile file, PbnFile.BoardContext context, AnalysisType type)
    {
        var names = GetAnalysisTagNames(type);
        var toDelete = context.Tokens.GetAllTagsByNames(names);

        foreach (var tag in toDelete) file.DeleteToken(tag);
    }

    /// <summary>
    /// Returns true if given board has full analysis of given type.
    /// </summary>
    private bool HasAnalysis(PbnFile.BoardContext context, AnalysisType type)
    {
        var names = GetAnalysisTagNames(type);
        return context.Tokens.GetAllTagsByNames(names).Any();
    }
}