using System.Collections.Generic;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data
{
    public interface IPredefinedDataManager : IDataManager 
    {
        List<CallClass> GetCallClasses();

        List<Period> GetPeriods();
        
    }
}
