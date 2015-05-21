﻿using System.Collections.Generic;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data
{
    public interface IStrategyDataManager : IDataManager 
    {
        Strategy GetStrategy(int strategyId);

        List<Strategy> GetAllStrategies();
    }
}
