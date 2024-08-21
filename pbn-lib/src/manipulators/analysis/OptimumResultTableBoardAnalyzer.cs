using System.Collections.Generic;
using pbn.dds;
using pbn.model;
using pbn.tokens;

namespace pbn.manipulators.analysis;

public class OptimumResultTableBoardAnalyzer : TagBasedBoardAnalyzer
{
    public OptimumResultTableBoardAnalyzer(DdsAnalysisService analysisService, TagFactory tagFactory) : base(analysisService, tagFactory)
    {
    }

    public override IEnumerable<SemanticPbnToken> CreateAnalysisTokens(AnalysisTable table)
    {
        throw new System.NotImplementedException();
    }
    
    private static readonly HashSet<string> OptimumResultTableTokenNames =
        new() { "OptimumResultTable", "OptimumScore", "DoubleDummyTricks" };

    protected override IEnumerable<string> AnalysisTagNames => OptimumResultTableTokenNames;
}