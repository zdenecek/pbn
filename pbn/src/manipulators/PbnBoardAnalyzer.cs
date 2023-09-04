using System;
using System.Collections.Generic;
using System.Linq;
using pbn.model;
using pbn.service;
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
        ///     <see cref="DeepFinesseAnalysis" /> like syntax.
        ///     See bridgecomposer.pbn file for example.
        ///     Note that BridgeComposer generated file is usually bloated.
        /// </summary>
        OptimumResultTableAnalysis
    }

    private static readonly HashSet<string> OptimumResultTableTokenNames =
        new() { "OptimumResultTable", "OptimumScore", "DoubleDummyTricks" };

    private static readonly HashSet<string> AbilityAnalysisTokenNames = new() { "Minimax", "Ability" };

    private readonly IAnalysisService analysisService;

    public PbnBoardAnalyzer(DdsAnalysisService analysisService)
    {
        this.analysisService = analysisService;
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

        AddAnalyses(file, toBeAnalyzed);
    }

    private void AddAnalyses(PbnFile file, List<PbnFile.BoardContext> boardContexts)
    {
        var boards = boardContexts.Cast<Board>();
        var tables = analysisService.AnalyzeBoards(boards);

        foreach (var (context, analysisTable) in boardContexts.Zip(tables))
        {
        }
    }

    public void PurgeAnalysesTokens(PbnFile file, AnalysisType type)
    {
        var names = GetAnalysisTagNames(type);
        var toDelete = file.Tokens.GetAllTagsByNames(names);

        foreach (var tag in toDelete) file.DeleteToken(tag);
    }

    public void PurgeAnalysesTokens(PbnFile file, PbnFile.BoardContext context, AnalysisType type)
    {
        var names = GetAnalysisTagNames(type);
        var toDelete = context.Tokens.GetAllTagsByNames(names);

        foreach (var tag in toDelete) file.DeleteToken(tag);
    }


    public bool HasAnalysis(PbnFile.BoardContext context, AnalysisType type)
    {
        var names = GetAnalysisTagNames(type);
        return context.Tokens.GetAllTagsByNames(names).Any();
    }
}