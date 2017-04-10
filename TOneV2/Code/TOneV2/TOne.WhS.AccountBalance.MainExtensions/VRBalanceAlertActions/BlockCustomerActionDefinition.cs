using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.AccountBalance.Business;
using Vanrise.Notification.Entities;

namespace TOne.WhS.AccountBalance.MainExtensions
{
    public class BlockCustomerActionDefinition : VRActionDefinitionExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("55E97A73-994A-4D60-9A9E-BBD04D08929D"); }
        }

        public override string RuntimeEditor
        {
            get
            {
                return "whs-accountbalance-action-customer-block";
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
