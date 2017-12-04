using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Queueing.Entities
{
    public interface ISummaryBatchQueueActivator
    {
        void UpdateNewBatches(DateTime batchStart, IEnumerable<PersistentQueueItem> newBatches, ref Object batchStartState);
    }
}
