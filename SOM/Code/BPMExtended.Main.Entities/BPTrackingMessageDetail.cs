using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Entities
{
    public class BPTrackingMessageDetail
    {
        public long Id { get; set; }
        public long ProcessInstanceId { get; set; }
        public long? ParentProcessId { get; set; }
       // public Vanrise.Entities.LogEntryType Severity { get; set; }
        public string TrackingMessage { get; set; }
        public string ExceptionDetail { get; set; }
        public DateTime EventTime { get; set; }
    }

}
