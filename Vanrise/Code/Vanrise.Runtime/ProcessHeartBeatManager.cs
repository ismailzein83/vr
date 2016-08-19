using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Runtime.Data;

namespace Vanrise.Runtime
{
    internal class ProcessHeartBeatManager
    {
        #region Singleton

        internal static ProcessHeartBeatManager s_current;

        internal static ProcessHeartBeatManager Current
        {
            get
            {
                if (s_current == null)
                    throw new NullReferenceException("s_current");
                return s_current;
            }
        }

        internal ProcessHeartBeatManager(List<int> runningProcessIds)
        {
            _processesHBInfo = runningProcessIds.ToDictionary(processId => processId, processId => new ProcessHBInfo { RunningProcessId = processId });
        }

        #endregion

        Dictionary<int, ProcessHBInfo> _processesHBInfo;

        internal bool UpdateProcessHB(int runningProcessId)
        {
            ProcessHBInfo runningProcess;
            if (!_processesHBInfo.TryGetValue(runningProcessId, out runningProcess))
            {
                if (IsRunningProcessAvailable(runningProcessId))
                {
                    lock (_processesHBInfo)
                    {
                        if (!_processesHBInfo.TryGetValue(runningProcessId, out runningProcess))
                        {
                            runningProcess = new ProcessHBInfo() { RunningProcessId = runningProcessId };
                            _processesHBInfo.Add(runningProcessId, runningProcess);
                        }
                    }
                }
                else
                    return false;
            }

            lock (runningProcess)
            {
                runningProcess.LastHeartBeatTime = DateTime.Now;
            }
            return true;
        }

        internal Dictionary<int, ProcessHBInfo> GetProcessesHBInfo()
        {
            Dictionary<int, ProcessHBInfo> copy;
            lock(_processesHBInfo)
            {
                copy = _processesHBInfo.ToDictionary(itm => itm.Key, itm => itm.Value);
            }
            return copy;
        }

        internal void SetProcessNotAvailable(int runningProcessId)
        {
            lock (_processesHBInfo)
            {
                if (_processesHBInfo.ContainsKey(runningProcessId))
                    _processesHBInfo.Remove(runningProcessId);
            }
        }

        private bool IsRunningProcessAvailable(int runningProcessId)
        {
            lock (_processesHBInfo)
            {
                if (_processesHBInfo.Count > 0 && runningProcessId < _processesHBInfo.Keys.Max())//old process no more available
                    return false;
            }
            IRunningProcessDataManager dataManager = RuntimeDataManagerFactory.GetDataManager<IRunningProcessDataManager>();
            return dataManager.IsExists(runningProcessId);
        }
    }

    internal class ProcessHBInfo
    {
        public int RunningProcessId { get; set; }

        public DateTime LastHeartBeatTime { get; set; }
    }
}