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

            Account account = GetAccount(eventPayload);
            account.ThrowIfNull("account", eventPayload.EntityId);

            objects.Add("User", user);
            objects.Add("Subscriber", account);
            objects.Add("SubscriberBalance", eventPayload.CurrentBalance);
            objects.Add("Threshold", eventPayload.Threshold);

            VRMailManager mailManager = new VRMailManager();
            mailManager.SendMail(this.MailMessageTemplateId, objects);

        }
    }
}
