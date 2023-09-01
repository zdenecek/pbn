using pbn.model;

namespace pbn.service;

public interface IAnalysisService
{
    AnalysisTable AnalyzePbn(string pbnDealStrings, Vulnerability vulnerability);
}