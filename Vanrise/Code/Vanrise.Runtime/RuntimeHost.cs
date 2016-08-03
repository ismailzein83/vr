using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Vanrise.Common;

namespace Vanrise.Runtime
{
    public enum RuntimeStatus { NotStarted, Started, Stopped }
    public class RuntimeHost
    {
        public RuntimeHost(List<RuntimeService> services)
        {
            _services = services;
            TimeSpan timerInterval = TimeSpan.FromSeconds(1);
            if (services != null && services.Count > 0)
            {
                if (services.Min(itm => itm.Interval) < timerInterval)
                    timerInterval = services.Min(itm => itm.Interval);
            }
            _timer = new Timer(timerInterval.TotalMilliseconds);
            _timer.Elapsed += timer_Elapsed;
            try
            {
                var curentProcess = RunningProcessManager.CurrentProcess;//this is only to trigger the Heartbeat process
            }
            catch(Exception ex)
            {
                LoggerFactory.GetExceptionLogger().WriteException(ex);
            }
        }

        public RuntimeHost(string[] applicationArgs) : 
            this(CreateServicesFromArgs(applicationArgs))
        {
        }

        private static List<RuntimeService> CreateServicesFromArgs(string[] applicationArgs)
        {
            List<RuntimeService> runtimeServices = new List<RuntimeService>();
            var runtimeConfig = Configuration.RuntimeConfig.GetConfig();
            if (runtimeConfig == null)
                throw new Exception("Runtime Config Section is not found in the config file");
            int parentProcessId;
            if (applicationArgs != null && applicationArgs.Length > 0)
            {
                parentProcessId = int.Parse(applicationArgs[1]);
                Console.WriteLine("Parent Process Id: {0}", parentProcessId);
                var parentProcess = Process.GetProcessById(parentProcessId);
                parentProcess.EnableRaisingEvents = true;
                parentProcess.Exited += (sender, e) =>
                    {                        
                        Process.GetCurrentProcess().Kill();
                    };

                var runtimeServiceGroupName = applicationArgs[2];
                var runtimeServiceGroupConfig = runtimeConfig.RuntimeServiceGroups[runtimeServiceGroupName];
                CreateAndAddRuntimeServices(runtimeServices, runtimeServiceGroupConfig);
            }
            else
            {
                parentProcessId = Process.GetCurrentProcess().Id;
                var processPath = System.Reflection.Assembly.GetEntryAssembly().Location;

                foreach(Configuration.RuntimeServiceGroup runtimeServiceGroupConfig in runtimeConfig.RuntimeServiceGroups)
                {
                    for (int i = 0; i < runtimeServiceGroupConfig.NbOfRuntimeInstances; i++)
                    {
                        StartChildProcess(parentProcessId, processPath, runtimeServiceGroupConfig.Name);
                    }
                }
            }
            return runtimeServices;
        }

        private static void CreateAndAddRuntimeServices(List<RuntimeService> runtimeServices, Configuration.RuntimeServiceGroup runtimeServiceGroupConfig)
        {
            foreach (Configuration.RuntimeService runtimeServiceConfig in runtimeServiceGroupConfig.RuntimeServices)
            {
                RuntimeService service = CreateRuntimeService(runtimeServiceConfig);
                runtimeServices.Add(service);
            }
        }

        private static RuntimeService CreateRuntimeService(Configuration.RuntimeService serviceConfig)
        {
            Type type = Type.GetType(serviceConfig.Type);
            var service = Activator.CreateInstance(type) as RuntimeService;
            service.Interval = serviceConfig.Interval;
            return service;
        }

        private static void StartChildProcess(int parentProcessId, string processPath, string serviceGroupName)
        {
            
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = processPath,
                Arguments = String.Format("ProcessId {0} {1}", parentProcessId, serviceGroupName)
            };
            var childProcess = Process.Start(startInfo);
            childProcess.EnableRaisingEvents = true;
            childProcess.Exited += (sender, e) =>
            {
                StartChildProcess(parentProcessId, processPath, serviceGroupName);
            };
        }

        Logger _logger = LoggerFactory.GetLogger();
        List<RuntimeService> _services;
        Timer _timer;

        public RuntimeStatus Status { get; private set; }

        public void Start()
        {
            if (this.Status != RuntimeStatus.NotStarted)
                throw new Exception(String.Format("Cannot Start a {0} RuntimeHost", this.Status));

            _logger.WriteInformation("Starting Host...");
            foreach (var service in _services)
            {
                service.OnStarted();
                service.Status = RuntimeStatus.Started;
            }
            _timer.Start();
            this.Status = RuntimeStatus.Started;
            _logger.WriteInformation("Host Started");
        }

        public void Stop()
        {
            if (this.Status != RuntimeStatus.Started)
                throw new Exception(String.Format("Cannot Stop a {0} RuntimeHost", this.Status));

            _logger.WriteInformation("Stopping Host...");
            _timer.Stop();

            foreach (var service in _services)
            {
                while (service.IsExecuting)
                    System.Threading.Thread.Sleep(1000);
                service.OnStopped();
                service.Status = RuntimeStatus.Stopped;
                service.Dispose();
            }
            this.Status = RuntimeStatus.Stopped;
            _logger.WriteInformation("Host Stopped");
        }


        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            foreach (var service in _services)
                service.ExecuteIfIdleAndDue();
        }
    }
}
