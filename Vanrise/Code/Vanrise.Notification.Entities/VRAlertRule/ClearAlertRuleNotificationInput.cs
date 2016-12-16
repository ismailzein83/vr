using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Notification.Entities
{
    public class ClearAlertRuleNotificationInput
    {
        public int UserId { get; set; }
        public Guid RuleTypeId { get; set; }
        public long? AlertRuleId { get; set; }
        public string EventKey { get; set; }
    }
}
