using System;

namespace Vanrise.Notification.Entities
{
    public class VRBalanceAlertEventPayload : IVRActionEventPayload
    {
        public string EntityId { get; set; }

        public Decimal CurrentBalance { get; set; }

        public Decimal Threshold { get; set; }
    }
}
