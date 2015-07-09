using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Queueing.Entities
{
    public class QueueGroup
    {
        public Dictionary<string, QueueGroupItem> ChildItems { get; set; }
    }

    public class QueueGroupItem
    {
        public string QueueName { get; set; }

        public IPersistentQueue Queue { get; set; }

        public Dictionary <string, QueueGroupItem> ChildItems { get; set; }
    }

    public class QueueGroupType
    {
        public QueueGroupType()
        {            
            ChildItems = new Dictionary<string, QueueGroupTypeItem>();
        }
        public Dictionary<string, QueueGroupTypeItem> ChildItems { get; set; }
    }

    public class QueueGroupTypeItem
    {
        public QueueGroupTypeItem(string queueItemFQTN)
        {
            this.QueueItemFQTN = queueItemFQTN;
            ChildItems = new Dictionary<string, QueueGroupTypeItem>();
        }

        public string QueueItemFQTN { get; set; }

        public Dictionary<string,QueueGroupTypeItem> ChildItems { get; set; }
    }
}
