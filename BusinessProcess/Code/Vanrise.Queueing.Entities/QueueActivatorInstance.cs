using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Queueing.Entities
{
    public enum QueueActivatorType { Normal = 0, Summary = 1}
    public class QueueActivatorInstance
    {
        public Guid ActivatorId { get; set; }

        public int ProcessId { get; set; }

        public QueueActivatorType ActivatorType { get; set; }

        public string ServiceURL { get; set; }
    }
}
