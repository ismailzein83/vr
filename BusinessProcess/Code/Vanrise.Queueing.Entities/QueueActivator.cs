using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Queueing.Entities
{
    public abstract class QueueActivator : IDisposable
    {
        public abstract void ProcessItem(PersistentQueueItem item, ItemsToEnqueue outputItems);

        public void Dispose()
        {
            OnDisposed();
        }

        public abstract void OnDisposed();
    }
}
