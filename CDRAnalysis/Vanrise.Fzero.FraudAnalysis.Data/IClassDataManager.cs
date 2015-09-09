using System.Collections.Generic;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data
{
    public interface IClassDataManager : IDataManager 
    {
        List<CallClass> GetCallClasses();
    }
}
