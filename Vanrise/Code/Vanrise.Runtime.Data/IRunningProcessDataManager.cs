using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Runtime.Entities;

namespace Vanrise.Runtime.Data
{
    public interface IRunningProcessDataManager : IDataManager
    {
        RunningProcessInfo InsertProcessInfo(string processName, string machineName, RunningProcessAdditionalInfo additionalInfo);       

        bool AreRunningProcessesUpdated(ref object _updateHandle);

        List<RunningProcessInfo> GetRunningProcesses();

        void DeleteRunningProcess(int runningProcessId);

        bool IsExists(int runningProcessId);
    }
}
