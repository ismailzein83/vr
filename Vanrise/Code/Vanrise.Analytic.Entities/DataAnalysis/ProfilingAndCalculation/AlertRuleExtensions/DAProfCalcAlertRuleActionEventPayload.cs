using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Notification.Entities;

namespace Vanrise.Analytic.Entities
{
    public class DAProfCalcAlertRuleActionEventPayload : IVRActionEventPayload
    {
        public Guid AlertRuleTypeId { get; set; }

        public long AlertRuleId { get; set; }

        public string GroupingKey { get; set; }
    }
}
