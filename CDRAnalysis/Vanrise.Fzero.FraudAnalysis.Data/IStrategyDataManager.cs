using System;
using System.Collections.Generic;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data
{
    public interface IStrategyDataManager : IDataManager 
    {
        Strategy GetStrategy(int strategyId);

        List<Strategy> GetAllStrategies();

        

        List<Strategy> GetFilteredStrategies(int fromRow, int toRow, string name, string description);

        bool AddStrategy(Strategy strategy, out int insertedId);

        bool UpdateStrategy(Strategy strategy);

        List<CallClass> GetAllCallClasses();

        List<Period> GetPeriods();

        

        
    }
}
