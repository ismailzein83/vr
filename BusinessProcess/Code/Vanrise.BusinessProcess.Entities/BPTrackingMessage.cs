﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vanrise.BusinessProcess.Entities
{
    public enum BPTrackingSeverity { Error = 10, Warning = 20, Information = 30, Debug = 40 }
    public class BPTrackingMessage
    {
        public long ProcessInstanceId { get; set; }
        public long? ParentProcessId { get; set; }
        public BPTrackingSeverity Severity { get; set; }
        public string Message { get; set; }
        public DateTime EventTime { get; set; }
    }
}
