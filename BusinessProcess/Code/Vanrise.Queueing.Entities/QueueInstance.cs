using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Queueing.Entities
{
    public enum QueueInstanceStatus { New = 0, ReadyToUse = 10, Deleted = 20 }
    public class QueueInstance
    {
        public int QueueInstanceId { get; set; }

        public string Name { get; set; }

        public int? ExecutionFlowId { get; set; }

        public string StageName { get; set; }

        public string Title { get; set; }

        public QueueInstanceStatus Status { get; set; }

        public int ItemTypeId { get; set; }

        public string ItemFQTN { get; set; }

        public QueueSettings Settings { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
