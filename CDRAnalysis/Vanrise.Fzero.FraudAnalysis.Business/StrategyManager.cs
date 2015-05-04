using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Fzero.FraudAnalysis.Data;

namespace Vanrise.Fzero.FraudAnalysis.Business
{
    public class StrategyManager
    {
        public Strategy GetDefaultStrategy()
        {

            IStrategyDataManager dataManager = FraudDataManagerFactory.GetDataManager<IStrategyDataManager>();

            return dataManager.GetDefaultStrategy();

        }
    }
}
