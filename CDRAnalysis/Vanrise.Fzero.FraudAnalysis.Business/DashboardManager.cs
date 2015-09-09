using System;
using System.Collections.Generic;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Business
{
    public class DashboardManager
    {
        public IEnumerable<StrategyCases> GetStrategyCases(DateTime fromDate, DateTime toDate)
        {
            IDashboardManager manager = FraudDataManagerFactory.GetDataManager<IDashboardManager>();
            return manager.GetStrategyCases(fromDate, toDate);
        }

       

        public Vanrise.Entities.IDataRetrievalResult<BTSCases> GetBTSCases(Vanrise.Entities.DataRetrievalInput<DashboardResultQuery> input)
        {
            IDashboardManager manager = FraudDataManagerFactory.GetDataManager<IDashboardManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, manager.GetBTSCases(input));
        }

        public Vanrise.Entities.IDataRetrievalResult<BTSHighValueCases> GetTop10BTSHighValue(Vanrise.Entities.DataRetrievalInput<DashboardResultQuery> input)
        {
            IDashboardManager manager = FraudDataManagerFactory.GetDataManager<IDashboardManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, manager.GetTop10BTSHighValue(input));
        }

       
    }
}
