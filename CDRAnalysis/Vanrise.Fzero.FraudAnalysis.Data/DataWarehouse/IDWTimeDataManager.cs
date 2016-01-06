using System.Collections.Generic;
using Vanrise.Fzero.FraudAnalysis.Entities;
using System;

namespace Vanrise.Fzero.FraudAnalysis.Data
{
    public interface IDWTimeDataManager : IDataManager 
    {
        List<DWTime> GetTimes(DateTime from, DateTime to);
    }
}
