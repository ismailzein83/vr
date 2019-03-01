using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Vanrise.Common;
using Vanrise.Common.Data;
using Vanrise.Entities;
using Vanrise.Runtime.Data;
using Vanrise.Runtime.Entities;

namespace Vanrise.Runtime
{
    public class RunningProcessManager : IRunningProcessManager
    {
        static IRunningProcessDataManager s_dataManager = RuntimeDataManagerFactory.GetDataManager<IRunningProcessDataManager>();
        static RunningProcessInfo s_currentProcess;
        
        public static RunningProcessInfo CurrentProcess
        {
            get
            {
                return s_currentProcess;
            }
            internal set
            {
                if (s_currentProcess != null)
                    throw new Exception("s_currentProcess already has value");
                s_currentProcess = value;
            }
        }

        public static bool IsCurrentProcessARuntime
        {
            get
            {
                return s_currentProcess != null;
            }
        }
        
        public Dictionary<int, RunningProcessInfo> GetAllRunningProcesses()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("RunningProcessManager_GetAllRunningProcesses",
               () =>
               {
                   return GetRunningProcesses().ToDictionary(p => p.ProcessId, p => p);
               });
        }


        public IDataRetrievalResult<RunningProcessDetails> GetFilteredRunningProcesses(DataRetrievalInput<RunningProcessQuery> input)
        {
            IEnumerable<RunningProcessDetails> runningProcesses = s_dataManager.GetFilteredRunningProcesses(input);
            runningProcesses.ToDictionary(runningProcess => runningProcess.ProcessId, runningProcess => runningProcess);
            return DataRetrievalManager.Instance.ProcessResult(input, runningProcesses.ToBigResult(input, null, RunningProcessDetailMapper));
        }

        
        public List<RunningProcessInfo> GetRunningProcesses()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("RunningProcessManager_GetRunningProcesses",
                () =>
                {
                    return s_dataManager.GetRunningProcesses();
                });
        }
        
        static Dictionary<int, string> s_processIdServiceUrls = new Dictionary<int, string>();
        
        internal string GetProcessTCPServiceURL(int processId)
        {
            string serviceURL;
            if(!s_processIdServiceUrls.TryGetValue(processId, out serviceURL))
            {
                lock(s_processIdServiceUrls)
                {
                    if (!s_processIdServiceUrls.TryGetValue(processId, out serviceURL))
                    {
                        RunningProcessInfo runningProcessInfo = s_dataManager.GetRunningProcess(processId);
                        if (runningProcessInfo == null)
                            throw new NullReferenceException(String.Format("runningProcessInfo '{0}'", processId));
                        if (runningProcessInfo.AdditionalInfo == null)
                            throw new NullReferenceException(String.Format("runningProcessInfo.AdditionalInfo '{0}'", processId));
                        if (runningProcessInfo.AdditionalInfo.TCPServiceURL == null)
                            throw new NullReferenceException(String.Format("runningProcessInfo.AdditionalInfo.TCPServiceURL '{0}'", processId));
                        serviceURL = runningProcessInfo.AdditionalInfo.TCPServiceURL;
                        s_processIdServiceUrls.Add(processId, serviceURL);
                    }
                }
            }
            return serviceURL;
        }

        RunningProcessInfo IRunningProcessManager.CurrentProcess
        {
            get { return RunningProcessManager.CurrentProcess; }
        }

        public bool TryLockRuntimeService(string serviceTypeUniqueName)
        {
            bool isLocked = false;
            RuntimeManagerClient.CreateClient((client, primaryNodeRuntimeNodeInstanceId) =>
                {
                    isLocked = client.TryLockRuntimeService(serviceTypeUniqueName, CurrentProcess.ProcessId);
                });
            return isLocked;
        }

        public bool TryGetRuntimeServiceProcessId(string serviceTypeUniqueName, out int runtimeProcessId)
        {
            int? runtimeProcessId_Internal = null;

            RuntimeManagerClient.CreateClient((client, primaryNodeRuntimeNodeInstanceId) =>
            {
                var response = client.TryGetServiceProcessId(new GetServiceProcessIdRequest
                    {
                        ServiceTypeUniqueName = serviceTypeUniqueName
                    });
                if (response != null && response.RuntimeProcessId.HasValue)
                    runtimeProcessId_Internal = response.RuntimeProcessId.Value;
            });

            if (runtimeProcessId_Internal.HasValue)
            {
                runtimeProcessId = runtimeProcessId_Internal.Value;
                return true;
            }
            else
            {
                runtimeProcessId = 0;
                return false;
            }
        }

        #region Mapper
        private RunningProcessDetails RunningProcessDetailMapper(RunningProcessDetails process)
        {
            return new RunningProcessDetails
            {
                ProcessId = process.ProcessId,
                OSProcessId = process.OSProcessId,
                RuntimeNodeId = process.RuntimeNodeId,
                RuntimeNodeInstanceId = process.RuntimeNodeInstanceId,
                StartedTime = process.StartedTime,
                AdditionalInfo = process.AdditionalInfo
            };
        }
        #endregion

        #region Private Classes

        internal class CacheManager: Vanrise.Caching.BaseCacheManager
        {
            int? _maxProcessId;
            int _processCount;

            protected override bool ShouldSetCacheExpired()
            {
                int? maxProcessIdFromDB;
                int processCountFromDB;
                s_dataManager.GetRunningProcessSummary(out maxProcessIdFromDB, out processCountFromDB);
                if(maxProcessIdFromDB != _maxProcessId || processCountFromDB != _processCount)
                {
                    _maxProcessId = maxProcessIdFromDB;
                    _processCount = processCountFromDB;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        #endregion
    }
}
