using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Notification.Entities;

namespace TOne.WhS.AccountBalance.MainExtensions.VRBalanceAlertActions
{
    public class SendCustomerEmailAction : VRAction
    {
        public Guid AccountMailTemplateId { get; set; }

        public Guid ProfileMailTemplateId { get; set; }

        public override void Execute(IVRActionExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
