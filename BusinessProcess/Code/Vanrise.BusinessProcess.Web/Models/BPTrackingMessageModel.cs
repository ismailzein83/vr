using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;

namespace Vanrise.BusinessProcess.Web.Models
{
    public class BPTrackingMessageModel
    {
        public long TrackingId { get; set; }
        public long ProcessInstanceId { get; set; }
        public long? ParentProcessId { get; set; }
        public LogEntryType Severity { get; set; }
        public string SeverityDescription { get; set; }
        public string Message { get; set; }
        public DateTime EventTime { get; set; }
    }
}