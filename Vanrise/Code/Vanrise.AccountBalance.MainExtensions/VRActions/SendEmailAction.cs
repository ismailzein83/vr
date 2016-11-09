using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Business;
using Vanrise.AccountBalance.Business.Extensions;
using Vanrise.Common.Business;
using Vanrise.Notification.Business;
using Vanrise.Notification.Entities;

namespace Vanrise.AccountBalance.MainExtensions.VRActions
{
    public class SendEmailAction : VRAction
    {
        public override Guid ConfigId { get { return new Guid("be74a60e-d312-4b4f-bd76-5b7be81abe62"); } }
        const string MAILTEMPLATE_ACCOUNTOBJECTNAME = "Account";

        public Guid MailTemplateId { get; set; }

        public override void Execute(IVRActionExecutionContext context)
        {
            BalanceAlertEventPayload eventPayload = context.EventPayload as BalanceAlertEventPayload;
            if (eventPayload == null)
                throw new NullReferenceException("eventPayload");
            var account = new AccountManager().GetAccount(eventPayload.AccountId);
            if (account == null)
                throw new NullReferenceException(String.Format("account '{0}'", eventPayload.AccountId));
            Dictionary<string, dynamic> mailTemplateObjects = new Dictionary<string, dynamic>();
            mailTemplateObjects.Add(MAILTEMPLATE_ACCOUNTOBJECTNAME, account);
            new VRMailManager().SendMail(this.MailTemplateId, mailTemplateObjects);
        }
    }

    public class SendEmailAction2 : VRAction
    {
        public override Guid ConfigId { get { return new Guid("be74a60e-d312-4b4f-bd76-5b7be81abe62"); } }

        const string MAILTEMPLATE_ACCOUNTOBJECTNAME = "Account";

        public Guid MailTemplateId { get; set; }

        static VRAlertRuleTypeManager s_alertRuleTypeManager = new VRAlertRuleTypeManager();

        public override void Execute(IVRActionExecutionContext context)
        {
            Vanrise.Notification.Entities.VRBalanceAlertEventPayload eventPayload = context.EventPayload as Vanrise.Notification.Entities.VRBalanceAlertEventPayload;
            if (eventPayload == null)
                throw new NullReferenceException("eventPayload");
            if (string.IsNullOrEmpty(eventPayload.EntityId))
                throw new NullReferenceException("eventPayload.EntityId");
            long accountId;
            if (!long.TryParse(eventPayload.EntityId, out accountId))
                throw new Exception(String.Format("eventPayload.EntityId '{0}' is not a valid long", eventPayload.EntityId));
            AccountBalanceAlertRuleTypeSettings accountBalanceAlertRuleTypeSettings = s_alertRuleTypeManager.GetVRAlertRuleTypeSettings<AccountBalanceAlertRuleTypeSettings>(eventPayload.AlertRuleTypeId);

            var account = new AccountManager().GetAccount(accountBalanceAlertRuleTypeSettings.AccountTypeId, accountId);
            if (account == null)
                throw new NullReferenceException(String.Format("account '{0}'", accountId));
            Dictionary<string, dynamic> mailTemplateObjects = new Dictionary<string, dynamic>();
            mailTemplateObjects.Add(MAILTEMPLATE_ACCOUNTOBJECTNAME, account);
            new VRMailManager().SendMail(this.MailTemplateId, mailTemplateObjects);
        }
    }
}
