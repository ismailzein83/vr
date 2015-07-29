using System;
using System.Collections.Generic;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Business
{
    public class DashboardManager
    {

        public IEnumerable<CasesSummary> GetCasesSummary(DateTime fromDate, DateTime toDate)
        {
            IDashboardManager manager = FraudDataManagerFactory.GetDataManager<IDashboardManager>();
            return manager.GetCasesSummary(fromDate, toDate);
        }


        public IEnumerable<StrategyCases> GetStrategyCases(DateTime fromDate, DateTime toDate)
        {
            IDashboardManager manager = FraudDataManagerFactory.GetDataManager<IDashboardManager>();
            return manager.GetStrategyCases(fromDate, toDate);
        }


        public IEnumerable<BTSCases> GetBTSCases(DateTime fromDate, DateTime toDate)
        {
            IDashboardManager manager = FraudDataManagerFactory.GetDataManager<IDashboardManager>();
            return manager.GetBTSCases(fromDate, toDate);
        }


        public IEnumerable<CellCases> GetCellCases(DateTime fromDate, DateTime toDate)
        {
            IDashboardManager manager = FraudDataManagerFactory.GetDataManager<IDashboardManager>();
            return manager.GetCellCases(fromDate, toDate);
        }


    }
}
