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

        public System.Collections.Generic.List<StrategyCases> GetStrategyCases(DateTime fromDate, DateTime toDate)
        {
            throw new NotImplementedException();
        }

        public Vanrise.Entities.BigResult<CasesSummary> GetCasesSummary(Vanrise.Entities.DataRetrievalInput<DashboardResultQuery> input)
        {
            throw new NotImplementedException();
        }

        public Vanrise.Entities.BigResult<BTSCases> GetBTSCases(Vanrise.Entities.DataRetrievalInput<DashboardResultQuery> input)
        {
            throw new NotImplementedException();
        }

        public Vanrise.Entities.BigResult<DailyVolumeLoose> GetDailyVolumeLooses(Vanrise.Entities.DataRetrievalInput<DashboardResultQuery> input)
        {
            throw new NotImplementedException();
        }


        public Vanrise.Entities.BigResult<BTSHighValueCases> GetTop10BTSHighValue(Vanrise.Entities.DataRetrievalInput<DashboardResultQuery> input)
        {
            throw new NotImplementedException();
        }
    }
}
