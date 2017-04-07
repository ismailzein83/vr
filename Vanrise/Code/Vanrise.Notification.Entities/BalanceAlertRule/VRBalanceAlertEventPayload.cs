﻿using System;

namespace Vanrise.Notification.Entities
{
    public class VRBalanceAlertEventPayload : IVRActionEventPayload
    {
        public Guid AlertRuleTypeId { get; set; }

        public string EntityId { get; set; }

        public Decimal CurrentBalance { get; set; }

        public Decimal Threshold { get; set; }
    }
}
