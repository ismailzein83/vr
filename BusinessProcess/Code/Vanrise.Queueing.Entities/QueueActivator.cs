using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Queueing.Entities
{
    public abstract class QueueActivator : IDisposable
    {
        public abstract void Run(QueueInstance queueInstance);

        public virtual void Run(QueueInstance queueInstance, IPersistentQueue queue, Dictionary<string, IPersistentQueue> nextQueues)
        {

        }

        public void Dispose()
        {
            OnDisposed();
        }

        public abstract void OnDisposed();
    }
}
