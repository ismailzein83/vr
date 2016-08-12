using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Notification.Entities
{
    public class VRAlertRuleMatchedEvent
    {
        public long AlertRuleId { get; set; }

        public VRAlertRuleEventPayload EventPayload { get; set; }
    }
}
