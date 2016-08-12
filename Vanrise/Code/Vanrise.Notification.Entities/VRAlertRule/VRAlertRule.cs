using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Notification.Entities
{
    public class VRAlertRule
    {
        public long VRAlertRuleId { get; set; }

        public string Name { get; set; }

        public Guid RuleTypeId { get; set; }

        public VRAlertRuleSettings Settings { get; set; }
    }

    public class VRAlertRule<T> : VRAlertRule where T : VRAlertRuleCriteria
    {
        public T RuleCriteria { get; set; }
    }
}
