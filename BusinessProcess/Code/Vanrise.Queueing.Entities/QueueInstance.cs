using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Queueing.Entities
{
    public enum QueueInstanceStatus {
         [Description("New")]
        New = 0,
        [Description("Ready To Use")]
        ReadyToUse = 10,
        [Description("Deleted")]
        Deleted = 20 }
    public class QueueInstance
    {
        public int QueueInstanceId { get; set; }

        public string Name { get; set; }

        public Guid? ExecutionFlowId { get; set; }

        public string StageName { get; set; }

        public string Title { get; set; }

        public QueueInstanceStatus Status { get; set; }

        public int ItemTypeId { get; set; }

        public string ItemFQTN { get; set; }

        public QueueSettings Settings { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
