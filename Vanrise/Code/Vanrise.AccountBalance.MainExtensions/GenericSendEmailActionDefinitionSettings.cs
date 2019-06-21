using System;
using Vanrise.Notification.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using System.Collections.Generic;
using Vanrise.GenericData.Business;

namespace Vanrise.AccountBalance.MainExtensions
{
    public class GenericSendEmailAction : VRAction
    {
        public override bool CanReexecute => true;
        public Guid MailMessageTemplateId { get; set; }
        public override void Execute(IVRActionExecutionContext context)
        {
            VRBalanceAlertEventPayload eventPayload = context.EventPayload.CastWithValidate<VRBalanceAlertEventPayload>("eventPayload");

            Dictionary<string, dynamic> objects = new Dictionary<string, dynamic>();
            Decimal currentBalance;
            if (context.RollbackEventPayload != null)
                currentBalance = context.RollbackEventPayload.CastWithValidate<VRBalanceAlertRollbackEventPayload>("context.RollbackEventPayload").CurrentBalance;
            else
                currentBalance = eventPayload.CurrentBalance;
            objects.Add("SubscriberBalance", currentBalance);
            objects.Add("Threshold", eventPayload.Threshold);
            VRMailManager mailManager = new VRMailManager();
            mailManager.SendMail(this.MailMessageTemplateId, objects);
        }
    }



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
