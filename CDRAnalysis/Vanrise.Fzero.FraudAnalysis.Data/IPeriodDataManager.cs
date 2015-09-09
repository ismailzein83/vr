using System.Collections.Generic;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data
{
    public interface IPeriodDataManager : IDataManager 
    {
        List<Period> GetPeriods();
    }
}
