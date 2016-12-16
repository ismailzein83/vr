using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vanrise.Notification.Entities
{
    public class VRBalanceActiveAlertThreshold
    {
        public long? AlertRuleId { get; set; }
        public decimal? Threshold { get; set; }
    }
}
