using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Vanrise.Common;
using Vanrise.Runtime.Business;
using Vanrise.Runtime.Data;
using Vanrise.Runtime.Entities;

namespace Vanrise.Runtime
{
    public enum RuntimeStatus { NotStarted, Started, Stopped }
    public class RuntimeHost
    {
        #region Singleton

        static RuntimeHost _current;
        internal static RuntimeHost Current
        {
            get
            {
                _current.ThrowIfNull("_current");
                return _current;
            }
        }

        #endregion

        #region ctor

        private RuntimeHost()
        {
            Vanrise.Common.Utilities.CompilePredefinedPropValueReaders();
            lock (typeof(RuntimeHost))
            {
                if (_current != null)
                    throw new Exception("Only one instance of RuntimeHost can be created");
                _current = this;
            }
        }

        public RuntimeHost(List<RuntimeService> services)
            : this()
        {
            _runtimeNodeId = GetRuntimeNodeIdFromConfig();
            var runtimeNode = GetRuntimeNodeWithValidate();//just to make sure node exists
            _runtimeNodeInstanceId = Guid.NewGuid();
            _numberOfEnabledProcesses = 1;
            InitializeRuntimeManager(true);
            _serviceExecutors = services != null ? services.Select(s => new RuntimeServiceExecutor(s)).ToList() : null;
        }

        public RuntimeHost(string[] applicationArgs)
            : this()
        {
            if (applicationArgs == null || applicationArgs.Length == 0)//this means it is the RuntimeManager of the Node (parent process)
            {
                _runtimeNodeId = GetRuntimeNodeIdFromConfig();
                var runtimeNodeConfiguration = GetRuntimeNodeConfigWithValidate();
                _runtimeNodeInstanceId = Guid.NewGuid();
                _isHostForChildRuntimes = true;
                _childRuntimeProcessProxies = new List<ChildRuntimeProcessProxy>();
                foreach (var processConfigEntry in runtimeNodeConfiguration.Settings.Processes)
                {
                    Guid processConfigId = processConfigEntry.Key;
                    processConfigEntry.Value.ThrowIfNull("processConfigEntry.Value", processConfigId);
                    processConfigEntry.Value.Settings.ThrowIfNull("processConfigEntry.Value.Settings", processConfigId);
                    if (processConfigEntry.Value.Settings.IsEnabled)
                    {
                        _numberOfEnabledProcesses += processConfigEntry.Value.Settings.NbOfInstances;
                        for (int i = 0; i < processConfigEntry.Value.Settings.NbOfInstances; i++)
                        {
                            _childRuntimeProcessProxies.Add(new ChildRuntimeProcessProxy
                            {
                                ProcessConfigId = processConfigId
                            });
                        }
                    }
                }
                InitializeRuntimeManager(false);
            }
            else//this means it is a Runtime Process (child process)
            {
                int parentProcessId = int.Parse(applicationArgs[1]);
                Console.WriteLine("Parent Process Id: {0}", parentProcessId);
                var parentProcess = Process.GetProcessById(parentProcessId);
                parentProcess.EnableRaisingEvents = true;
                parentProcess.Exited += (sender, e) =>
                {
                    Environment.Exit(0);
                };

                _runtimeNodeId = Guid.Parse(applicationArgs[2]);
                var runtimeNodeConfiguration = GetRuntimeNodeConfigWithValidate();
                _runtimeNodeInstanceId = Guid.Parse(applicationArgs[3]);
                Guid processConfigId = Guid.Parse(applicationArgs[4]);
                var processConfig = runtimeNodeConfiguration.Settings.Processes.GetRecord(processConfigId);
                processConfig.ThrowIfNull("processConfig", processConfigId);
                processConfig.Settings.ThrowIfNull("processConfig.Settings", processConfigId);
                processConfig.Settings.Services.ThrowIfNull("processConfig.Settings.Services", processConfigId);
                CreateAndAddRuntimeServices(processConfig.Settings.Services);
            }
        }

        #endregion

        #region Local Variables/Properties

        Logger _logger = LoggerFactory.GetLogger();
        Guid _runtimeNodeId;
        Guid _runtimeNodeInstanceId;
        List<RuntimeServiceExecutor> _serviceExecutors;
        List<ChildRuntimeProcessProxy> _childRuntimeProcessProxies;
        Timer _timerRuntimeManager;        
        bool _isRuntimeManagerExecuting;
        object _runtimeManagerExecutingLockObj = new object();

        Timer _timerServicesManager;
        bool _isTimerServicesManagerExecuting;
        object _timerServicesManagerExecutingLockObj = new object();

        RuntimeNodeManager _runtimeNodeManager = new RuntimeNodeManager();
        RuntimeNodeConfigurationManager _runtimeNodeConfigurationManager = new RuntimeNodeConfigurationManager();

        int _numberOfEnabledProcesses;
        internal int NbOfEnabledProcesses
        {
            get
            {
                return _numberOfEnabledProcesses;
            }
        }

        List<int> _runningProcessIds = new List<int>();

        ConcurrentQueue<TransactionLockItem> _queueFreezedTransactionLocks = new ConcurrentQueue<TransactionLockItem>();
        List<TransactionLockItem> _FreezedTransactionLocks = new List<TransactionLockItem>();

        bool _isHostForChildRuntimes;

        #endregion

        #region Public Methods (called from Main Host process)

        public RuntimeStatus Status { get; private set; }

        public void Start()
        {
            if (this.Status != RuntimeStatus.NotStarted)
                throw new Exception(String.Format("Cannot Start a {0} RuntimeHost", this.Status));

            _logger.WriteInformation("Starting Host...");
            if (_serviceExecutors != null)
            {
                LoggerFactory.GetLogger().WriteInformation("Registering Runtime Host...");
                string currentProcessInterRuntimeServiceURL;
                ServiceHostManager.Current.CreateAndOpenTCPServiceHost(typeof(InterRuntimeWCFService), typeof(IInterRuntimeWCFService), OnServiceHostCreated, OnServiceHostRemoved, out currentProcessInterRuntimeServiceURL);

                List<RegisterRuntimeServiceInput> runtimeServicesRegistrationInfos = new List<RegisterRuntimeServiceInput>();
                Dictionary<Guid, RuntimeServiceExecutor> runtimeServiceExecutorsByInstanceId = new Dictionary<Guid, RuntimeServiceExecutor>();
                foreach (var serviceExecutor in _serviceExecutors)
                {
                    RuntimeServiceInitializeContext initializeContext = new RuntimeServiceInitializeContext();
                    serviceExecutor.Initialize(initializeContext);
                    Guid serviceInstanceId = Guid.NewGuid();
                    runtimeServicesRegistrationInfos.Add(new RegisterRuntimeServiceInput
                    {
                        ServiceInstanceId = serviceInstanceId,
                        ServiceTypeUniqueName = serviceExecutor.RuntimeService.ServiceTypeUniqueName,
                        ServiceInstanceInfo = initializeContext.ServiceInstanceInfo
                    });
                    runtimeServiceExecutorsByInstanceId.Add(serviceInstanceId, serviceExecutor);
                }
                RunningProcessAdditionalInfo additionalInfo = new RunningProcessAdditionalInfo
                {
                    TCPServiceURL = currentProcessInterRuntimeServiceURL
                };

                var parentNodeState = RuntimeDataManagerFactory.GetDataManager<IRuntimeNodeStateDataManager>().GetNodeState(_runtimeNodeId);
                parentNodeState.ThrowIfNull("parentNodeState", _runtimeNodeId);
                parentNodeState.ServiceURL.ThrowIfNull("parentNodeState.ServiceURL", _runtimeNodeId);
                var currentProcess = System.Diagnostics.Process.GetCurrentProcess();
                RunningProcessRegistrationOutput registrationOutput = null;
                ServiceClientFactory.CreateTCPServiceClient<IRuntimeManagerWCFService>(parentNodeState.ServiceURL,
                    (client) =>
                    {
                        var input = new RunningProcessRegistrationInput
                        {
                            OSProcessId = currentProcess.Id,
                            RuntimeNodeId = _runtimeNodeId,
                            RuntimeNodeInstanceId = _runtimeNodeInstanceId,
                            RuntimeServices = runtimeServicesRegistrationInfos,
                            AdditionalInfo = additionalInfo
                        };
                        string serializedRegistrationOutput = client.RegisterRunningProcess(Serializer.Serialize(input));
                        registrationOutput = Serializer.Deserialize<RunningProcessRegistrationOutput>(serializedRegistrationOutput);
                        registrationOutput.ThrowIfNull("registrationOutput");
                    });
                RunningProcessManager.CurrentProcess = registrationOutput.RunningProcessInfo;
                foreach (var runtimeServiceInstance in registrationOutput.RegisteredServices)
                {
                    RuntimeServiceExecutor serviceExecutor = runtimeServiceExecutorsByInstanceId.GetRecord(runtimeServiceInstance.ServiceInstanceId);
                    serviceExecutor.ThrowIfNull("serviceExecutor", runtimeServiceInstance.ServiceInstanceId);
                    serviceExecutor.RuntimeService.ServiceInstance = runtimeServiceInstance;
                    RuntimeServiceStartContext startContext = new RuntimeServiceStartContext();
                    serviceExecutor.Start(startContext);
                    serviceExecutor.Status = RuntimeStatus.Started;
                }

                LoggerFactory.GetLogger().WriteInformation("Runtime Host registered");
                System.Threading.Thread.Sleep(3000);//wait some time to ensure that all runtimes takes the latest version of running processes and runtime services            
                _timerServicesManager = new Timer(1000);
                _timerServicesManager.Elapsed += timerServicesManager_Elapsed;
                _timerServicesManager.Start();
            }
            if (_timerRuntimeManager != null)
                _timerRuntimeManager.Start();
            this.Status = RuntimeStatus.Started;
            _logger.WriteInformation("Host Started");
        }

        public void Stop()
        {
            if (this.Status != RuntimeStatus.Started)
                throw new Exception(String.Format("Cannot Stop a {0} RuntimeHost", this.Status));

            _logger.WriteInformation("Stopping Host...");
            if (_serviceExecutors != null)
            {
                _timerServicesManager.Stop();

                foreach (var serviceExecutor in _serviceExecutors)
                {
                    while (serviceExecutor.IsExecuting)
                        System.Threading.Thread.Sleep(1000);
                    serviceExecutor.Stop(null);
                    serviceExecutor.Status = RuntimeStatus.Stopped;
                    serviceExecutor.Dispose();
                }
            }
            this.Status = RuntimeStatus.Stopped;
            _logger.WriteInformation("Host Stopped");
        }

        #endregion
        
        #region Timers events

        void timerRuntimeManager_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (_runtimeManagerExecutingLockObj)
            {
                if (_isRuntimeManagerExecuting)
                    return;
                _isRuntimeManagerExecuting = true;
            }
            try
            {
                RuntimeManager.Current.Execute();
                if (_childRuntimeProcessProxies != null)
                {
                    foreach (var childRuntimeProcessProxy in _childRuntimeProcessProxies)
                    {
                        if (!childRuntimeProcessProxy.OSProcessId.HasValue)
                        {
                            StartChildProcess(childRuntimeProcessProxy);
                            System.Threading.Thread.Sleep(250);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerFactory.GetExceptionLogger().WriteException(ex);
            }
            lock (_runtimeManagerExecutingLockObj)
            {
                _isRuntimeManagerExecuting = false;
            }
        }

        void timerServicesManager_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (_timerServicesManagerExecutingLockObj)
            {
                if (_isTimerServicesManagerExecuting)
                    return;
                _isTimerServicesManagerExecuting = true;
            }
            try
            {
                if (_serviceExecutors != null && _serviceExecutors.Count > 0)
                {
                    foreach (var serviceExecutor in _serviceExecutors)
                        serviceExecutor.ExecuteIfIdleAndDue();
                }
                TransactionLockItem lockItem;
                while (_queueFreezedTransactionLocks.TryDequeue(out lockItem))
                {
                    _FreezedTransactionLocks.Add(lockItem);
                }
                if (_FreezedTransactionLocks.Count > 0)
                {
                    RuntimeManagerClient.CreateClient((client, primaryNodeRuntimeNodeInstanceId) =>
                    {
                        client.UnlockFreezedTransactions(_FreezedTransactionLocks);
                        _FreezedTransactionLocks.Clear();
                    });
                }
            }
            catch (Exception ex)
            {
                LoggerFactory.GetExceptionLogger().WriteException(ex);
            }
            lock (_timerServicesManagerExecutingLockObj)
            {
                _isTimerServicesManagerExecuting = false;
            }
        }

        #endregion

        #region Private Methods

        private Guid GetRuntimeNodeIdFromConfig()
        {
            string runtimeNodeIdString = ConfigurationManager.AppSettings["RuntimeNodeId"];
            if (runtimeNodeIdString == null)
                throw new Exception("RuntimeNodeId entry is not available in the AppSettings section");
            Guid runtimeNodeId;
            if (!Guid.TryParse(runtimeNodeIdString, out runtimeNodeId))
                throw new Exception(String.Format("RuntimeNodeId entry in the AppSettings section should be a valid Guid. wrong value '{0}'", runtimeNodeIdString));
            return runtimeNodeId;
        }

        private RuntimeNode GetRuntimeNodeWithValidate()
        {
            var runtimeNode = _runtimeNodeManager.GetNode(_runtimeNodeId);
            if (runtimeNode == null)
                throw new Exception(String.Format("Runtime Node not defined in the system database. Invalid RuntimeNodeId '{0}'", _runtimeNodeId));
            return runtimeNode;
        }

        private RuntimeNodeConfiguration GetRuntimeNodeConfigWithValidate()
        {
            var runtimeNode = GetRuntimeNodeWithValidate();
            var runtimeNodeConfiguration = _runtimeNodeConfigurationManager.GetNodeConfiguration(runtimeNode.RuntimeNodeConfigurationId);
            runtimeNodeConfiguration.ThrowIfNull("runtimeNodeConfiguration", runtimeNode.RuntimeNodeConfigurationId);
            runtimeNodeConfiguration.Settings.ThrowIfNull("runtimeNodeConfiguration.Settings", runtimeNode.RuntimeNodeConfigurationId);
            runtimeNodeConfiguration.Settings.Processes.ThrowIfNull("runtimeNodeConfiguration.Settings.Processes", runtimeNode.RuntimeNodeConfigurationId);
            return runtimeNodeConfiguration;
        }

        private void InitializeRuntimeManager(bool executeRuntimeManager)
        {
            RuntimeManager.Current = new RuntimeManager(this, _runtimeNodeId, _runtimeNodeInstanceId);
            if (executeRuntimeManager)
                RuntimeManager.Current.Execute();
            _timerRuntimeManager = new Timer(2000);
            _timerRuntimeManager.Elapsed += timerRuntimeManager_Elapsed;
        }

        private void CreateAndAddRuntimeServices(Dictionary<Guid, RuntimeServiceConfiguration> serviceConfigs)
        {
            _serviceExecutors = new List<RuntimeServiceExecutor>();
            foreach (var serviceConfigEntry in serviceConfigs)
            {
                Guid serviceConfigId = serviceConfigEntry.Key;
                serviceConfigEntry.Value.ThrowIfNull("serviceConfigEntry.Value", serviceConfigId);
                serviceConfigEntry.Value.Settings.ThrowIfNull("serviceConfigEntry.Value.Settings", serviceConfigId);
                serviceConfigEntry.Value.Settings.RuntimeService.ThrowIfNull("serviceConfigEntry.Value.Settings.RuntimeService", serviceConfigId);
                RuntimeServiceExecutor serviceExecutor = new RuntimeServiceExecutor(serviceConfigEntry.Value.Settings.RuntimeService);
                _serviceExecutors.Add(serviceExecutor);
            }
        }

        private void StartChildProcess(ChildRuntimeProcessProxy childRuntimeProcessProxy)
        {
            int currentProcessId = Process.GetCurrentProcess().Id;
            var processPath = System.Reflection.Assembly.GetEntryAssembly().Location;
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = processPath,
                Arguments = String.Format("ProcessId {0} {1} {2} {3}", currentProcessId, _runtimeNodeId.ToString(), _runtimeNodeInstanceId.ToString(), childRuntimeProcessProxy.ProcessConfigId)
            };
            var childProcess = Process.Start(startInfo);
            lock (_childRuntimeProcessProxies)
            {
                childRuntimeProcessProxy.OSProcessId = childProcess.Id;
            }
            childProcess.EnableRaisingEvents = true;
            childProcess.Exited += (sender, e) =>
            {
                lock (_childRuntimeProcessProxies)
                {
                    if (childRuntimeProcessProxy.ProcessId.HasValue)
                    {
                        RuntimeManager.Current.OnChildProcessExited(childRuntimeProcessProxy.ProcessId.Value);
                        _runningProcessIds.Remove(childRuntimeProcessProxy.ProcessId.Value);
                    }
                    childRuntimeProcessProxy.ProcessId = null;
                    childRuntimeProcessProxy.OSProcessId = null;
                }
            };
        }

        #endregion

        #region Calls received from the RuntimeManager

        internal bool IsHostForChildRuntimes()
        {
            return _isHostForChildRuntimes;
        }

        internal bool TrySetProcessIdOfChildProcess(int osProcessId, int processId)
        {
            if (_childRuntimeProcessProxies != null)
            {
                lock (_childRuntimeProcessProxies)
                {
                    var childProcessProxy = _childRuntimeProcessProxies.FindRecord(itm => itm.OSProcessId == osProcessId);
                    if (childProcessProxy != null)
                    {
                        if (childProcessProxy.OSProcessId == osProcessId)//another runtime process might have been started with a new OSProcessId
                        {
                            childProcessProxy.ProcessId = processId;
                            _runningProcessIds.Add(processId);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        internal List<int> GetAllRunningProcessIds()
        {
            List<int> runningProcessIds;
            if (_childRuntimeProcessProxies != null)
            {
                lock (_childRuntimeProcessProxies)
                {
                    runningProcessIds = new List<int>(_runningProcessIds);
                }
            }
            else
            {
                runningProcessIds = new List<int>();
            }
            return runningProcessIds;
        }

        internal void KillAllChildProcesses()
        {
            if (_childRuntimeProcessProxies != null)
            {
                lock (_childRuntimeProcessProxies)
                {
                    foreach (var childRuntimeProcessProxy in _childRuntimeProcessProxies)
                    {
                        if (childRuntimeProcessProxy.OSProcessId.HasValue)
                            Process.GetProcessById(childRuntimeProcessProxy.OSProcessId.Value).Kill();
                    }
                }
            }
        }

        internal void KillChildProcess(int processId)
        {
            if (_childRuntimeProcessProxies != null)
            {
                lock (_childRuntimeProcessProxies)
                {
                    foreach (var childRuntimeProcessProxy in _childRuntimeProcessProxies)
                    {
                        if (childRuntimeProcessProxy.ProcessId.HasValue && childRuntimeProcessProxy.ProcessId.Value == processId)
                        {
                            Process.GetProcessById(childRuntimeProcessProxy.OSProcessId.Value).Kill();
                            break;
                        }
                    }
                }
            }
        }

        #endregion

        #region Calls received from the TransactionLocker

        internal void SetTransactionLockFreezed(TransactionLockItem transactionLockItem)
        {
            _queueFreezedTransactionLocks.Enqueue(transactionLockItem);
        }

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

        #region Private Classes

        private class ChildRuntimeProcessProxy
        {
            public Guid ProcessConfigId { get; set; }

            public int? OSProcessId { get; set; }

            public int? ProcessId { get; set; }
        }

        #endregion
    }
}
