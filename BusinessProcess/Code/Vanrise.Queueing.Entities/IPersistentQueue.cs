using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Queueing.Entities
{
    public interface IPersistentQueue
    {
        long EnqueueObject(PersistentQueueItem item);

        bool TryDequeueObject(Action<PersistentQueueItem> processItem);
    }
}
