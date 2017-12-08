﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Vanrise.Common;
using Vanrise.Runtime.Data;
using Vanrise.Runtime.Entities;

namespace Vanrise.Runtime
{
    public class RunningProcessManager : IRunningProcessManager
    {

        static RunningProcessInfo s_currentProcess;
        static List<RunningProcessInfo> s_allRunningProcesses;
        static Dictionary<int, RunningProcessInfo> s_allRunningProcessesDict;
        static Object s_lockObj = new object();
        static DateTime s_lastReceivedPingTime;

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

        internal static DateTime LastReceivedPingTime
        {
            get
            {
                return s_lastReceivedPingTime;
            }
            set
            {
                lock (s_lockObj)
                {
                    s_lastReceivedPingTime = value;
                }
            }
        }

        public static bool IsCurrentProcessARuntime
        {
            get
            {
                return s_currentProcess != null;
            }
        }

        internal static void SetRunningProcesses(List<RunningProcessInfo> allRunningProcesses)
        {
            var allRunningProcessesLocal = new List<RunningProcessInfo>(allRunningProcesses);
            Dictionary<int, RunningProcessInfo> dict = allRunningProcesses.ToDictionary(itm => itm.ProcessId, itm => itm);
            lock (s_lockObj)
            {
                s_allRunningProcesses = allRunningProcessesLocal;
                s_allRunningProcessesDict = dict;
            }
        }

        public Dictionary<int, RunningProcessInfo> GetAllRunningProcesses()
        {
            s_allRunningProcessesDict.ThrowIfNull("s_allRunningProcessesDict");
             return s_allRunningProcessesDict;
        }

        internal List<RunningProcessInfo> GetRunningProcessesFromDB()
        {
            s_allRunningProcesses.ThrowIfNull("s_allRunningProcesses");
            return s_allRunningProcesses;
        }

        public List<RunningProcessInfo> GetRunningProcesses()
        {
            return GetRunningProcessesFromDB();
        }

        public List<RunningProcessInfo> GetCachedRunningProcesses()
        {
            return GetRunningProcessesFromDB();
        }

        public List<RunningProcessInfo> GetCachedRunningProcesses(TimeSpan maxCacheTime)
        {
            return GetRunningProcessesFromDB();
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
                        RunningProcessInfo runningProcessInfo = GetAllRunningProcesses().GetRecord(processId);
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
            RuntimeManagerClient.CreateClient((client) =>
                {
                    isLocked = client.TryLockRuntimeService(serviceTypeUniqueName, CurrentProcess.ProcessId);
                });
            return isLocked;
        }

        public bool TryGetRuntimeServiceProcessId(string serviceTypeUniqueName, out int runtimeProcessId)
        {
            int? runtimeProcessId_Internal = null;

            RuntimeManagerClient.CreateClient((client) =>
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
    }
}
