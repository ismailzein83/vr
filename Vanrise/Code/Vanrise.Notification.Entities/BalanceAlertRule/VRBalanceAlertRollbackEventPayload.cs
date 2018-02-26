using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Notification.Entities
{
    public class VRBalanceAlertRollbackEventPayload : IVRActionRollbackEventPayload
    {
        public Decimal CurrentBalance { get; set; }
    }
}
