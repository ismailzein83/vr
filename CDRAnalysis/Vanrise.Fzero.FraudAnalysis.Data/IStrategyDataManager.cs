using System;
using System.Collections.Generic;
using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data
{
    public interface IStrategyDataManager : IDataManager 
    {
        List<String> GetStrategyNames(List<int> strategyIds);

        Strategy GetStrategy(int strategyId);

        List<Strategy> GetStrategies(int PeriodId);

        BigResult<Strategy> GetFilteredStrategies(Vanrise.Entities.DataRetrievalInput<StrategyResultQuery> input);

        bool AddStrategy(Strategy strategyObject, out int insertedId, int userId);

        bool UpdateStrategy(Strategy strategy, int userId);
        
    }
}
