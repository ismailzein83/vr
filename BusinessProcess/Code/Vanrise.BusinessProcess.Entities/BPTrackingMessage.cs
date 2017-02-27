using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vanrise.Common;
using Vanrise.Entities;

namespace Vanrise.BusinessProcess.Entities
{
    public class BPTrackingMessage
    {
        public long Id { get; set; }
        public long ProcessInstanceId { get; set; }
        public long? ParentProcessId { get; set; }
        public LogEntryType  Severity { get; set; }
        public string TrackingMessage { get; set; }

        public string ExceptionDetail { get; set; }

        public DateTime EventTime { get; set; }
    }
}
