using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;
using System.Collections.Generic;

namespace Vanrise.Fzero.FraudAnalysis.Business
{
    public class StrategyManager
    {
        public Strategy GetStrategy(int StrategyId)
        {

            IStrategyDataManager dataManager = FraudDataManagerFactory.GetDataManager<IStrategyDataManager>();

            return dataManager.GetStrategy(StrategyId);

        }


        public IEnumerable<Strategy> GetFilteredStrategies(int fromRow, int toRow, string name, string description)
        {

            IStrategyDataManager manager = FraudDataManagerFactory.GetDataManager<IStrategyDataManager>();

            return ((IEnumerable<Strategy>)(manager.GetFilteredStrategies(fromRow, toRow, name, description)));

        }


        public void AddStrategy(Strategy strategy)
        {
            IStrategyDataManager manager = FraudDataManagerFactory.GetDataManager<IStrategyDataManager>();

            manager.AddStrategy(strategy);
        }



        public void UpdateStrategy(Strategy strategy)
        {
            IStrategyDataManager manager = FraudDataManagerFactory.GetDataManager<IStrategyDataManager>();

            manager.UpdateStrategy(strategy);
        }








    }
}
