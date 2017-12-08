using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Runtime.Entities;

namespace Vanrise.Runtime
{
    public abstract class RuntimeService : IDisposable
    {
        Logger _logger = LoggerFactory.GetLogger();

        public TimeSpan Interval { get; set; }

        public RuntimeStatus Status { get; internal set; }

        public virtual string ServiceTypeUniqueName
        {
            get
            {
                return this.GetType().AssemblyQualifiedName;
            }
        }

        public RuntimeServiceInstance ServiceInstance
        {
            get;
            internal set;
        }

        public bool IsExecuting { get; private set; }
        
        
        DateTime _lastExecutedTime;

        internal void ExecuteIfIdleAndDue()
        {            
            lock(this)
            {
                if (IsExecuting || (DateTime.Now - _lastExecutedTime) < this.Interval)
                    return;
                this.IsExecuting = true;
                _lastExecutedTime = DateTime.Now;
            }
            Task task = new Task(() =>
            {
                try
                {
                    _logger.WriteVerbose("Executing {0} Runtime Service...", this.GetType().Name);
                    DateTime start = DateTime.Now;
                    Execute();
                    _logger.WriteVerbose("{0} Runtime Service executed in {1}", this.GetType().Name, (DateTime.Now - start));
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

        protected abstract void Execute();

        internal void Initialize(IRuntimeServiceInitializeContext context)
        {
            OnInitialized(context);
        }

        protected virtual void OnInitialized(IRuntimeServiceInitializeContext context)
        {

        }

        internal void Start(IRuntimeServiceStartContext context)
        {
            OnStarted(context);
            _logger.WriteInformation("{0} Runtime Service Started", this.GetType().Name);
        }

        protected virtual void OnStarted(IRuntimeServiceStartContext context)
        {            
        }

        internal void Stop(IRuntimeServiceStopContext context)
        {
            OnStopped(context);
            _logger.WriteInformation("{0} Runtime Service Stopped", this.GetType().Name);
        }

        internal protected virtual void OnStopped(IRuntimeServiceStopContext context)
        {
        }

        public void Dispose()
        {
            OnDisposed();
        }

        protected virtual void OnDisposed()
        {

        }
    }

    public interface IRuntimeServiceInitializeContext
    {
        ServiceInstanceInfo ServiceInstanceInfo { set; }
    }

    public interface IRuntimeServiceStartContext
    {
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

    public interface IRuntimeServiceStopContext
    {

    }
}
