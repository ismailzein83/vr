using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Business;
using Vanrise.Common.Business;
using Vanrise.Notification.Entities;

namespace Vanrise.AccountBalance.MainExtensions.VRActions
{
    public class SendEmailAction : VRAction
    {
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
}
