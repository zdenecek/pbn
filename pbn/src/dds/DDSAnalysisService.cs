using System.Collections.Generic;
using System.Linq;
using pbn.model;
using pbn.service;

namespace pbn.dds;

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


        return BuildAnalysisTable(tables, parResults, 0, board.Dealer);
    }

    public IList<AnalysisTable> AnalyzeBoards(IList<Board> boards)
    {
        // DDS does not support vulnerability parametrization for each board separately.
        // We run analysis for each vulnerability separately.

        // return boards.Select(
        //     this.AnalyzeBoard).ToList();

        var byVul = boards.Select((board, index) => new
            {
                Board = board,
                Index = index,
            })
            .GroupBy(x => x.Board.Vulnerability);

        var analyses = new AnalysisTable[boards.Count];

        foreach (var grouping in byVul)
        {
            var res = this.AnalyzeBoardsGivenVulnerability(grouping.Select(b => b.Board).ToList(), grouping.Key);
            for (int i = 0; i < res.Length; i++)
            {
                analyses[grouping.ElementAt(i).Index] = res[i];
            }
        }

        return analyses;
    }

    /// <summary>
    /// DDS does not support vulnerability parametrization for each board separately.
    /// </summary>
    private AnalysisTable[] AnalyzeBoardsGivenVulnerability(IList<Board> boards, Vulnerability vulnerability)
    {
        var cardStrings = boards.Select(b => b.CardString).ToArray();

        var deals = new DdsTypes.ddTableDealsPBN(cardStrings);

        var parResults = new DdsTypes.allParResults();

        var tables = new DdsTypes.ddTablesRes();

        var mode = DdsTypes.VulnerabilityToDdsMode(vulnerability);

        var trumpFilter = new int[DdsTypes.DDS_STRAINS];


        DdsInterop.CalcAllTablesPBN(ref deals, mode, trumpFilter, ref tables, ref parResults);

        var res = new AnalysisTable[boards.Count];


        for (int i = 0; i < boards.Count; i++)
        {
            res[i] = BuildAnalysisTable(tables, parResults, i, boards[i].Dealer);
        }

        return res;
    }

    private static AnalysisTable BuildAnalysisTable(DdsTypes.ddTablesRes tables, DdsTypes.allParResults parResults,  int i, Position dealer)
    {
        return AnalysisTable.BuildAnalysisTable(
            (pos, suit) =>
                tables.results[i]
                    .resTable[
                        DdsTypes.DDS_HANDS * DdsTypes.SuitToDdsStrain(suit) + DdsTypes.PositionToDdsPos(pos)
                    ],
            parResults.presults[i].GetParContract(dealer),
            parResults.presults[i].GetParScore(dealer)
        );
    }
}