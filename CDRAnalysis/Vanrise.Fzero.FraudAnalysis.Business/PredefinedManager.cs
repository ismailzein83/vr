using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;
using System.Collections.Generic;

namespace Vanrise.Fzero.FraudAnalysis.Business
{
    public class PredefinedManager
    {
        public List<Period> GetPeriods()
        {
            IPredefinedDataManager dataManager = FraudDataManagerFactory.GetDataManager<IPredefinedDataManager>();

            return dataManager.GetPeriods();
        }

    }
}
