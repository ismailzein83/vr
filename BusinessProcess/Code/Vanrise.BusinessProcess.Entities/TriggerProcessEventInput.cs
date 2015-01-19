using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vanrise.BusinessProcess.Entities
{
    public class TriggerProcessEventInput
    {
        public long ProcessInstanceId { get; set; }
        public string BookmarkName { get; set; }
        public object EventData { get; set; }
    }
}
