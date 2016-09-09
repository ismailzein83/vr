using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Notification.Entities;

namespace Vanrise.Analytic.MainExtensions.VRActions
{
    public class DAProfCalcAlertRuleAction : VRAction
    {
        public Guid MailMessageTemplateId { get; set; }


        public override void Execute(IVRActionExecutionContext context)
        {

        }
    }
}
