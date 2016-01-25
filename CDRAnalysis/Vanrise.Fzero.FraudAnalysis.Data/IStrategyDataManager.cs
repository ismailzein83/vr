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

        List<Strategy> GetStrategies();

        bool AddStrategy(Strategy strategyObject, out int insertedId);

        bool UpdateStrategy(Strategy strategy);

        bool AreStrategiesUpdated(ref object updateHandle);

    }
}
