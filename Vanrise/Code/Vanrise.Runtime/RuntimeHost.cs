using System;
using System.Collections.Generic;
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
            _timer = new Timer(1000);
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
