﻿using System;
using System.Collections.Generic;
using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data
{
    public interface IStrategyDataManager : IDataManager 
    {
        List<String> GetStrategyNames(List<int> strategyIds);

        Strategy GetStrategy(int strategyId);

        List<Strategy> GetAllStrategies(int? PeriodId);

        BigResult<Strategy> GetFilteredStrategies(Vanrise.Entities.DataRetrievalInput<StrategyResultQuery> input);

        bool AddStrategy(Strategy strategy, out int insertedId);

        bool UpdateStrategy(Strategy strategy);
        
    }
}
