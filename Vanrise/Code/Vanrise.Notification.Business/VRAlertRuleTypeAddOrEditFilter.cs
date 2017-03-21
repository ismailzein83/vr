using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Notification.Entities;
using Vanrise.Security.Entities;

namespace Vanrise.Notification.Business
{
    public class VRAlertRuleTypeAddOrEditFilter: IVRAlertRuleTypeFilter
    {
        public bool EditMode { get; set; }

        public bool IsMatch(VRAlertRuleType AlertRuleType)
        {
            VRAlertRuleTypeManager ruleTypeManager = new VRAlertRuleTypeManager();
            int userId = ContextFactory.GetContext().GetLoggedInUserId();
            bool matched = (this.EditMode) ? ruleTypeManager.DoesUserHaveAddAccess(userId, AlertRuleType.VRAlertRuleTypeId) || ruleTypeManager.DoesUserHaveEditAccess(userId, AlertRuleType.VRAlertRuleTypeId) : ruleTypeManager.DoesUserHaveAddAccess(userId, AlertRuleType.VRAlertRuleTypeId);
            return matched;
        }
    }
}
