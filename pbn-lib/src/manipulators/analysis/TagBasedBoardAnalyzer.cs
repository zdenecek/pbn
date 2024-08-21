using System;
using System.Collections.Generic;
using System.Linq;
using pbn.dds;
using pbn.model;
using pbn.tokens;
using pbn.utils;

namespace pbn.manipulators.analysis;

public abstract class TagBasedBoardAnalyzer : BaseBoardAnalyzer
{
    protected readonly TagFactory TagFactory;
    
    protected abstract IEnumerable<string> AnalysisTagNames { get; }

    protected TagBasedBoardAnalyzer(DdsAnalysisService analysisService, TagFactory tagFactory) : base(analysisService)
    {
        TagFactory = tagFactory ?? throw new ArgumentNullException(nameof(tagFactory));
    }
    

    public override void PurgeAnalysesTokens(PbnFile file)
    {
        var names = AnalysisTagNames;
        var toDelete = file.Tokens.GetAllTagsByNames(names).ToList();

        foreach (var tag in toDelete) file.DeleteToken(tag);
    }
    
    public override void PurgeAnalysesTokens(PbnFile file, PbnFile.BoardContext context)
    {
        var names = AnalysisTagNames;
        var toDelete = context.Tokens.GetAllTagsByNames(names).ToList();

        foreach (var tag in toDelete) file.DeleteToken(tag);
    }

    public override bool HasAnalysis(PbnFile.BoardContext context)
    {
        var names = AnalysisTagNames;
        return context.Tokens.GetAllTagsByNames(names).Any();
    }
}