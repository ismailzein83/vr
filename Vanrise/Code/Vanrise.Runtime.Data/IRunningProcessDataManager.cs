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
        RunningProcessInfo InsertProcessInfo(string processName, string machineName);       

        bool UpdateHeartBeat(int processId, out DateTime heartBeatTime);

        void DeleteTimedOutProcesses(TimeSpan heartBeatReceivedBefore);

        List<RunningProcessInfo> GetRunningProcesses(TimeSpan? heartBeatReceivedWithin);        
    }
}
