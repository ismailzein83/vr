using System;
using System.Collections.Generic;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data
{
    public interface IPredefinedDataManager : IDataManager 
    {
        List<CallClass> GetAllCallClasses();

        List<Period> GetPeriods();
        
    }
}
