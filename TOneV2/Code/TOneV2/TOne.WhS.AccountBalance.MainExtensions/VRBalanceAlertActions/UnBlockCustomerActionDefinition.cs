using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.AccountBalance.Business;
using Vanrise.Notification.Entities;

namespace TOne.WhS.AccountBalance.MainExtensions.VRBalanceAlertActions
{
    public class UnBlockCustomerActionDefinition : VRActionDefinitionExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("86912E3E-7305-4D5D-8796-7CCF88B9A7FA"); }
        }
        public override string RuntimeEditor
        {
            get
            {
                return "whs-accountbalance-action-customer-unblock";
            }
        }

        public override bool IsApplicable(IVRActionDefinitionIsApplicableContext context)
        {
            return (context.Target as CustomerAccountBalanceRuleTargetType != null);
        }
    }
}
