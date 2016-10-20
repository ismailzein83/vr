using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Queueing.Entities;

namespace Vanrise.Integration.Entities
{
    public abstract class MappedBatchItem : PersistentQueueItem
    {
        public abstract int GetRecordCount();
        public Guid DataSourceID { get; set; }
    }

    public class MappedBatchItemToEnqueue
    {
        public MappedBatchItemToEnqueue(string stageName, MappedBatchItem item)
        {
            this.StageName = stageName;
            this.Item = item;
        }
        public string StageName { get; set; }

        public MappedBatchItem Item { get; set; }
    }

    public class MappedBatchItemsToEnqueue : List<MappedBatchItemToEnqueue>
    {
        public void Add(string stageName, MappedBatchItem item)
        {
            Add(new MappedBatchItemToEnqueue(stageName, item));
        }
    }
}
