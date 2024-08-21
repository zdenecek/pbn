using System;
using System.Collections.Generic;
using System.Linq;

using pbn.dds;
using pbn.model;
using pbn.service;
using pbn.tokens;


namespace pbn.manipulators.analysis;

public abstract class BaseBoardAnalyzer : IBoardAnalyzer
{
    protected readonly IAnalysisService AnalysisService;

    protected BaseBoardAnalyzer(DdsAnalysisService analysisService)
    {
        AnalysisService = analysisService ?? throw new ArgumentNullException(nameof(analysisService));
    }
    
    public abstract IEnumerable<SemanticPbnToken> CreateAnalysisTokens(AnalysisTable table);
    
    /// <summary>
    ///     Removes all tokens corresponding to given analysis type from a <see cref="PbnFile" />.
    /// </summary>
    public abstract void PurgeAnalysesTokens(PbnFile file);
    
    /// <summary>
    ///     Removes all tokens corresponding to given analysis type from a <see cref="PbnFile.BoardContext" />.
    /// </summary>
    public abstract void PurgeAnalysesTokens(PbnFile file, PbnFile.BoardContext context);

    public void AddAnalyses(PbnFile file)
    {
        var toBeAnalyzed = new List<PbnFile.BoardContext>();

        foreach (var board in file.Boards)
        {
            if (HasAnalysis(board))
                continue;

            PurgeAnalysesTokens(file, board);
            toBeAnalyzed.Add(board);
        }

        AddAnalyses(toBeAnalyzed);
    }

    public abstract bool HasAnalysis(PbnFile.BoardContext board);

    public void AddAnalyses(List<PbnFile.BoardContext> boardContexts)
    {
        var boards = boardContexts.Select(context => context.AsBoard()).ToList();
        var tables = AnalysisService.AnalyzeBoards(boards);

        foreach (var (context, analysisTable) in boardContexts.Zip(tables))
        {
            var tokens = CreateAnalysisTokens(analysisTable);
            foreach (var token in tokens) context.AppendToken(token);
        }
    }
}