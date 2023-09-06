using System.Collections.Generic;
using pbn.model;

namespace pbn.service;

/// <summary>
///     Service for adding double dummy and par contract analysis to a boards.
/// </summary>
public interface IAnalysisService
{
    /// <returns>
    ///     Analysis of a single board.
    /// </returns>
    AnalysisTable AnalyzeBoard(Board board);

    /// <returns>
    ///     Returns analysis of a list of boards.
    /// </returns>
    IList<AnalysisTable> AnalyzeBoards(IList<Board> boards);
}