using System.Collections.Generic;
using pbn.model;

namespace pbn.service;

public interface IAnalysisService
{
    AnalysisTable AnalyzeBoard(Board board);

    AnalysisTable[] AnalyzeBoards(IEnumerable<Board> boards);
}