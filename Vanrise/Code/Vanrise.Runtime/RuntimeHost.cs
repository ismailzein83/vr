using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
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
    public enum RuntimeStatus { NotStarted, Started, Stopped }
    public class RuntimeHost
    {
        Logger _logger = LoggerFactory.GetLogger();
        List<RuntimeServiceExecutor> _serviceExecutors;
        Timer _timerServicesManager;
        Timer _timerRuntimeManager;
        List<ChildRuntimeProcessProxy> _childRuntimeProcessProxies;

        public RuntimeHost(List<RuntimeService> services)
            : this()
        {
            InitializeRuntimeManager(true);
            _serviceExecutors = services != null ? services.Select(s => new RuntimeServiceExecutor(s)).ToList() : null;
        }

        public RuntimeHost(string[] applicationArgs)
            : this()
        {
            var runtimeConfig = Configuration.RuntimeConfig.GetConfig();
            if (runtimeConfig == null)
                throw new Exception("Runtime Config Section is not found in the config file");
            int parentProcessId;
            if (applicationArgs != null && applicationArgs.Length > 0)
            {
                List<RuntimeServiceExecutor> runtimeServiceExecutors = new List<RuntimeServiceExecutor>();

                parentProcessId = int.Parse(applicationArgs[1]);
                Console.WriteLine("Parent Process Id: {0}", parentProcessId);
                var parentProcess = Process.GetProcessById(parentProcessId);
                parentProcess.EnableRaisingEvents = true;
                parentProcess.Exited += (sender, e) =>
                {
                    Environment.Exit(0);
                };

                var runtimeServiceGroupName = applicationArgs[2];
                var runtimeServiceGroupConfig = runtimeConfig.RuntimeServiceGroups[runtimeServiceGroupName];
                CreateAndAddRuntimeServices(runtimeServiceExecutors, runtimeServiceGroupConfig);
                _serviceExecutors = runtimeServiceExecutors;
            }
            else
            {
                _childRuntimeProcessProxies = new List<ChildRuntimeProcessProxy>();
                foreach (Configuration.RuntimeServiceGroup runtimeServiceGroupConfig in runtimeConfig.RuntimeServiceGroups)
                {
                    for (int i = 0; i < runtimeServiceGroupConfig.NbOfRuntimeInstances; i++)
                    {
                        _childRuntimeProcessProxies.Add(new ChildRuntimeProcessProxy
                            {
                                 ServiceGroupName = runtimeServiceGroupConfig.Name
                            });
                    }
                }
                InitializeRuntimeManager(false);
            }
        }

        private RuntimeHost()
        {
            Vanrise.Common.Utilities.CompilePredefinedPropValueReaders();                
        }


        private void InitializeRuntimeManager(bool executeRuntimeManager)
        {
            RuntimeManager.Current = new RuntimeManager();
            if (executeRuntimeManager)
                RuntimeManager.Current.Execute(false);
            _timerRuntimeManager = new Timer(2000);
            _timerRuntimeManager.Elapsed += timerRuntimeManager_Elapsed;
            _timerRuntimeManager.Start();
        }

        bool _runtimeManagerFailed;
        void timerRuntimeManager_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (s_runtimeManagerExecutingLockObj)
            {
                if (s_isRuntimeManagerExecuting)
                    return;
                s_isRuntimeManagerExecuting = true;
            }
            try
            {
                RuntimeManager.Current.Execute(_runtimeManagerFailed);
                _runtimeManagerFailed = false;
                if (_childRuntimeProcessProxies != null)
                {
                    foreach (var childRuntimeProcessProxy in _childRuntimeProcessProxies)
                    {
                        if (!childRuntimeProcessProxy.IsStarted)
                        {
                            StartChildProcess(childRuntimeProcessProxy);
                            System.Threading.Thread.Sleep(250);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _runtimeManagerFailed = true;
                LoggerFactory.GetExceptionLogger().WriteException(ex);
            }
            lock (s_runtimeManagerExecutingLockObj)
            {
                s_isRuntimeManagerExecuting = false;
            }
        }

        private void StartChildProcess(ChildRuntimeProcessProxy childRuntimeProcessProxy)
        {
            int currentProcessId = Process.GetCurrentProcess().Id;
            var processPath = System.Reflection.Assembly.GetEntryAssembly().Location;
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = processPath,
                Arguments = String.Format("ProcessId {0} {1}", currentProcessId, childRuntimeProcessProxy.ServiceGroupName)
            };
            var childProcess = Process.Start(startInfo);
            lock (childRuntimeProcessProxy)
            {
                childRuntimeProcessProxy.IsStarted = true;
            }
            childProcess.EnableRaisingEvents = true;
            childProcess.Exited += (sender, e) =>
            {
                lock (childRuntimeProcessProxy)
                {
                    childRuntimeProcessProxy.IsStarted = false;
                }
            };
        }

        private void CreateAndAddRuntimeServices(List<RuntimeServiceExecutor> runtimeServiceExecutors, Configuration.RuntimeServiceGroup runtimeServiceGroupConfig)
        {
            runtimeServiceExecutors.ThrowIfNull("runtimeServiceExecutors");
            runtimeServiceGroupConfig.ThrowIfNull("runtimeServiceGroupConfig");
            foreach (Configuration.RuntimeService runtimeServiceConfig in runtimeServiceGroupConfig.RuntimeServices)
            {
                RuntimeServiceExecutor serviceExecutor = CreateRuntimeServiceExecutor(runtimeServiceConfig);
                runtimeServiceExecutors.Add(serviceExecutor);
            }
        }

        private RuntimeServiceExecutor CreateRuntimeServiceExecutor(Configuration.RuntimeService serviceConfig)
        {
            Type type = Type.GetType(serviceConfig.Type);
            var service = Activator.CreateInstance(type).CastWithValidate<RuntimeService>("service", serviceConfig.Name);
            var serviceExecutor = new RuntimeServiceExecutor(service);
            serviceExecutor.RuntimeService.Interval = serviceConfig.Interval;
            return serviceExecutor;
        }

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
                foreach (var service in _serviceExecutors)
                {
                    RuntimeServiceInitializeContext initializeContext = new RuntimeServiceInitializeContext();
                    service.Initialize(initializeContext);
                    Guid serviceInstanceId = Guid.NewGuid();
                    runtimeServicesRegistrationInfos.Add(new RegisterRuntimeServiceInput
                        {
                            ServiceInstanceId = serviceInstanceId,
                            ServiceTypeUniqueName = service.RuntimeService.ServiceTypeUniqueName,
                            ServiceInstanceInfo = initializeContext.ServiceInstanceInfo
                        });
                    runtimeServiceExecutorsByInstanceId.Add(serviceInstanceId, service);
                }
                RunningProcessAdditionalInfo additionalInfo = new RunningProcessAdditionalInfo
                {
                    TCPServiceURL = currentProcessInterRuntimeServiceURL
                };
                
                RuntimeManagerClient.CreateClient((client) =>
                                           {
                                               var input = new RunningProcessRegistrationInput
                                               {
                                                   ProcessName = System.Diagnostics.Process.GetCurrentProcess().ProcessName,
                                                   MachineName = Environment.MachineName,
                                                   RuntimeServices = runtimeServicesRegistrationInfos,
                                                   AdditionalInfo = additionalInfo
                                               };
                                               string serializedRegistrationOutput = client.RegisterRunningProcess(Serializer.Serialize(input));
                                               var registrationOutput = Serializer.Deserialize<RunningProcessRegistrationOutput>(serializedRegistrationOutput);
                                               RunningProcessManager.CurrentProcess = registrationOutput.RunningProcessInfo;
                                               RunningProcessManager.LastReceivedPingTime = DateTime.Now;
                                               RunningProcessManager.SetRunningProcesses(registrationOutput.AllRunningProcesses);
                                               RuntimeServiceInstanceManager.SetRuntimeServices(registrationOutput.AllRunningServices);
                                               foreach (var runtimeServiceInstance in registrationOutput.RegisteredServices)
                                               {
                                                   RuntimeServiceExecutor serviceExecutor = runtimeServiceExecutorsByInstanceId.GetRecord(runtimeServiceInstance.ServiceInstanceId);
                                                   serviceExecutor.ThrowIfNull("serviceExecutor", runtimeServiceInstance.ServiceInstanceId);
                                                   serviceExecutor.RuntimeService.ServiceInstance = runtimeServiceInstance;
                                                   RuntimeServiceStartContext startContext = new RuntimeServiceStartContext();
                                                   serviceExecutor.Start(startContext);
                                                   serviceExecutor.Status = RuntimeStatus.Started;
                                               }
                                           });

                LoggerFactory.GetLogger().WriteInformation("Runtime Host registered"); 
                System.Threading.Thread.Sleep(3000);//wait some time to ensure that all runtimes takes the latest version of running processes and runtime services            
                _timerServicesManager = new Timer(1000);
                _timerServicesManager.Elapsed += timerServicesManager_Elapsed;
                _timerServicesManager.Start();
            }   
            this.Status = RuntimeStatus.Started;
            _logger.WriteInformation("Host Started");
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


        static ConcurrentQueue<TransactionLockItem> s_queueFreezedTransactionLocks = new ConcurrentQueue<TransactionLockItem>();
        static List<TransactionLockItem> s_FreezedTransactionLocks = new List<TransactionLockItem>();       

        internal static void SetTransactionLockFreezed(TransactionLockItem transactionLockItem)
        {
            s_queueFreezedTransactionLocks.Enqueue(transactionLockItem);
        }


        static bool s_isRuntimeManagerExecuting;
        static object s_runtimeManagerExecutingLockObj = new object();

        static bool s_isTimerServicesManagerExecuting;
        static object s_timerServicesManagerExecutingLockObj = new object();

        void timerServicesManager_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (s_timerServicesManagerExecutingLockObj)
            {
                if (s_isTimerServicesManagerExecuting)
                    return;
                s_isTimerServicesManagerExecuting = true;
            }
            try
            {
                ExitCurrentProcessIfNotRegistered();
                if (_serviceExecutors != null && _serviceExecutors.Count > 0)
                {
                    foreach (var service in _serviceExecutors)
                        service.ExecuteIfIdleAndDue();
                }
                TransactionLockItem lockItem;
                while (s_queueFreezedTransactionLocks.TryDequeue(out lockItem))
                {
                    s_FreezedTransactionLocks.Add(lockItem);
                }
                if (s_FreezedTransactionLocks.Count > 0)
                {
                    RuntimeManagerClient.CreateClient((client) =>
                        {
                            client.UnlockFreezedTransactions(s_FreezedTransactionLocks);
                            s_FreezedTransactionLocks.Clear();
                        });
                }
            }
            catch(Exception ex)
            {
                LoggerFactory.GetExceptionLogger().WriteException(ex);
            }
            lock (s_timerServicesManagerExecutingLockObj)
            {
                s_isTimerServicesManagerExecuting = false;
            }
        }

        private void ExitCurrentProcessIfNotRegistered()
        {
            if (RunningProcessManager.IsCurrentProcessARuntime && (DateTime.Now - RunningProcessManager.LastReceivedPingTime) >= RuntimeManager.s_RunningProcessHeartBeatTimeout)
            {
                bool isRunningProcessStillRegistered = false;
                for (int i = 0; i < 3; i++)//this forloop to retry the call 3 times in case the call to RuntimeManager fails
                {
                    try
                    {
                        RuntimeManagerClient.CreateClient((client) =>
                        {
                            isRunningProcessStillRegistered = client.IsRunningProcessStillRegistered(RunningProcessManager.CurrentProcess.ProcessId);
                        });
                        break;
                    }
                    catch
                    {
                        isRunningProcessStillRegistered = false;
                        System.Threading.Thread.Sleep(1000);
                    }
                }
                if (isRunningProcessStillRegistered)
                    RunningProcessManager.LastReceivedPingTime = DateTime.Now;
                else
                    Environment.Exit(0);
            }
        }

        #region Private Classes

        private class ChildRuntimeProcessProxy
        {
            public string ServiceGroupName { get; set; }

            public bool IsStarted { get; set; }
        }

        #endregion
    }
}
