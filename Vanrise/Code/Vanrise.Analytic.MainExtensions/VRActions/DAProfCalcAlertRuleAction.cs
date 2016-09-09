using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Notification.Entities;
using Vanrise.Common.Business;

namespace Vanrise.Analytic.MainExtensions.VRActions
{
    public class DAProfCalcAlertRuleAction : VRAction
    {
        public Guid MailMessageTemplateId { get; set; }


        public override void Execute(IVRActionExecutionContext context)
        {
            new VRMailManager().SendMail(this.MailMessageTemplateId, null);
        }
    }
}
