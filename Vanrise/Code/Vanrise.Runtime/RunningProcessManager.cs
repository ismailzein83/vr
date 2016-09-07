using System;
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
        static RunningProcessManager()
        {
            _dataManager = RuntimeDataManagerFactory.GetDataManager<IRunningProcessDataManager>();           
            s_Timer = new Timer(7000);
            s_Timer.Elapsed += s_Timer_Elapsed;            
        }

        static string _runtimeManagerServiceURL;

        static void s_Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (s_lockObj)
            {
                if (s_isRunning)
                    return;
                s_isRunning = true;
            }
            try
            {
                if (_currentProcess == null)
                    InitializeCurrentProcessIfNotInitialized();
                else
                {
                    try
                    {
                        HeartBeatResponse hpResponse = null;
                        Common.ServiceClientFactory.CreateTCPServiceClient<IRuntimeManagerWCFService>(GetRuntimeManagerServiceURL(), (client) =>
                            {
                                hpResponse = client.UpdateHeartBeat(new HeartBeatRequest
                                    {
                                        RunningProcessId = CurrentProcess.ProcessId
                                    });
                            });
                        if (hpResponse == null || hpResponse.Result != HeartBeatResult.Succeeded)
                        {
                            Environment.Exit(0);
                        }
                    }
                    catch (Exception ex)
                    {
                        LoggerFactory.GetExceptionLogger().WriteException(ex);
                        _runtimeManagerServiceURL = null;
                    }
                }

                if (s_FreezedTransactionLocks.Count > 0 || s_queueFreezedTransactionLocks.Count > 0)
                    SaveFreezedTransactionLocks();
            }
            catch (Exception ex)
            {
                LoggerFactory.GetExceptionLogger().WriteException(ex);
            }
            lock (s_lockObj)
                s_isRunning = false;
        }

        private static string GetRuntimeManagerServiceURL()
        {
            if (_runtimeManagerServiceURL == null)
                _runtimeManagerServiceURL = RuntimeDataManagerFactory.GetDataManager<IRuntimeManagerDataManager>().GetRuntimeManagerServiceURL();
            if (_runtimeManagerServiceURL == null)
                throw new NullReferenceException("_runtimeManagerServiceURL");
            return _runtimeManagerServiceURL;
        }

        static List<Guid> s_FreezedTransactionLocks = new List<Guid>();
        private static void SaveFreezedTransactionLocks()
        {
            Guid lockItemId;
            while(s_queueFreezedTransactionLocks.TryDequeue(out lockItemId))
            {
                s_FreezedTransactionLocks.Add(lockItemId);
            }
            RuntimeDataManagerFactory.GetDataManager<IFreezedTransactionLockDataManager>().SaveFreezedLockTransaction(s_FreezedTransactionLocks);
            s_FreezedTransactionLocks.Clear();
        }

        static object s_lockObj = new object();
        static bool s_isRunning;
        static RunningProcessInfo _currentProcess;
        static IRunningProcessDataManager _dataManager;
        static Timer s_Timer;
        static ServiceHost s_serviceHost;

        public static RunningProcessInfo CurrentProcess
        {
            get
            {
                if(_currentProcess == null)
                {
                    InitializeCurrentProcessIfNotInitialized();
                }
                return _currentProcess;
            }
        }

        private static void InitializeCurrentProcessIfNotInitialized()
        {
            lock(s_lockObj)
            {
                if (_currentProcess == null)
                {
                    LoggerFactory.GetLogger().WriteInformation("Registering Runtime Host...");
                    string serviceURL;
                    s_serviceHost = ServiceHostManager.Current.CreateAndOpenTCPServiceHost(typeof(InterRuntimeWCFService), typeof(IInterRuntimeWCFService), OnServiceHostCreated, OnServiceHostRemoved, out serviceURL);
                    RunningProcessAdditionalInfo additionalInfo = new RunningProcessAdditionalInfo
                    {
                        TCPServiceURL = serviceURL
                    };
                    _currentProcess = _dataManager.InsertProcessInfo(System.Diagnostics.Process.GetCurrentProcess().ProcessName, Environment.MachineName, additionalInfo);
                    s_Timer.Enabled = true;
                    System.Threading.Thread.Sleep(3000);
                    LoggerFactory.GetLogger().WriteInformation("Runtime Host registered");
                }
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
            LoggerFactory.GetLogger().WriteInformation("InterRuntimeWCFService Service is opening..");
        }

        static void serviceHost_Opened(object sender, EventArgs e)
        {
            LoggerFactory.GetLogger().WriteInformation("InterRuntimeWCFService Service opened");
        }

        static void serviceHost_Closed(object sender, EventArgs e)
        {
            LoggerFactory.GetLogger().WriteInformation("InterRuntimeWCFService Service closed");
        }

        static void serviceHost_Closing(object sender, EventArgs e)
        {
            LoggerFactory.GetLogger().WriteInformation("InterRuntimeWCFService is closing..");
        }

        #endregion

        public Dictionary<int, RunningProcessInfo> GetAllRunningProcesses()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetAllRunningProcesses",
               () =>
               {
                   IRunningProcessDataManager dataManager = RuntimeDataManagerFactory.GetDataManager<IRunningProcessDataManager>();
                   return dataManager.GetRunningProcesses().ToDictionary(itm => itm.ProcessId, itm => itm);
               });
        }

        public List<RunningProcessInfo> GetRunningProcesses()
        {
            return GetAllRunningProcesses().Values.ToList();
        }

        public List<RunningProcessInfo> GetCachedRunningProcesses()
        {
            return GetRunningProcesses();
        }

        public List<RunningProcessInfo> GetCachedRunningProcesses(TimeSpan maxCacheTime)
        {
            return GetRunningProcesses();
        }

        internal string GetProcessTCPServiceURL(int processId)
        {
            var allProcesses = GetAllRunningProcesses();
            RunningProcessInfo runningProcessInfo = allProcesses.GetRecord(processId);
            if(runningProcessInfo == null && processId > allProcesses.Keys.Max())//the processId might be a new process
            {
                Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                runningProcessInfo = GetAllRunningProcesses().GetRecord(processId);
            }
            if (runningProcessInfo == null)
                throw new NullReferenceException(String.Format("runningProcessInfo '{0}'", processId));
            if (runningProcessInfo.AdditionalInfo == null)
                throw new NullReferenceException(String.Format("runningProcessInfo.AdditionalInfo '{0}'", processId));
            if (runningProcessInfo.AdditionalInfo.TCPServiceURL == null)
                throw new NullReferenceException(String.Format("runningProcessInfo.AdditionalInfo.TCPServiceURL '{0}'", processId));
            return runningProcessInfo.AdditionalInfo.TCPServiceURL;
        }

        RunningProcessInfo IRunningProcessManager.CurrentProcess
        {
            get { return RunningProcessManager.CurrentProcess; }
        }

        static ConcurrentQueue<Guid> s_queueFreezedTransactionLocks = new ConcurrentQueue<Guid>();
        internal void SetTransactionLockFreezed(TransactionLockItem transactionLockItem)
        {
            s_queueFreezedTransactionLocks.Enqueue(transactionLockItem.LockItemUniqueId);
        }

        public bool TryLockRuntimeService(string serviceTypeUniqueName)
        {
            bool isLocked = false;
            try
            {
                ServiceClientFactory.CreateTCPServiceClient<IRuntimeManagerWCFService>(GetRuntimeManagerServiceURL(), (client) =>
                    {
                        isLocked = client.TryLockRuntimeService(serviceTypeUniqueName, CurrentProcess.ProcessId);
                    });
            }
            catch
            {
                _runtimeManagerServiceURL = null;
                throw;
            }
            return isLocked;
        }

        public bool TryGetRuntimeServiceProcessId(string serviceTypeUniqueName, out int runtimeProcessId)
        {
            int? runtimeProcessId_Internal = null;
            try
            {
                ServiceClientFactory.CreateTCPServiceClient<IRuntimeManagerWCFService>(GetRuntimeManagerServiceURL(), (client) =>
                {
                    var response = client.TryGetServiceProcessId(new GetServiceProcessIdRequest
                        {
                            ServiceTypeUniqueName = serviceTypeUniqueName
                        });
                    if (response != null && response.RuntimeProcessId.HasValue)
                        runtimeProcessId_Internal = response.RuntimeProcessId.Value;
                });
            }
            catch
            {
                _runtimeManagerServiceURL = null;
                throw;
            }
            if(runtimeProcessId_Internal.HasValue)
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

        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IRunningProcessDataManager _dataManager = RuntimeDataManagerFactory.GetDataManager<IRunningProcessDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired()
            {
                return _dataManager.AreRunningProcessesUpdated(ref _updateHandle);
            }
        }

        #endregion
    }
}
