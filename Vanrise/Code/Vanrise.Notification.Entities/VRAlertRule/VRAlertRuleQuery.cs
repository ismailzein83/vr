using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Notification.Entities
{
    public class VRAlertRuleQuery
    {
        public string Name { get; set; }
        public List<Guid> RuleTypeIds { get; set; }
        public List<VRAlertRuleStatus> Statuses { get; set; }
    }
    public enum VRAlertRuleStatus { Enabled = 0, Disabled = 1 }
}
 