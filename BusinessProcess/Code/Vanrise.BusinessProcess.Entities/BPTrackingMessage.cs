using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vanrise.BusinessProcess.Entities
{
    public enum BPTrackingSeverity { Error = 10, Warning = 20, Information = 30, Verbose = 40 }
    public class BPTrackingMessage
    {
        public long Id { get; set; }
        public long ProcessInstanceId { get; set; }
        public long? ParentProcessId { get; set; }
        public BPTrackingSeverity Severity { get; set; }
        public string TrackingMessage { get; set; }
        public DateTime EventTime { get; set; }
    }
}
