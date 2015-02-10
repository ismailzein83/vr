using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.Runtime
{
    public abstract class RuntimeService
    {
        public TimeSpan Interval { get; set; }

        private bool IsExecuting { get; set; }

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
                    Execute();
                }
                catch (Exception ex)
                {
                    LoggerFactory.GetLogger().LogException(ex);
                }
                lock (this)
                {
                    this.IsExecuting = false;
                }
            });
            task.Start();
        }

        protected abstract void Execute();
    }
}
