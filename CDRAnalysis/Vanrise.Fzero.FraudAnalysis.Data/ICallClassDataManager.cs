using System.Collections.Generic;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data
{
    public interface ICallClassDataManager : IDataManager 
    {
        List<CallClass> GetCallClasses();

        bool AreCallClassesUpdated(ref object updateHandle);
    }
}
