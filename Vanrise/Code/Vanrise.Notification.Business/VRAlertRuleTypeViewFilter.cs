using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Notification.Entities;
using Vanrise.Security.Entities;

namespace Vanrise.Notification.Business
{  
    public class VRAlertRuleTypeViewFilter : IVRAlertRuleTypeFilter
    {
        public bool IsMatch(VRAlertRuleType AlertRuleType)
        {
            VRAlertRuleTypeManager ruleTypeManager = new VRAlertRuleTypeManager();
            int userId = ContextFactory.GetContext().GetLoggedInUserId();
            return ruleTypeManager.DoesUserHaveViewAccess(userId, AlertRuleType);
        }
    }
}
