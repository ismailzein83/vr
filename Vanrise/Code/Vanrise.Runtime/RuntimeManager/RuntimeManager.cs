﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Runtime.Entities;
using Vanrise.Common;
using System.Configuration;
using Vanrise.Runtime.Data;
using System.ServiceModel;

namespace Vanrise.Runtime
{
    internal class RuntimeManager
    {
        #region Static

        static TimeSpan s_RunningProcessHeartBeatTimeout;
        static int s_hostingRuntimeManagerWCFServiceMaxRetryCount;
        static int s_pingRunningProcessMaxRetryCount;
        static TimeSpan s_pingRunningProcessRetryOffset;
        static TimeSpan s_removeLocksForNotRunningProcessInterval;

        static RuntimeManager()
        {
            if (!int.TryParse(ConfigurationManager.AppSettings["Runtime_HostingRuntimeManagerWCFServiceMaxRetryCount"], out s_hostingRuntimeManagerWCFServiceMaxRetryCount))
                s_hostingRuntimeManagerWCFServiceMaxRetryCount = 10;
            if (!TimeSpan.TryParse(ConfigurationManager.AppSettings["Runtime_RunningProcessHeartBeatTimeout"], out s_RunningProcessHeartBeatTimeout))
                s_RunningProcessHeartBeatTimeout = TimeSpan.FromSeconds(120);
            if (!int.TryParse(ConfigurationManager.AppSettings["Runtime_PingRunningProcessMaxRetryCount"], out s_pingRunningProcessMaxRetryCount))
                s_pingRunningProcessMaxRetryCount = 3;
            if (!TimeSpan.TryParse(ConfigurationManager.AppSettings["Runtime_PingRunningProcessRetryOffset"], out s_pingRunningProcessRetryOffset))
                s_pingRunningProcessRetryOffset = TimeSpan.FromSeconds(1);
            if (!TimeSpan.TryParse(ConfigurationManager.AppSettings["Runtime_RemoveLocksForNotRunningProcessInterval"], out s_removeLocksForNotRunningProcessInterval))
                s_removeLocksForNotRunningProcessInterval = TimeSpan.FromSeconds(60);
        }

        #endregion

        Guid _serviceInstanceId;
        string _serviceURL;

        internal RuntimeManager()
        {            
            int retryCount = 0;
            while (true)
            {
                if (TryHostWCFService())
                    break;
                else
                {
                    retryCount++;
                    if (retryCount >= s_hostingRuntimeManagerWCFServiceMaxRetryCount)
                        throw new Exception(String.Format("Max Retry Count '{0}' reached when trying to host RuntimeManagerWCFService", s_hostingRuntimeManagerWCFServiceMaxRetryCount));
                    System.Threading.Thread.Sleep(200);
                }
            }
            _serviceInstanceId = Guid.NewGuid();
        }
        private bool TryHostWCFService()
        {            
            try
            {
                ServiceHostManager.Current.CreateAndOpenTCPServiceHost(typeof(RuntimeManagerWCFService), typeof(IRuntimeManagerWCFService), OnServiceHostCreated, OnServiceHostRemoved, out _serviceURL);
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        #region WCF Host Events

        static void OnServiceHostCreated(ServiceHost serviceHost)
        {
            serviceHost.Opening += serviceHost_Opening;
            serviceHost.Opened += serviceHost_Opened;
            serviceHost.Closing += serviceHost_Closing;
            serviceHost.Closed += serviceHost_Closed;
        }

        static void OnServiceHostRemoved(ServiceHost serviceHost)
        {
            serviceHost.Opening -= serviceHost_Opening;
            serviceHost.Opened -= serviceHost_Opened;
            serviceHost.Closing -= serviceHost_Closing;
            serviceHost.Closed -= serviceHost_Closed;
        }

        static void serviceHost_Opening(object sender, EventArgs e)
        {
            LoggerFactory.GetLogger().WriteInformation("Runtime Manager Service is opening..");
        }

        static void serviceHost_Opened(object sender, EventArgs e)
        {
            LoggerFactory.GetLogger().WriteInformation("Runtime Manager Service opened");
        }

        static void serviceHost_Closed(object sender, EventArgs e)
        {
            LoggerFactory.GetLogger().WriteInformation("Runtime Manager Service closed");
        }

        static void serviceHost_Closing(object sender, EventArgs e)
        {
            LoggerFactory.GetLogger().WriteInformation("Runtime Manager is closing..");
        }

        #endregion

        IRuntimeManagerDataManager _dataManager = RuntimeDataManagerFactory.GetDataManager<IRuntimeManagerDataManager>();

        static DateTime s_lastTimeLocksRemovedForNotRunningProcesses;

        internal void Execute()
        {
            var runningProcessesIds = new RunningProcessManager().GetRunningProcessesFromDB().Select(itm => itm.ProcessId).ToList();
            if (IsCurrentRuntimeAManager())
            {
                if (ProcessHeartBeatManager.s_current == null)
                {
                    ProcessHeartBeatManager.s_current = new ProcessHeartBeatManager(runningProcessesIds);
                    TransactionLockHandler.InitializeCurrent();
                }
                else if ((DateTime.Now - s_lastTimeLocksRemovedForNotRunningProcesses) > s_removeLocksForNotRunningProcessInterval)
                {
                    TransactionLockHandler.Current.RemoveLocksForNotRunningProcesses(runningProcessesIds);
                    s_lastTimeLocksRemovedForNotRunningProcesses = DateTime.Now;
                }
                PingRunningProcesses(runningProcessesIds);
                new RuntimeServiceInstanceManager().DeleteNonRunningServices();
            }
            else
            {
                TransactionLockHandler.RemoveCurrent();
                ProcessHeartBeatManager.s_current = null;
            }
        }

        private bool IsCurrentRuntimeAManager()
        {
            try
            {
                return _dataManager.TryUpdateHeartBeat(_serviceInstanceId, _serviceURL, s_RunningProcessHeartBeatTimeout);
            }
            catch (Exception ex)
            {
                LoggerFactory.GetExceptionLogger().WriteException(ex);
                return false;
            }
        }

        private void PingRunningProcesses(List<int> runningProcessesIds)
        {
            var processesHeartBeatInfo = ProcessHeartBeatManager.Current.GetProcessesHBInfo();
            List<int> processesIdsToPing = new List<int>();
            foreach (var runningProcessId in runningProcessesIds)
            {
                ProcessHBInfo processHBInfo;
                if(!processesHeartBeatInfo.TryGetValue(runningProcessId, out processHBInfo) || (DateTime.Now - processHBInfo.LastHeartBeatTime) > s_RunningProcessHeartBeatTimeout)
                {
                    processesIdsToPing.Add(runningProcessId);
                }
            }

            if (processesIdsToPing.Count > 0)
            {
                PingAndDeleteNonReachable(processesIdsToPing);
            }

            //remove non available process Ids from HeartBeat manager
            foreach(var processId in processesHeartBeatInfo.Keys.Where(hbProcessId => !runningProcessesIds.Contains(hbProcessId)))
            {
                ProcessHeartBeatManager.Current.SetProcessNotAvailable(processId);
            }
        }

        private void PingAndDeleteNonReachable(List<int> processesIdsToPing)
        {
            Parallel.ForEach(processesIdsToPing, (runningProcessId) =>
            {
                int retryCount = 0;
                while (retryCount < s_pingRunningProcessMaxRetryCount)
                {
                    try
                    {
                        if (!TryPing(runningProcessId))
                        {
                            DeleteRunningProcess(runningProcessId);
                        }
                        break;
                    }
                    catch
                    {
                        retryCount++;
                        if (retryCount >= s_pingRunningProcessMaxRetryCount)
                        {
                            DeleteRunningProcess(runningProcessId);
                            break;                       
                        }
                        else
                        {                            
                            System.Threading.Thread.Sleep(s_pingRunningProcessRetryOffset);
                        }
                    }
                }
            });
        }

        private bool TryPing(int runningProcessId)
        {
            PingRunningProcessServiceRequest pingRequest = new PingRunningProcessServiceRequest
            {
                RunningProcessId = runningProcessId
            };
            var response = new InterRuntimeServiceManager().SendRequest(runningProcessId, pingRequest);
            return response != null && response.Result == PingRunningProcessResult.Succeeded;
        }

        private void DeleteRunningProcess(int runningProcessId)
        {
            IRunningProcessDataManager dataManager = RuntimeDataManagerFactory.GetDataManager<IRunningProcessDataManager>();
            dataManager.DeleteRunningProcess(runningProcessId);
            ProcessHeartBeatManager.Current.SetProcessNotAvailable(runningProcessId);
        }
    }
}
