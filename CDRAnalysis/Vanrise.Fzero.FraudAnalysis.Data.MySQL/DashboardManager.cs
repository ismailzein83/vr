using System;
using Vanrise.Data.MySQL;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data.SQL
{
    public class DashboardManager : BaseMySQLDataManager, IDashboardManager
    {

        public DashboardManager()
            : base("CDRDBConnectionStringMySQL")
        {

        }




        public System.Collections.Generic.List<CasesSummary> GetCasesSummary(DateTime fromDate, DateTime toDate)
        {
            throw new NotImplementedException();
        }

        public System.Collections.Generic.List<StrategyCases> GetStrategyCases(DateTime fromDate, DateTime toDate)
        {
            throw new NotImplementedException();
        }

        public System.Collections.Generic.List<BTSCases> GetBTSCases(DateTime fromDate, DateTime toDate)
        {
            throw new NotImplementedException();
        }

        public System.Collections.Generic.List<CellCases> GetCellCases(DateTime fromDate, DateTime toDate)
        {
            throw new NotImplementedException();
        }
    }
}
