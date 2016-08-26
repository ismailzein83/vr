using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vanrise.BusinessProcess.Entities
{
    public class BPEvent
    {
        public long BPEventID { get; set; }
        public long ProcessInstanceID { get; set; }
        public string Bookmark { get; set; }
        public object Payload { get; set; }
    }
}
