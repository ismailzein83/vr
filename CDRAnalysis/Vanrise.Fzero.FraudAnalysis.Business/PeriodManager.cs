using System.Collections.Generic;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Business
{
    public class PeriodManager
    {

        public List<Period> GetPeriods()
        {
            IPeriodDataManager dataManager = FraudDataManagerFactory.GetDataManager<IPeriodDataManager>();

            return dataManager.GetPeriods();
        }

    }
}
