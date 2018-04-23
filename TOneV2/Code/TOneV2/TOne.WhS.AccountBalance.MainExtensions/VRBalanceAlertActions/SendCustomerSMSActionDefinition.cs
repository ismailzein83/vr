using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.AccountBalance.Business;
using Vanrise.Notification.Entities;

namespace TOne.WhS.AccountBalance.MainExtensions.VRBalanceAlertActions
{
    public class SendCustomerSMSActionDefinition : VRActionDefinitionExtendedSettings
    {
        public Guid AccountSMSMessageTypeId { get; set; }
        public Guid ProfileSMSMessageTypeId { get; set; }
        public Vanrise.Entities.SMSSendHandler Handler { get; set; }
        public override Guid ConfigId
        {
            get { return new Guid("114CE0AC-4848-442A-B351-BE031F22E130"); }
        }
        public override string RuntimeEditor
        {
            get
            {
                return "whs-accountbalance-action-send-customer-sms";
            }
        }

        public override bool IsApplicable(IVRActionDefinitionIsApplicableContext context)
        {
            return (context.Target as CustomerAccountBalanceRuleTargetType != null);
        }
    }
}
