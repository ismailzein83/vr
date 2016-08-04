using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;
using Vanrise.Notification.Entities;

namespace Vanrise.AccountBalance.Business
{
    public class BalanceAlertEventPayload : IVRActionEventPayload
    {
        public long AccountId { get; set; }

        public Decimal Threshold { get; set; }
    }
}
