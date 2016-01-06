using System.Collections.Generic;
using Vanrise.Fzero.FraudAnalysis.Entities;
using System;

namespace Vanrise.Fzero.FraudAnalysis.Data
{
    public interface IDWStrategyDataManager : IDataManager 
    {
        List<DWStrategy> GetStrategies();
    }
}
