﻿using System;
using System.Collections.Generic;
using Vanrise.Data;
using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data
{
    public interface IStrategyDataManager : IDataManager//, IBulkApplyDataManager<StrategyExecutionDetail>
    {
        List<String> GetStrategyNames(List<int> strategyIds);

        Strategy GetStrategy(int strategyId);

        List<Strategy> GetStrategies(int PeriodId, bool? IsEnabled);

        BigResult<Strategy> GetFilteredStrategies(Vanrise.Entities.DataRetrievalInput<StrategyResultQuery> input);

        bool AddStrategy(Strategy strategyObject, out int insertedId, int userId);

        bool UpdateStrategy(Strategy strategy, int userId);

        void DeleteStrategyResults(int StrategyId, DateTime FromDate, DateTime ToDate);
        
    }
}
