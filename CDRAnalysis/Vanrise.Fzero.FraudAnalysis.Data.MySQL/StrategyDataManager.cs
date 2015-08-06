using System;
using System.Collections.Generic;
using Vanrise.Data.MySQL;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data.MySQL
{
    public class StrategyDataManager : BaseMySQLDataManager, IStrategyDataManager 
    {
        public StrategyDataManager()
            : base("CDRDBConnectionStringMySQL")
        {

        }

        public Strategy GetStrategy(int strategyId)
        {
            throw new NotImplementedException();
        }

        public Vanrise.Entities.BigResult<Strategy> GetFilteredStrategies(Vanrise.Entities.DataRetrievalInput<StrategyResultQuery> input)
        {
            throw new NotImplementedException();
        }

        public bool AddStrategy(Strategy strategy, out int insertedId)
        {
            throw new NotImplementedException();
        }

        public bool UpdateStrategy(Strategy strategy)
        {
            throw new NotImplementedException();
        }

        public List<String> GetStrategyNames(List<int> strategyIds)
        {
            throw new NotImplementedException();
        }


        public List<Strategy> GetAllStrategies(int PeriodId)
        {
            throw new NotImplementedException();
        }
    }
}
