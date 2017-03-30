using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Entities;

namespace Vanrise.Notification.Business
{
    public class VRAlertRuleViewSettings : ViewSettings
    {
        public override bool DoesUserHaveAccess(IViewUserAccessContext context)
        {
            return new VRAlertRuleTypeManager().DoesUserHaveViewAccess();
        }
    }
}
