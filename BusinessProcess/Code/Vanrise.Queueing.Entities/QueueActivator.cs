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

        public virtual void ProcessItem(IQueueActivatorExecutionContext context)
        {

        }

        public void Dispose()
        {
            OnDisposed();
        }

        public abstract void OnDisposed();
    }

    public interface IQueueActivatorExecutionContext
    {
        PersistentQueueItem ItemToProcess { get; }

        ItemsToEnqueue OutputItems { get; }

        QueueExecutionFlowStage CurrentStage { get; }

        QueueInstance Queue { get; }
    }
}
