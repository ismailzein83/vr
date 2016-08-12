using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Notification.Entities
{
    public abstract class VRAlertRuleSettings
    {
        public Guid RuleTypeId { get; set; }

        public VRAlertRuleCriteria Criteria { get; set; }

        public List<VRAction> Actions { get; set; }

        public List<VRAction> RollbackActions { get; set; }
    }
}
