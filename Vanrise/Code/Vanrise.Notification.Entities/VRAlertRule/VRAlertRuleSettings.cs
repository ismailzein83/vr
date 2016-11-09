using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.Notification.Entities
{
    public class VRAlertRuleSettings
    {
        public VRAlertRuleExtendedSettings ExtendedSettings { get; set; }

        public Guid RuleTypeId { get; set; }

        public VRAlertRuleCriteria Criteria { get; set; }

        public List<VRAction> Actions { get; set; }

        public List<VRAction> RollbackActions { get; set; }
    }

    public abstract class VRAlertRuleExtendedSettings
    {
    }

    public abstract class VRGenericAlertRuleExtendedSettings : VRAlertRuleExtendedSettings
    {
        public GenericRuleCriteria Criteria { get; set; }
    }
}
