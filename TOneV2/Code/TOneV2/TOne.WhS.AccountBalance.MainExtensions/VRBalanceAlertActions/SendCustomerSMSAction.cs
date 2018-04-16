using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Notification.Entities;

namespace TOne.WhS.AccountBalance.MainExtensions.VRBalanceAlertActions
{
    public class SendCustomerSMSAction : VRAction
    {
        public Guid AccountSMSTemplateId { get; set; }

        public Guid ProfileSMSTemplateId { get; set; }
        public override void Execute(IVRActionExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
