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

        public Vanrise.Entities.BigResult<Strategy> GetFilteredStrategies(Vanrise.Entities.DataRetrievalInput<StrategyResultQuery> input)
        {
            throw new NotImplementedException();
        }

        public bool UpdateStrategy(Strategy strategy, int userId)
        {
            throw new NotImplementedException();
        }

        public List<String> GetStrategyNames(List<int> strategyIds)
        {
            throw new NotImplementedException();
        }


        public List<Strategy> GetStrategies(int PeriodId)
        {
            throw new NotImplementedException();
        }


        public bool AddStrategy(Strategy strategyObject, out int insertedId, int userId)
        {
            throw new NotImplementedException();
        }


        public Strategy GetStrategy(int strategyId)
        {
            throw new NotImplementedException();
        }

        public List<Strategy> GetStrategies(int PeriodId, bool? IsEnabled)
        {
            throw new NotImplementedException();
        }


        public void DeleteStrategyResults(int StrategyId, DateTime FromDate, DateTime ToDate)
        {
            throw new NotImplementedException();
        }
    }
}
