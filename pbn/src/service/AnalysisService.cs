using System.Collections.Generic;
using pbn.model;

namespace pbn.service;

public interface IAnalysisService
{
    AnalysisTable AnalyzeBoard(Board board);

    IList<AnalysisTable> AnalyzeBoards(IList<Board> boards);
}