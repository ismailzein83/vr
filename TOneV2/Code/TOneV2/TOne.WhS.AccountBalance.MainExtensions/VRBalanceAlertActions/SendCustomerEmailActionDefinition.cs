using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.AccountBalance.Business;
using Vanrise.Notification.Entities;

namespace TOne.WhS.AccountBalance.MainExtensions.VRBalanceAlertActions
{
    public class SendCustomerEmailActionDefinition : VRActionDefinitionExtendedSettings
    {
        public Guid MailMessageTypeId { get; set; }

        public override Guid ConfigId
        {
            get { throw new NotImplementedException(); }
        }

        public override bool IsApplicable(IVRActionDefinitionIsApplicableContext context)
        {
            return (context.Target as CustomerAccountBalanceRuleTargetType != null);
        }
    }
}
