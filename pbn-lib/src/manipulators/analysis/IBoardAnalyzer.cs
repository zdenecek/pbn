using System.Collections.Generic;
using pbn.dds;
using pbn.model;
using pbn.tokens;

namespace pbn.manipulators.analysis;

public interface IBoardAnalyzer
{
    static IDictionary<AnalysisSerializationType, IBoardAnalyzer> GetDefault(
        DdsAnalysisService analysisService,
        TagFactory tagFactory
        )
    {
        return new Dictionary<AnalysisSerializationType, IBoardAnalyzer>
        {
            { AnalysisSerializationType.AbilityAnalysis, new AbilityBoardAnalyzer(analysisService, tagFactory) },
            { AnalysisSerializationType.OptimumResultTableAnalysis, new OptimumResultTableBoardAnalyzer(analysisService, tagFactory) },
            { AnalysisSerializationType.PsBridgeAnalysis, new PsBridgeBoardAnalyzer(analysisService) }
        };
    }


    /// <summary>
    ///     Adds analysis to given file. Reuses existing analysis tokens.
    /// </summary>
    void AddAnalyses(PbnFile file);

    /// <summary>
    ///     Adds analyses to given board contexts.
    /// </summary>
    /// <remarks>
    ///     See the <see cref="AnalysisSerializationType.AbilityAnalysis" /> for the type of analysis.
    /// </remarks>
    void AddAnalyses(List<PbnFile.BoardContext> boardContexts);

    /// <summary>
    ///     Serialize analysis into adjacent tokens from a given analysis table.
    /// </summary>
    IEnumerable<SemanticPbnToken> CreateAnalysisTokens(AnalysisTable table);

    /// <summary>
    ///     Removes all tokens corresponding from a <see cref="PbnFile" />.
    /// </summary>
    void PurgeAnalysesTokens(PbnFile file);

    /// <summary>
    ///     Removes all tokens corresponding to given analysis type from a board context.
    /// </summary>
    void PurgeAnalysesTokens(PbnFile file, PbnFile.BoardContext context);

    /// <summary>
    ///     Returns true if given board has full analysis of the given type.
    /// </summary>
    bool HasAnalysis(PbnFile.BoardContext context);
}