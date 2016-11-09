using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Notification.Entities
{
    public class VRBalanceAlertEventPayload : IVRActionEventPayload
    {
        public Guid AlertRuleTypeId { get; set; }

        public string EntityId { get; set; }

        public Decimal CurrentBalance { get; set; }

        public Decimal Threshold { get; set; }
    }
}
