using System;
using Vanrise.Notification.Entities;

namespace Vanrise.AccountBalance.MainExtensions
{
    public class GenericSendEmailActionDefinitionSettings : VRActionDefinitionExtendedSettings
    {
        public override Guid ConfigId => new Guid("A0E3AC54-E002-495D-8F74-A49CF02FA099");
        public Guid FinancialAccountBEDefinitionId { get; set; }
        public Guid MailMessageTypeId { get; set; }
        public override string RuntimeEditor
        {
            get
            {
                return "vr-accountbalance-accountaction-sendemail";
            }
        }
        public override bool IsApplicable(IVRActionDefinitionIsApplicableContext context)
        {
            GenericAccountBalanceRuleTargetType targetType = context.Target as GenericAccountBalanceRuleTargetType;
            if (targetType == null)
                return false;
            if (targetType.FinancialAccountBEDefinitionId != this.FinancialAccountBEDefinitionId)
                return false;

            return true;
        }
    }
}
