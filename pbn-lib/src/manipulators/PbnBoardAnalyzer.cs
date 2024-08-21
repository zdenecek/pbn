using System;
using System.Collections.Generic;
using pbn.dds;
using pbn.tokens;

namespace pbn.manipulators.analysis;

/// <summary>
///     Class for adding double dummy and par score analysis to PBN file.
/// </summary>
public class PbnBoardAnalyzer
{
    private IDictionary<AnalysisSerializationType, IBoardAnalyzer> analyzers;

    public PbnBoardAnalyzer(DdsAnalysisService analysisService, TagFactory tagFactory)
    {
        analyzers = IBoardAnalyzer.GetDefault(analysisService, tagFactory);
    }

    /// <summary>
    ///     Adds analysis to given file. Removes or reuses existing analysis tokens.
    /// </summary>
    public void AddAnalyses(
        PbnFile file,
        AnalysisSerializationType type = AnalysisSerializationType.AbilityAnalysis
    )
    {
        if (!analyzers.ContainsKey(type)) throw new ArgumentException($"Analyzer for type {type} not found");

        foreach (var (analysisType, boardAnalyzer) in analyzers)
        {
            if (type == analysisType) continue;
            boardAnalyzer.PurgeAnalysesTokens(file);
        }

        analyzers[type].AddAnalyses(file);
    }
}