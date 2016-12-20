using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using Vanrise.Common.Business;
using Vanrise.Notification.Entities;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;
using Vanrise.Common;

namespace Retail.BusinessEntity.MainExtensions.AccountActions
{
    public class SendEmailAction : VRAction
    {
        public Guid MailMessageTemplateId { get; set; }
        public override Guid ConfigId
        {
            get { return new Guid("BE74A60E-D312-4B4F-BD76-5B7BE81ABE62"); }
        }

        public override void Execute(IVRActionExecutionContext context)
        {
            VRBalanceAlertEventPayload eventPayload = context.EventPayload as VRBalanceAlertEventPayload;
            eventPayload.ThrowIfNull("eventPayload", "");

            Dictionary<string, dynamic> objects = new Dictionary<string, dynamic>();

            UserManager userManager = new UserManager();
            User user = userManager.GetUserbyId(context.UserID);
            user.ThrowIfNull("user", context.UserID);

            AccountManager accountManager = new AccountManager();
            long accountId = 0;
            long.TryParse(eventPayload.EntityId, out accountId);
            Account account = accountManager.GetAccount(accountId);
            account.ThrowIfNull("account", accountId);

            objects.Add("User", user);
            objects.Add("Subscriber", account);
            objects.Add("SubscriberBalance", eventPayload.CurrentBalance);
            objects.Add("Threshold", eventPayload.Threshold);

            VRMailManager mailManager = new VRMailManager();
            mailManager.SendMail(this.MailMessageTemplateId, objects);

        }
    }


}
