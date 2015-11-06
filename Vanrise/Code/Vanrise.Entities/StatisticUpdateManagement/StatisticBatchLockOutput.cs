using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public enum StatisticBatchLockResult { NewBatch, ExistingBatch, BatchInfoCorrupted }
    public class StatisticBatchLockOutput
    {
        public StatisticBatchLockResult Result { get; set; }

        public long BatchId { get; set; }

        public StatisticBatchInfo BatchInfo { get; set; }
    }

    public class StatisticBatchInfo
    {
        public Dictionary<string, long> ItemsIdsByKey { get; set; }
    }
}