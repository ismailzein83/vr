using System;
using System.Collections.Generic;
using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Notification.Entities;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;

namespace Retail.BusinessEntity.MainExtensions.AccountActions
{
    public class SendEmailAction : BaseAccountBalanceAlertVRAction
    {
        public Guid MailMessageTemplateId { get; set; }

        public override void Execute(IVRActionExecutionContext context)
        {
            VRBalanceAlertEventPayload eventPayload = context.EventPayload as VRBalanceAlertEventPayload;
            eventPayload.ThrowIfNull("eventPayload", "");


            Dictionary<string, dynamic> objects = new Dictionary<string, dynamic>();

            UserManager userManager = new UserManager();
            User user = userManager.GetUserbyId(context.UserID);
            user.ThrowIfNull("user", context.UserID);

            Guid accountDefinitionId;
            Account account = GetAccount(eventPayload, out accountDefinitionId);
            account.ThrowIfNull("account", eventPayload.EntityId);

            objects.Add("User", user);
            objects.Add("Subscriber", account);
            objects.Add("SubscriberBalance", eventPayload.CurrentBalance);
            objects.Add("Threshold", eventPayload.Threshold);
            objects.Add("CreditLimit", GetAccountCreditLimit(eventPayload, out accountDefinitionId));
            VRMailManager mailManager = new VRMailManager();
            mailManager.SendMail(this.MailMessageTemplateId, objects);

        }
    }

    public class SendEmailActionDefinitionSettings : VRActionDefinitionExtendedSettings
    {
        public Guid AccountBEDefinitionId { get; set; }
        public Guid MailMessageTypeId { get; set; }
        public override Guid ConfigId
        {
            get { return new Guid("1BDACFE6-F050-4187-9E96-9647049605D3"); }
        }
        public override string RuntimeEditor
        {
            get
            {
                return "retail-be-accountaction-email";
            }
        }

        public override bool IsApplicable(IVRActionDefinitionIsApplicableContext context)
        {
            RetailAccountBalanceRuleTargetType targetType = context.Target as RetailAccountBalanceRuleTargetType;
            if (targetType == null)
                return false;
            if (targetType.AccountBEDefinitionId != this.AccountBEDefinitionId)
                return false;

            return true;
        }
    }
}
