using System;
using System.Collections.Generic;
using Vanrise.Notification.Entities;
using Vanrise.Common.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;

namespace Vanrise.Analytic.MainExtensions.VRActions
{
    public class DAProfCalcAlertRuleAction : VRAction
    {
        public override Guid ConfigId { get { return new Guid("EED64841-21FE-4AA1-996F-0415C9412427"); } }
        public Guid MailMessageTemplateId { get; set; }
        const string MAILTEMPLATE_USEROBJECTNAME = "User";

        public override void Execute(IVRActionExecutionContext context)
        {
            DAProfCalcAlertRuleActionEventPayload payload = context.EventPayload as DAProfCalcAlertRuleActionEventPayload;
            UserManager userManager = new UserManager();
            User user = userManager.GetUserbyId(payload.UserId);

            Dictionary<string, dynamic> mailTemplateObjects = new Dictionary<string, dynamic>();
            mailTemplateObjects.Add(MAILTEMPLATE_USEROBJECTNAME, user);

            new VRMailManager().SendMail(this.MailMessageTemplateId, mailTemplateObjects);
        }
    }
}