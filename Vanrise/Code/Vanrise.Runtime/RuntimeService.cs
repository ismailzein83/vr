using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.Runtime
{
    public abstract class RuntimeService : IDisposable
    {
        Logger _logger = LoggerFactory.GetLogger();

        public TimeSpan Interval { get; set; }

        public RuntimeStatus Status { get; internal set; }

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

        internal protected virtual void OnStarted()
        {
            _logger.WriteInformation("{0} Runtime Service Started", this.GetType().Name);
        }

        internal protected virtual void OnStopped()
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
}
