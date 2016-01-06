using System;
using System.Collections.Generic;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Business
{
    public class DWStrategyManager
    {
        public List<DWStrategy> GetStrategies()
        {
            IDWStrategyDataManager dataManager = FraudDataManagerFactory.GetDataManager<IDWStrategyDataManager>();

            return dataManager.GetStrategies();
        }
    }
}
