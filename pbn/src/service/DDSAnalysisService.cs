using pbn.dds;
using pbn.model;

namespace pbn.service;

public class DdsAnalysisService : IAnalysisService
{
    public AnalysisTable AnalyzePbn(string pbnDealString, Vulnerability vulnerability)
    {
        var deals = new DdsTypes.ddTableDealsPBN(new[] { pbnDealString });

        var parResults = new DdsTypes.allParResults();

        var tables = new DdsTypes.ddTablesRes();

        var mode = DdsTypes.VulnerabilityToDdsMode(vulnerability);

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
}