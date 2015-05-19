using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Business
{
    public class StrategyManager
    {
        public Strategy GetStrategy(int StrategyId)
        {

            IStrategyDataManager dataManager = FraudDataManagerFactory.GetDataManager<IStrategyDataManager>();

            return dataManager.GetStrategy(StrategyId);

        }
    }
}
