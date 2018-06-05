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
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Vanrise.Runtime
{
    internal class RuntimeManager
    {
        #region Static

        static TimeSpan s_RuntimeNodeHeartBeatTimeout;
        static int s_hostingRuntimeManagerWCFServiceMaxRetryCount;
        static TimeSpan s_removeLocksForNotRunningProcessInterval;

        static RuntimeManager()
        {
            if (!int.TryParse(ConfigurationManager.AppSettings["Runtime_HostingRuntimeManagerWCFServiceMaxRetryCount"], out s_hostingRuntimeManagerWCFServiceMaxRetryCount))
                s_hostingRuntimeManagerWCFServiceMaxRetryCount = 10;
            if (!TimeSpan.TryParse(ConfigurationManager.AppSettings["Runtime_RuntimeNodeHeartBeatTimeout"], out s_RuntimeNodeHeartBeatTimeout))
                s_RuntimeNodeHeartBeatTimeout = TimeSpan.FromSeconds(45);
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

        #region ctor

        internal RuntimeManager(RuntimeHost runtimeHost, Guid runtimeNodeId, Guid runtimeNodeInstanceId)
        {
            _runtimeHost = runtimeHost;
            _runtimeNodeId = runtimeNodeId;
            _runtimeNodeInstanceId = runtimeNodeInstanceId;

            //_cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            //_ramCounter = new PerformanceCounter("Memory", "Available MBytes");
            //ramTotMB = new PerformanceCounter("Memory", "Total MBytes");

            int retryCount = 0;
            while (true)
            {
                if (TryHostWCFService())
                    break;
                else
                {
                    retryCount++;
                    if (retryCount >= s_hostingRuntimeManagerWCFServiceMaxRetryCount)
                    {
                        LoggerFactory.GetLogger().WriteError("Max Retry Count '{0}' reached when trying to host RuntimeManagerWCFService. Runtime Node '{1}'", s_hostingRuntimeManagerWCFServiceMaxRetryCount, _runtimeNodeId);
                        Environment.Exit(0);
                    }
                    System.Threading.Thread.Sleep(200);
                }
            }
            if (!TrySetNodeStarted())
            {
                LoggerFactory.GetLogger().WriteError("Cannot Start Runtime Node. another Instance is already running for the Runtime Node '{0}'", _runtimeNodeId);
                Environment.Exit(0);
            }
            else
            {
                _lastHeartbeatTime = DateTime.Now;
            }
        }

        #endregion

        #region Local Variables

        Guid _runtimeNodeId;
        Guid _runtimeNodeInstanceId;
        string _serviceURL;
        RuntimeHost _runtimeHost;

        //PerformanceCounter _cpuCounter;
        //PerformanceCounter _ramCounter;

        IRuntimeManagerDataManager _dataManager = RuntimeDataManagerFactory.GetDataManager<IRuntimeManagerDataManager>();
        RunningProcessManager _runningProcessManager = new RunningProcessManager();
        IRunningProcessDataManager _runningProcessDataManager = RuntimeDataManagerFactory.GetDataManager<IRunningProcessDataManager>();
        IRuntimeServiceInstanceDataManager _runtimeServiceDataManager = RuntimeDataManagerFactory.GetDataManager<IRuntimeServiceInstanceDataManager>();
        IRuntimeNodeStateDataManager _runtimeNodeStateDataManager = RuntimeDataManagerFactory.GetDataManager<IRuntimeNodeStateDataManager>();
        List<int> _exitedChildProcessIds = new List<int>();

        internal bool _isPrimaryNodeReady;
        Object _runtimeManagerLockObj = new object();
        DateTime? _nextTimeToCheckTimedOutNodes;
        DateTime _lastHeartbeatTime;

        Guid _primaryNodeRuntimeNodeInstanceId;
        bool _areRuntimeProcessesChanged;
        bool _receivedProcessesAndServicesChangedFromPrimaryNode;

        #endregion

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

        #region Main Calls received from the RuntimeHost

        internal void Execute()
        {
            if (IsThisThePrimaryNode())
            {
                if (!_isPrimaryNodeReady)
                {
                    lock (_runtimeManagerLockObj)
                    {
                        TransactionLockHandler.InitializeCurrent();
                        _areRuntimeProcessesChanged = true;
                        _isPrimaryNodeReady = true;
                    }
                }
                UnregisterExitedChildProcesses();
                DeleteRunningProcessesForTimedOutNodes();
                NotifyRunningProcessesChangedIfNeeded();
            }
            else
            {
                lock (_runtimeManagerLockObj)
                {
                    TransactionLockHandler.RemoveCurrent();
                    _isPrimaryNodeReady = false;
                }
                UnregisterExitedChildProcesses();
                KillNotRegisteredChildProcessesIfProcessesChanged();
            }
        }

        internal void OnChildProcessExited(int processId)
        {
            lock (_exitedChildProcessIds)
            {
                _exitedChildProcessIds.Add(processId);
            }
        }

        #endregion

        #region Calls Received from other Nodes/Runtimes through the WCF service IRuntimeManagerWCFService

        internal bool IsThisRuntimeNodeInstanceRequestReceived(Guid runtimeNodeId, Guid instanceId)
        {
            return _runtimeNodeId == runtimeNodeId && _runtimeNodeInstanceId == instanceId;
        }

        internal PingPrimaryNodeResponse PingPrimaryNodeRequestReceived(PingPrimaryNodeRequest request)
        {
            if (!IsThisThePrimaryNodeAndItIsReady())
                throw new Exception(String.Format("this is not the primary node. Node Id '{0}'", _runtimeNodeId));
            var response = new PingPrimaryNodeResponse();

            if (request.RunningProcessesChangedInCurrentNode)
            {
                lock (_runtimeManagerLockObj)
                {
                    _areRuntimeProcessesChanged = true;
                }
            }
            return response;
        }

        internal RunningProcessRegistrationOutput RegisterRunningProcessRequestReceived(RunningProcessRegistrationInput input)
        {
            RunningProcessInfo runningProcessInfo = RuntimeDataManagerFactory.GetDataManager<IRunningProcessDataManager>().InsertProcessInfo(input.RuntimeNodeId, input.RuntimeNodeInstanceId, input.OSProcessId, input.AdditionalInfo);
            RuntimeServiceInstanceManager runtimeServiceInstanceManager = new RuntimeServiceInstanceManager();
            List<RuntimeServiceInstance> registeredServices = new List<RuntimeServiceInstance>();
            foreach (var runtimeServiceInput in input.RuntimeServices)
            {
                var serviceInstance = runtimeServiceInstanceManager.RegisterServiceInstance(runtimeServiceInput.ServiceInstanceId, runningProcessInfo.ProcessId, runtimeServiceInput.ServiceTypeUniqueName, runtimeServiceInput.ServiceInstanceInfo);
                registeredServices.Add(serviceInstance);
            }
            RunningProcessRegistrationOutput output = new RunningProcessRegistrationOutput
            {
                RunningProcessInfo = runningProcessInfo,
                RegisteredServices = registeredServices
            };
            if (_runtimeHost.IsHostForChildRuntimes())
            {
                bool isProcessIdAssignedToChildProcess = _runtimeHost.TrySetProcessIdOfChildProcess(input.OSProcessId, output.RunningProcessInfo.ProcessId);
                if (!isProcessIdAssignedToChildProcess)
                    OnChildProcessExited(runningProcessInfo.ProcessId);
            }
            lock (_runtimeManagerLockObj)
            {                
                _areRuntimeProcessesChanged = true;
            }

            return output;
        }

        internal void SetRuntimeProcessesAndServicesChanged()
        {
            lock (_runtimeManagerLockObj)
            {
                _receivedProcessesAndServicesChangedFromPrimaryNode = true;
            }
            RunningProcessManager.SetRunningProcessChanged();
            RuntimeServiceInstanceManager.SetServicesChanged();
        }

        #endregion

        #region Private Methods

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

        private bool TrySetNodeStarted()
        {
            var currentProcess = System.Diagnostics.Process.GetCurrentProcess();
            string machineName = Environment.MachineName;
            RuntimeNodeState nodeExistingInstanceState = _runtimeNodeStateDataManager.GetNodeState(_runtimeNodeId);
            if (nodeExistingInstanceState != null)
            {
                DateTime startOfTryingToSetStarted = DateTime.Now;
                while ((DateTime.Now - startOfTryingToSetStarted) <= s_RuntimeNodeHeartBeatTimeout)
                {
                    bool isNodeExistingInstanceAlive = false;
                    try
                    {
                        ServiceClientFactory.CreateTCPServiceClient<IRuntimeManagerWCFService>(nodeExistingInstanceState.ServiceURL,
                            (client) =>
                            {
                                isNodeExistingInstanceAlive = client.IsThisRuntimeNodeInstance(nodeExistingInstanceState.RuntimeNodeId, nodeExistingInstanceState.InstanceId);
                            });
                        if (isNodeExistingInstanceAlive)
                            return false;
                    }
                    catch
                    {

                    }
                    if (_runtimeNodeStateDataManager.TrySetInstanceStarted(_runtimeNodeId, _runtimeNodeInstanceId, machineName, currentProcess.Id, currentProcess.ProcessName, _serviceURL, s_RuntimeNodeHeartBeatTimeout))
                        return true;
                    else
                        Thread.Sleep(2000);
                }
                return false;
            }
            else
            {
                return _runtimeNodeStateDataManager.TrySetInstanceStarted(_runtimeNodeId, _runtimeNodeInstanceId, machineName, currentProcess.Id, currentProcess.ProcessName, _serviceURL, s_RuntimeNodeHeartBeatTimeout);
            }
        }

        private bool IsThisThePrimaryNode()
        {
            TryUpdateNodeHeartBeat();
            if (_isPrimaryNodeReady && _dataManager.TryTakePrimaryNode(_runtimeNodeInstanceId, s_RuntimeNodeHeartBeatTimeout))
            {
                return true;
            }
            else
            {
                DateTime startOfPing = DateTime.Now;
                while (true)
                {
                    if (TryPingCurrentPrimaryNode())
                    {
                        return false;
                    }
                    else
                    {
                        if (DateTime.Now - startOfPing > GetTimeOutToConsiderNodeDown())
                            KillAllRuntimeProcesses();

                        if (_dataManager.TryTakePrimaryNode(_runtimeNodeInstanceId, s_RuntimeNodeHeartBeatTimeout))
                            return true;
                        else
                            Thread.Sleep(1000);
                    }
                }
            }
        }

        private bool IsThisThePrimaryNodeAndItIsReady()
        {
            bool isReady;
            lock (_runtimeManagerLockObj)
            {
                isReady = _isPrimaryNodeReady;
            }
            return isReady;
        }

        private void TryUpdateNodeHeartBeat()
        {
            try
            {
                List<VRDiskInfo> diskInfos = GetDiskInfos();
                Decimal cpuUsage = 0M;//(Decimal)_cpuCounter.NextValue()
                Decimal availableRam = 0M;//(Decimal)_ramCounter.NextValue()
                if (!_runtimeNodeStateDataManager.TryUpdateHeartBeat(_runtimeNodeId, _runtimeNodeInstanceId, cpuUsage, availableRam,
                    Serializer.Serialize(diskInfos), _runtimeHost.NbOfEnabledProcesses, _runtimeHost.GetAllRunningProcessIds().Count))
                {
                    LoggerFactory.GetLogger().WriteError("Cannot Update Heart in Node '{0}', Service InstanceID '{1}'. Node Shutdown", _runtimeNodeId, _runtimeNodeInstanceId);
                    Environment.Exit(0);
                }
                else
                {
                    _lastHeartbeatTime = DateTime.Now;
                }
            }
            catch
            {
                if (DateTime.Now - _lastHeartbeatTime > GetTimeOutToConsiderNodeDown())
                    KillAllRuntimeProcesses();
                throw;
            }
        }

        private List<VRDiskInfo> GetDiskInfos()
        {
            List<VRDiskInfo> diskInfos = new List<VRDiskInfo>();
            foreach (System.IO.DriveInfo d in System.IO.DriveInfo.GetDrives())
            {
                if (d.IsReady)
                {
                    diskInfos.Add(new VRDiskInfo
                        {
                            PartitionName = d.Name,
                            AvailableFreeSpace = d.AvailableFreeSpace
                        });
                }
            }
            return diskInfos;
        }

        private void KillAllRuntimeProcesses()
        {
            _runtimeHost.KillAllChildProcesses();
        }

        private bool TryPingCurrentPrimaryNode()
        {
            try
            {
                var pingRequest = new PingPrimaryNodeRequest
                {
                    RuntimeNodeInstanceId = _runtimeNodeInstanceId,
                };

                RuntimeManagerClient.CreateClient((client, primaryNodeRuntimeNodeInstanceId) =>
                    {
                        if (primaryNodeRuntimeNodeInstanceId != _primaryNodeRuntimeNodeInstanceId//Primary Node is changed
                            || _areRuntimeProcessesChanged)
                        {
                            lock (_runtimeManagerLockObj)
                            {
                                _areRuntimeProcessesChanged = false;//can be set here because it is not the Primary Node
                            }
                            pingRequest.RunningProcessesChangedInCurrentNode = true;
                        }
                        client.PingPrimaryNode(pingRequest);
                        _primaryNodeRuntimeNodeInstanceId = primaryNodeRuntimeNodeInstanceId;

                    });
                return true;
            }
            catch
            {
                lock (_runtimeManagerLockObj)
                {
                    _areRuntimeProcessesChanged = true;
                }
                return false;
            }
        }

        private TimeSpan GetTimeOutToConsiderNodeDown()
        {
            return s_RuntimeNodeHeartBeatTimeout + TimeSpan.FromSeconds(30);
        }

        private void DeleteRunningProcessesForTimedOutNodes()
        {
            if (_nextTimeToCheckTimedOutNodes.HasValue && _nextTimeToCheckTimedOutNodes.Value > DateTime.Now)
                return;

            double timeOutIntervalInSec = GetTimeOutToConsiderNodeDown().TotalSeconds;
            var allNodeStates = _runtimeNodeStateDataManager.GetAllNodes();
            allNodeStates.ThrowIfNull("allNodeStates");
            var allNodeStatesByRuntimeNodeInstanceId = allNodeStates.ToDictionary(node => node.InstanceId, node => node);
            var allRuntimeProcesses = _runningProcessManager.GetRunningProcesses();
            foreach (var runtimeProcess in allRuntimeProcesses)
            {
                if (runtimeProcess.RuntimeNodeInstanceId == _runtimeNodeInstanceId)
                    continue;
                RuntimeNodeState nodeState;
                if (!allNodeStatesByRuntimeNodeInstanceId.TryGetValue(runtimeProcess.RuntimeNodeInstanceId, out nodeState) 
                    || nodeState.NbOfSecondsHeartBeatReceived > timeOutIntervalInSec)
                {
                    DeleteRunningProcess(runtimeProcess.ProcessId);
                }
            }

            DateTime? soonestProbableNodeTimeOut = null;
            foreach (var nodeState in allNodeStates)
            {
                if (nodeState.InstanceId == _runtimeNodeInstanceId)
                    continue;
                if (nodeState.NbOfSecondsHeartBeatReceived <= timeOutIntervalInSec)
                {
                    DateTime probableNodeTimeOut = DateTime.Now.AddSeconds(timeOutIntervalInSec - nodeState.NbOfSecondsHeartBeatReceived);
                    if (!soonestProbableNodeTimeOut.HasValue || soonestProbableNodeTimeOut.Value > probableNodeTimeOut)
                        soonestProbableNodeTimeOut = probableNodeTimeOut;
                }
            }

            if (soonestProbableNodeTimeOut.HasValue)
                _nextTimeToCheckTimedOutNodes = soonestProbableNodeTimeOut;
            else
                _nextTimeToCheckTimedOutNodes = DateTime.Now.AddSeconds(timeOutIntervalInSec);
        }

        private void UnregisterExitedChildProcesses()
        {
            while (_exitedChildProcessIds.Count > 0)
            {
                int firstChildProcessId = _exitedChildProcessIds[0];
                DeleteRunningProcess(firstChildProcessId);
                lock (_exitedChildProcessIds)
                {
                    _exitedChildProcessIds.Remove(firstChildProcessId);
                }
            }
        }

        private void NotifyRunningProcessesChangedIfNeeded()
        {
            if (_areRuntimeProcessesChanged)
            {
                try
                {
                    lock (_runtimeManagerLockObj)
                    {
                        _areRuntimeProcessesChanged = false;//clear the flag from the begining to handle the case when the running changes during the execution of this method
                    }
                    RunningProcessManager.SetRunningProcessChanged();
                    RuntimeServiceInstanceManager.SetServicesChanged();
                    Dictionary<int, RunningProcessInfo> allRegisteredRunningProcesses = _runningProcessManager.GetAllRunningProcesses();
                    allRegisteredRunningProcesses.ThrowIfNull("allRegisteredRunningProcesses");
                    foreach (var runningChildProcessId in _runtimeHost.GetAllRunningProcessIds())
                    {
                        if (!allRegisteredRunningProcesses.ContainsKey(runningChildProcessId))
                            _runtimeHost.KillChildProcess(runningChildProcessId);
                    }

                    double timeOutIntervalInSec = GetTimeOutToConsiderNodeDown().TotalSeconds;
                    List<RuntimeNodeState> otherRunningNodeStates = _runtimeNodeStateDataManager.GetAllNodes().Where(otherNodeState => otherNodeState.InstanceId != _runtimeNodeInstanceId && otherNodeState.NbOfSecondsHeartBeatReceived <= timeOutIntervalInSec).ToList();

                    bool errorWhenCallingOtherNodesAndRuntimes = false;
                    Parallel.ForEach(allRegisteredRunningProcesses.Values, (registeredProcess) =>
                    {
                        try
                        {
                            SetProcessesAndServicesChangedRequest setProcessesAndServicesChangedRequest = new SetProcessesAndServicesChangedRequest();
                            new InterRuntimeServiceManager().SendRequest(registeredProcess.ProcessId, setProcessesAndServicesChangedRequest);

                        }
                        catch (Exception ex)
                        {
                            LoggerFactory.GetExceptionLogger().WriteException(ex);
                            lock (_runtimeManagerLockObj)
                            {
                                errorWhenCallingOtherNodesAndRuntimes = true;
                            }
                        }
                    });

                    Parallel.ForEach(otherRunningNodeStates, (runningNodeState) =>
                    {
                        try
                        {
                            ServiceClientFactory.CreateTCPServiceClient<IRuntimeManagerWCFService>(runningNodeState.ServiceURL,
                                (client) =>
                                {
                                    client.SetRuntimeProcessesAndServicesChanged();
                                });

                        }
                        catch (Exception ex)
                        {
                            LoggerFactory.GetExceptionLogger().WriteException(ex);
                            lock (_runtimeManagerLockObj)
                            {
                                errorWhenCallingOtherNodesAndRuntimes = true;
                            }
                        }
                    });

                    TransactionLockHandler.Current.RemoveLocksForNotRunningProcesses(allRegisteredRunningProcesses.Keys.ToList());

                    if (errorWhenCallingOtherNodesAndRuntimes)
                    {
                        lock (_runtimeManagerLockObj)
                        {
                            _areRuntimeProcessesChanged = true;//rollback in case of exception
                        }
                    }
                }
                catch
                {
                    lock (_runtimeManagerLockObj)
                    {
                        _areRuntimeProcessesChanged = true;//rollback in case of exception
                    }
                    throw;
                }
            }
        }

        private void KillNotRegisteredChildProcessesIfProcessesChanged()
        {
            if (_receivedProcessesAndServicesChangedFromPrimaryNode)
            {
                lock (_runtimeManagerLockObj)
                {
                    if (_receivedProcessesAndServicesChangedFromPrimaryNode)
                        _receivedProcessesAndServicesChangedFromPrimaryNode = false;
                }
                try
                {
                    Dictionary<int, RunningProcessInfo> allRegisteredRunningProcesses = _runningProcessManager.GetAllRunningProcesses();
                    allRegisteredRunningProcesses.ThrowIfNull("allRegisteredRunningProcesses");
                    foreach (var runningChildProcessId in _runtimeHost.GetAllRunningProcessIds())
                    {
                        if (!allRegisteredRunningProcesses.ContainsKey(runningChildProcessId))
                            _runtimeHost.KillChildProcess(runningChildProcessId);
                    }
                }
                catch
                {
                    lock (_runtimeManagerLockObj)
                        _receivedProcessesAndServicesChangedFromPrimaryNode = true;
                    throw;
                }
            }
        }

        private void DeleteRunningProcess(int processId)
        {
            try
            {
                _runtimeServiceDataManager.DeleteByProcessId(processId);
                _runningProcessDataManager.DeleteRunningProcess(processId);
            }
            finally
            {
                lock (_runtimeManagerLockObj)
                {
                    _areRuntimeProcessesChanged = true;
                }
            }
        }

        #endregion
    }

    public class RunningProcessRegistrationInput
    {
        public int OSProcessId { get; set; }

        public Guid RuntimeNodeId { get; set; }

        public Guid RuntimeNodeInstanceId { get; set; }

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

        public List<RuntimeServiceInstance> RegisteredServices { get; set; }
    }
}
