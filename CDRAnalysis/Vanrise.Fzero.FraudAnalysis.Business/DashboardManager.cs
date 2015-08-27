using System;
using System.Collections.Generic;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Business
{
    public class DashboardManager
    {


        public IEnumerable<StrategyCases> GetStrategyCases(DateTime fromDate, DateTime toDate)
        {
            IDashboardManager manager = FraudDataManagerFactory.GetDataManager<IDashboardManager>();
            return manager.GetStrategyCases(fromDate, toDate);
        }


        public Vanrise.Entities.IDataRetrievalResult<CasesSummary> GetCasesSummary(Vanrise.Entities.DataRetrievalInput<DashboardResultQuery> input)
        {
            IDashboardManager manager = FraudDataManagerFactory.GetDataManager<IDashboardManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, manager.GetCasesSummary(input));
        }

        public Vanrise.Entities.IDataRetrievalResult<BTSCases> GetBTSCases(Vanrise.Entities.DataRetrievalInput<DashboardResultQuery> input)
        {
            IDashboardManager manager = FraudDataManagerFactory.GetDataManager<IDashboardManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, manager.GetBTSCases(input));
        }

        public Vanrise.Entities.IDataRetrievalResult<BTSHighValueCases> GetBTSHighValueCases(Vanrise.Entities.DataRetrievalInput<DashboardResultQuery> input)
        {
            IDashboardManager manager = FraudDataManagerFactory.GetDataManager<IDashboardManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, manager.GetBTSHighValueCases(input));
        }


        


        public Vanrise.Entities.IDataRetrievalResult<DailyVolumeLoose> GetDailyVolumeLooses(Vanrise.Entities.DataRetrievalInput<DashboardResultQuery> input)
        {
            IDashboardManager manager = FraudDataManagerFactory.GetDataManager<IDashboardManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, manager.GetDailyVolumeLooses(input));
        }




        


    }
}
