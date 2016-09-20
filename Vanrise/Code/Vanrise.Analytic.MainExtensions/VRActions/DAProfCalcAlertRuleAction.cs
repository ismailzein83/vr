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
        public override Guid ConfigId { get { return  new Guid("EED64841-21FE-4AA1-996F-0415C9412427"); } }
        public Guid MailMessageTemplateId { get; set; }


        public override void Execute(IVRActionExecutionContext context)
        {
            new VRMailManager().SendMail(this.MailMessageTemplateId, null);
        }
    }
}
