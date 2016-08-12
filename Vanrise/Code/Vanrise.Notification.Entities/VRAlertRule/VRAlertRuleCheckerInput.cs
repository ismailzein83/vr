using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Queueing;

namespace Vanrise.Notification.Entities
{
    public class VRAlertRuleCheckerInput
    {
        public Guid RuleTypeId { get; set; }

        public BaseQueue<VRAlertRuleMatchedEvent> MatchedEventsOutputQueue { get; set; }
    }
}
