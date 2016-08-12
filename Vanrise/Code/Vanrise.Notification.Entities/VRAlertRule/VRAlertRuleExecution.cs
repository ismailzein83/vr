using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Notification.Entities
{
    public class VRAlertRuleExecution
    {
        public long AlertRuleId { get; set; }

        public string EventKey { get; set; }

        public VRAlertRuleExecutionData ExecutionData { get; set; }
    }

    public class VRAlertRuleExecutionData
    {
        public Object EventPayload { get; set; }

        public List<VRAction> RollbackActions { get; set; }
    }
}
