using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.AccountBalance.Business;
using Vanrise.Notification.Entities;

namespace TOne.WhS.AccountBalance.MainExtensions
{
    public class SendCustomerEmailActionDefinition : VRActionDefinitionExtendedSettings
    {
        public Guid MailMessageTypeId { get; set; }

        public override Guid ConfigId
        {
            get { return new Guid("DA72C2E9-4321-4C0C-8FCA-B8A363F6B480"); }
        }

        public override string RuntimeEditor
        {
            get
            {
                return "whs-accountbalance-action-customer-sendemail";
            }
            set
            {
                base.RuntimeEditor = value;
            }
        }

        public override bool IsApplicable(IVRActionDefinitionIsApplicableContext context)
        {
            return (context.Target as CustomerAccountBalanceRuleTargetType != null);
        }
    }
}
