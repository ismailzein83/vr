using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Runtime.Entities
{
    public abstract class RuntimeService : IDisposable
    {
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
            set;
        }

        public TimeSpan Interval { get; set; }

        public abstract void Execute();

        public virtual void OnInitialized(IRuntimeServiceInitializeContext context)
        {

        }

        public virtual void OnStarted(IRuntimeServiceStartContext context)
        {
        }

        public virtual void OnStopped(IRuntimeServiceStopContext context)
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

    public interface IRuntimeServiceStopContext
    {

    }
}
