using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Queueing.Entities
{
    public class ItemToEnqueue
    {
        public ItemToEnqueue(string stageName, PersistentQueueItem item)
        {
            this.StageName = stageName;
            this.Item = item;
        }
        public string StageName { get; set; }

        public PersistentQueueItem Item { get; set; }
    }

    public class ItemsToEnqueue : List<ItemToEnqueue>
    {
        public void Add(string stageName, PersistentQueueItem item)
        {
            Add(new ItemToEnqueue(stageName, item));
        }
    }
}
