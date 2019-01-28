using System.Collections.Generic;
using Vanrise.Entities;

namespace Vanrise.Common.Data
{
    public interface IProcessStateDataManager : IDataManager
    {
        List<ProcessState> GetProcessStates();

        bool InsertOrUpdate(string processStateUniqueName, ProcessStateSettings settings);
    }
}