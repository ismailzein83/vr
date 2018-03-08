using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Runtime.Entities;

namespace Vanrise.Runtime
{

    internal class RuntimeServiceExecutor : IDisposable
    {
        Logger _logger = LoggerFactory.GetLogger();

        public RuntimeServiceExecutor(RuntimeService runtimeService)
        {
            this.RuntimeService = runtimeService;
        }

        public RuntimeService RuntimeService { get; private set; }

        public RuntimeStatus Status { get; internal set; }

        public bool IsExecuting { get; private set; }


        DateTime _lastExecutedTime;

        public void ExecuteIfIdleAndDue()
        {
            lock (this)
            {
                if (IsExecuting || (DateTime.Now - _lastExecutedTime) < this.RuntimeService.Interval)
                    return;
                this.IsExecuting = true;
                _lastExecutedTime = DateTime.Now;
            }
            Task task = new Task(() =>
            {
                try
                {
                    _logger.WriteVerbose("Executing {0} Runtime Service...", this.RuntimeService.GetType().Name);
                    DateTime start = DateTime.Now;
                    this.RuntimeService.Execute();
                    _logger.WriteVerbose("{0} Runtime Service executed in {1}", this.RuntimeService.GetType().Name, (DateTime.Now - start));
                }
                catch (Exception ex)
                {
                    LoggerFactory.GetExceptionLogger().WriteException(ex);
                }
                lock (this)
                {
                    this.IsExecuting = false;
                }
            });
            task.Start();
        }

        internal void Initialize(IRuntimeServiceInitializeContext context)
        {
            this.RuntimeService.OnInitialized(context);
        }

        internal void Start(IRuntimeServiceStartContext context)
        {
            this.RuntimeService.OnStarted(context);
            _logger.WriteInformation("{0} Runtime Service Started", this.RuntimeService.GetType().Name);
        }

        internal void Stop(IRuntimeServiceStopContext context)
        {
            this.RuntimeService.OnStopped(context);
            _logger.WriteInformation("{0} Runtime Service Stopped", this.RuntimeService.GetType().Name);
        }

        public void Dispose()
        {
            this.RuntimeService.Dispose();
        }
    }
    

    internal class RuntimeServiceInitializeContext : IRuntimeServiceInitializeContext
    {
        public ServiceInstanceInfo ServiceInstanceInfo
        {
            set;
            get;
        }
    }

    internal class RuntimeServiceStartContext : IRuntimeServiceStartContext
    {
    }
}
