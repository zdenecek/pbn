using System.Collections.Generic;
using pbn.dds;
using pbn.model;

namespace pbn.service;

public class DdsAnalysisService : IAnalysisService
{

    public AnalysisTable AnalyzeBoard(Board board)
    {
        var deals = new DdsTypes.ddTableDealsPBN(new[] { board.CardString });

        var parResults = new DdsTypes.allParResults();

        var tables = new DdsTypes.ddTablesRes();

        var mode = DdsTypes.VulnerabilityToDdsMode(board.Vulnerability);

        var trumpFilter = new int[DdsTypes.DDS_STRAINS];


        DdsInterop.CalcAllTablesPBN(ref deals, mode, trumpFilter, ref tables, ref parResults);


        return AnalysisTable.BuildAnalysisTable(
            (pos, suit) =>
                tables.results[0]
                    .resTable[
                        DdsTypes.DDS_HANDS * DdsTypes.SuitToDdsStrain(suit) + DdsTypes.PositionToDdsPos(pos)
                    ]
        );
    }

    public AnalysisTable[] AnalyzeBoards(IEnumerable<Board> boards)
    {
        throw new System.NotImplementedException();
    }
}