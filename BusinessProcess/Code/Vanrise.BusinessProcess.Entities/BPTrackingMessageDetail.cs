using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vanrise.Common;

namespace Vanrise.BusinessProcess.Entities
{
    public class BPTrackingMessageDetail
    {
     public BPTrackingMessage Entity { get; set; }
     public string SeverityDescription { get { if (this.Entity != null) return this.Entity.Severity.ToString(); return null; } }
    }
}
