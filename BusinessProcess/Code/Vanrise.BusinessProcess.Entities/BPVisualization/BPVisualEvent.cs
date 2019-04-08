using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BusinessProcess.Entities
{
    public class BPVisualEvent
    {
        public long BPVisualEventId { get; set; }

        public long ProcessInstanceId { get; set; }

        public Guid ActivityId { get; set; }

        public string Title { get; set; }

        public Guid EventTypeId { get; set; }

        public BPVisualEventPayload EventPayload { get; set; }

        public DateTime CreatedTime { get; set; }
    }

    public abstract class BPVisualEventPayload
    {

    }
}
