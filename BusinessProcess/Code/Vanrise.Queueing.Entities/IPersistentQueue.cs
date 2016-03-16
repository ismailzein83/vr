﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Queueing.Entities
{
    public interface IPersistentQueue
    {
        QueueSettings QueueSettings { get; }

        long EnqueueObject(PersistentQueueItem item);

        bool TryDequeueObject(Action<PersistentQueueItem> processItem);

        List<DateTime> GetAvailableBatchStarts();

        bool TryDequeueSummaryBatches(DateTime batchStart, Action<IEnumerable<PersistentQueueItem>> processBatches);
    }
}
