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

        internal protected virtual void OnStarted(IRuntimeServiceStartContext context)
        {
            _logger.WriteInformation("{0} Runtime Service Started", this.GetType().Name);
        }

        internal protected virtual void OnStopped(IRuntimeServiceStopContext context)
        {
            _logger.WriteInformation("{0} Runtime Service Stopped", this.GetType().Name);
        }

        public void Dispose()
        {
            OnDisposed();
        }

        protected virtual void OnDisposed()
        {

        }
    }

    public interface IRuntimeServiceStartContext
    {
        ServiceInstanceInfo ServiceInstanceInfo { set; }
    }

    internal class RuntimeServiceStartContext : IRuntimeServiceStartContext
    {
        public ServiceInstanceInfo ServiceInstanceInfo
        {
            set;
            get;
        }
    }


    public interface IRuntimeServiceStopContext
    {

    }
}
