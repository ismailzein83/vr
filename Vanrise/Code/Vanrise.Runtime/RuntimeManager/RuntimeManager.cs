using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Runtime.Entities;
using Vanrise.Common;
using System.Configuration;
using Vanrise.Runtime.Data;
using System.ServiceModel;
using System.Threading;

namespace Vanrise.Runtime
{
    internal class RuntimeManager
    {
        #region Static

        static TimeSpan s_RuntimeManagerHeartBeatTimeout;
        internal static TimeSpan s_RunningProcessHeartBeatTimeout;
        static int s_hostingRuntimeManagerWCFServiceMaxRetryCount;
        static int s_pingRunningProcessMaxRetryCount;
        static TimeSpan s_removeLocksForNotRunningProcessInterval;

        List<RunningProcessInfo> _runningProcesses;
        Dictionary<int, RunningProcessSyncInfo> _runningProcessSyncInfos;
        List<RuntimeServiceInstance> _runtimeServiceInstances;

        static RuntimeManager()
        {
            if (!int.TryParse(ConfigurationManager.AppSettings["Runtime_HostingRuntimeManagerWCFServiceMaxRetryCount"], out s_hostingRuntimeManagerWCFServiceMaxRetryCount))
                s_hostingRuntimeManagerWCFServiceMaxRetryCount = 10;
            if (!TimeSpan.TryParse(ConfigurationManager.AppSettings["Runtime_RuntimeManagerHeartBeatTimeout"], out s_RuntimeManagerHeartBeatTimeout))
                s_RuntimeManagerHeartBeatTimeout = TimeSpan.FromSeconds(45);
            if (!TimeSpan.TryParse(ConfigurationManager.AppSettings["Runtime_RunningProcessHeartBeatTimeout"], out s_RunningProcessHeartBeatTimeout))
                s_RunningProcessHeartBeatTimeout = TimeSpan.FromSeconds(90);
            if (s_RunningProcessHeartBeatTimeout - s_RuntimeManagerHeartBeatTimeout < TimeSpan.FromSeconds(30))
                throw new Exception("RunningProcessHeartBeatTimeout should be greater than RuntimeManagerHeartBeatTimeout by 30 sec at least");
            if (!int.TryParse(ConfigurationManager.AppSettings["Runtime_PingRunningProcessMaxRetryCount"], out s_pingRunningProcessMaxRetryCount))
                s_pingRunningProcessMaxRetryCount = 3;
            if (!TimeSpan.TryParse(ConfigurationManager.AppSettings["Runtime_RemoveLocksForNotRunningProcessInterval"], out s_removeLocksForNotRunningProcessInterval))
                s_removeLocksForNotRunningProcessInterval = TimeSpan.FromSeconds(60);
        }

        static RuntimeManager s_current;
        internal static RuntimeManager Current
        {
            get
            {
                return s_current;
            }
            set
            {
                if (s_current != null)
                    throw new Exception("Current RuntimeManager already initialized");
                s_current = value;
            }
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
            catch (Exception ex)
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
        IRunningProcessDataManager _runningProcessDataManager = RuntimeDataManagerFactory.GetDataManager<IRunningProcessDataManager>();
        IRuntimeServiceInstanceDataManager _runtimeServiceDataManager = RuntimeDataManagerFactory.GetDataManager<IRuntimeServiceInstanceDataManager>();

        static DateTime s_lastTimeLocksRemovedForNotRunningProcesses;

        internal bool _isCurrentRuntimeManagerReady;
        Object _runtimeManagerLockObj = new object();

        internal void Execute(bool killTimedOutProcesses)
        {
            if (IsCurrentRuntimeAManager())
            {
                if (!_isCurrentRuntimeManagerReady)
                {
                    TransactionLockHandler.InitializeCurrent();
                    var runningProcesses = _runningProcessDataManager.GetRunningProcesses();
                    var runtimeServiceInstances = _runtimeServiceDataManager.GetServices();

                    lock (_runtimeManagerLockObj)
                    {
                        _runningProcesses = runningProcesses;
                        _runtimeServiceInstances = runtimeServiceInstances;
                        _runningProcessSyncInfos = _runningProcesses.ToDictionary(itm => itm.ProcessId, itm => new RunningProcessSyncInfo { RunningProcessInfo = itm, NeedsRunningProcessesUpdate = true });
                        RunningProcessManager.SetRunningProcesses(_runningProcesses);
                        RuntimeServiceInstanceManager.SetRuntimeServices(_runtimeServiceInstances);
                        _isCurrentRuntimeManagerReady = true;
                    }
                    killTimedOutProcesses = true;
                }
                if ((DateTime.Now - s_lastTimeLocksRemovedForNotRunningProcesses) > s_removeLocksForNotRunningProcessInterval)
                {
                    List<int> runningProcessIds;
                    lock (_runtimeManagerLockObj)
                    {
                        runningProcessIds = new List<int>(_runningProcessSyncInfos.Keys);
                    }
                    TransactionLockHandler.Current.RemoveLocksForNotRunningProcesses(runningProcessIds);
                    s_lastTimeLocksRemovedForNotRunningProcesses = DateTime.Now;
                }
                if (killTimedOutProcesses)
                {
                    for (int i = 0; i < s_pingRunningProcessMaxRetryCount; i++)
                    {
                        PingRunningProcesses();
                    }
                }
                else
                {
                    PingRunningProcesses();
                }
            }
            else
            {
                lock (_runtimeManagerLockObj)
                {
                    TransactionLockHandler.RemoveCurrent();
                    _runningProcesses = null;
                    _runtimeServiceInstances = null;
                    _runningProcessSyncInfos = null;
                    _isCurrentRuntimeManagerReady = false;
                }
            }
        }

        private bool IsCurrentRuntimeAManager()
        {
            while (!_dataManager.TryUpdateHeartBeat(_serviceInstanceId, _serviceURL, s_RuntimeManagerHeartBeatTimeout))
            {
                if (TryPingCurrentRuntimeManager())
                {
                    return false;
                }
                else
                {
                    Thread.Sleep(1000);
                }
            }
            return true;
        }

        private bool TryPingCurrentRuntimeManager()
        {
            bool pingSucceeded = false;
            try
            {

                RuntimeManagerClient.CreateClient((client) =>
                    {
                        pingSucceeded = client.IsThisCurrentRuntimeManager();
                    });
            }
            catch
            {
                pingSucceeded = false;
            }
            return pingSucceeded;
        }

        private void PingRunningProcesses()
        {
            List<RunningProcessSyncInfo> runningProcessSyncInfos;
            lock (_runtimeManagerLockObj)
            {
                runningProcessSyncInfos = new List<RunningProcessSyncInfo>(_runningProcessSyncInfos.Values);
            }
            Parallel.ForEach(runningProcessSyncInfos, (runningProcessSyncInfo) =>
            {
                try
                {
                    if (!TryPing(runningProcessSyncInfo))
                    {
                        DeleteRunningProcess(runningProcessSyncInfo);
                    }
                }
                catch (Exception ex)
                {
                    runningProcessSyncInfo.FailedRetryCount++;
                    if (runningProcessSyncInfo.FailedRetryCount >= s_pingRunningProcessMaxRetryCount && (DateTime.Now - runningProcessSyncInfo.LastHeartBeatTime) >= s_RunningProcessHeartBeatTimeout)
                    {
                        DeleteRunningProcess(runningProcessSyncInfo);
                    }
                }
            });
        }

        private bool TryPing(RunningProcessSyncInfo runningProcessSyncInfo)
        {
            PingRunningProcessServiceRequest pingRequest = new PingRunningProcessServiceRequest
            {
                RunningProcessId = runningProcessSyncInfo.RunningProcessInfo.ProcessId
            };
            if (runningProcessSyncInfo.NeedsRunningProcessesUpdate)
            {
                lock (_runtimeManagerLockObj)
                {
                    pingRequest.NewRunningProcesses = new List<RunningProcessInfo>(_runningProcesses);
                    pingRequest.NewRuntimeServices = new List<RuntimeServiceInstance>(_runtimeServiceInstances);
                }
            }

            var response = new InterRuntimeServiceManager().SendRequest(runningProcessSyncInfo.RunningProcessInfo.ProcessId, pingRequest);
            if (response != null && response.Result == PingRunningProcessResult.Succeeded)
            {
                lock (_runtimeManagerLockObj)
                {
                    runningProcessSyncInfo.NeedsRunningProcessesUpdate = false;
                    runningProcessSyncInfo.LastHeartBeatTime = DateTime.Now;
                    runningProcessSyncInfo.FailedRetryCount = 0;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private void DeleteRunningProcess(RunningProcessSyncInfo runningProcessSyncInfo)
        {
            HashSet<RuntimeServiceInstance> runtimeServicesToDelete;
            lock (_runtimeManagerLockObj)
            {
                runtimeServicesToDelete = new HashSet<RuntimeServiceInstance>(_runtimeServiceInstances.Where(itm => itm.ProcessId == runningProcessSyncInfo.RunningProcessInfo.ProcessId));
            }
            foreach (var runtimeService in runtimeServicesToDelete)
            {
                _runtimeServiceDataManager.Delete(runtimeService.ServiceInstanceId);
            }
            _runningProcessDataManager.DeleteRunningProcess(runningProcessSyncInfo.RunningProcessInfo.ProcessId);
            lock (_runtimeManagerLockObj)
            {
                _runtimeServiceInstances.RemoveAll(itm => runtimeServicesToDelete.Contains(itm));
                _runningProcessSyncInfos.Remove(runningProcessSyncInfo.RunningProcessInfo.ProcessId);
                _runningProcesses.Remove(runningProcessSyncInfo.RunningProcessInfo);
                RunningProcessManager.SetRunningProcesses(_runningProcesses);
                RuntimeServiceInstanceManager.SetRuntimeServices(_runtimeServiceInstances);
                foreach (var syncInfo in _runningProcessSyncInfos.Values)
                {
                    syncInfo.NeedsRunningProcessesUpdate = true;
                }
            }
        }

        internal RunningProcessRegistrationOutput RegisterRunningProcess(RunningProcessRegistrationInput input)
        {
            if (!_isCurrentRuntimeManagerReady)
                throw new Exception("Runtime Manager is not current instance");
            RunningProcessInfo runningProcessInfo = RuntimeDataManagerFactory.GetDataManager<IRunningProcessDataManager>().InsertProcessInfo(input.ProcessName, input.MachineName, input.AdditionalInfo);
            RuntimeServiceInstanceManager runtimeServiceInstanceManager = new RuntimeServiceInstanceManager();
            List<RuntimeServiceInstance> registeredServices = new List<RuntimeServiceInstance>();
            foreach (var runtimeServiceInput in input.RuntimeServices)
            {
                var serviceInstance = runtimeServiceInstanceManager.RegisterServiceInstance(runtimeServiceInput.ServiceInstanceId, runningProcessInfo.ProcessId, runtimeServiceInput.ServiceTypeUniqueName, runtimeServiceInput.ServiceInstanceInfo);
                registeredServices.Add(serviceInstance);
            }
            lock (_runtimeManagerLockObj)
            {
                _runningProcesses.Add(runningProcessInfo);
                _runningProcessSyncInfos.Add(runningProcessInfo.ProcessId, new RunningProcessSyncInfo { RunningProcessInfo = runningProcessInfo });
                _runtimeServiceInstances.AddRange(registeredServices);
                RunningProcessManager.SetRunningProcesses(_runningProcesses);
                RuntimeServiceInstanceManager.SetRuntimeServices(_runtimeServiceInstances);
                foreach (var syncInfo in _runningProcessSyncInfos.Values)
                {
                    syncInfo.NeedsRunningProcessesUpdate = true;
                }
            }
            RunningProcessRegistrationOutput output;
            lock (_runtimeManagerLockObj)
            {
                output = new RunningProcessRegistrationOutput
                    {
                        RunningProcessInfo = runningProcessInfo,
                        RegisteredServices = registeredServices,
                        AllRunningProcesses = new List<RunningProcessInfo>(_runningProcesses),
                        AllRunningServices = new List<RuntimeServiceInstance>(_runtimeServiceInstances)
                    };
            }
            return output;
        }

        internal bool IsRunningProcessStillRegistered(int runningProcessId)
        {
            bool isRunningProcessStillRegistered = false;
            lock(_runtimeManagerLockObj)
            {
                RunningProcessSyncInfo runningProcessSyncInfo;
                isRunningProcessStillRegistered = _runningProcessSyncInfos.TryGetValue(runningProcessId, out runningProcessSyncInfo);
                if (isRunningProcessStillRegistered)
                    runningProcessSyncInfo.LastHeartBeatTime = DateTime.Now;
            }
            return isRunningProcessStillRegistered;
        }

        #region Private Classes

        private class RunningProcessSyncInfo
        {
            public RunningProcessInfo RunningProcessInfo { get; set; }

            public DateTime LastHeartBeatTime { get; set; }

            public int FailedRetryCount { get; set; }

            public bool NeedsRunningProcessesUpdate { get; set; }
        }

        #endregion
    }

    public class RunningProcessRegistrationInput
    {
        public string ProcessName { get; set; }

        public string MachineName { get; set; }

        public List<RegisterRuntimeServiceInput> RuntimeServices { get; set; }

        public RunningProcessAdditionalInfo AdditionalInfo { get; set; }
    }

    public class RegisterRuntimeServiceInput
    {
        public Guid ServiceInstanceId { get; set; }

        public string ServiceTypeUniqueName { get; set; }

        public ServiceInstanceInfo ServiceInstanceInfo { get; set; }
    }

    public class RunningProcessRegistrationOutput
    {
        public RunningProcessInfo RunningProcessInfo { get; set; }

        public List<RunningProcessInfo> AllRunningProcesses { get; set; }

        public List<RuntimeServiceInstance> RegisteredServices { get; set; }

        public List<RuntimeServiceInstance> AllRunningServices { get; set; }
    }
}
