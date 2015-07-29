using System;
using System.Collections.Generic;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data
{
    public interface IDashboardManager : IDataManager 
    {
        List<CasesSummary> GetCasesSummary(DateTime fromDate, DateTime toDate);
        List<StrategyCases> GetStrategyCases(DateTime fromDate, DateTime toDate);
        List<BTSCases> GetBTSCases(DateTime fromDate, DateTime toDate);
        List<CellCases> GetCellCases(DateTime fromDate, DateTime toDate);
    }
}
