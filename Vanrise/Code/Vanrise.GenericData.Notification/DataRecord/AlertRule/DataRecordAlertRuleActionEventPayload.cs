using System;
using System.Collections.Generic;
using Vanrise.Notification.Entities;

namespace Vanrise.GenericData.Notification
{
    public class DataRecordAlertRuleActionEventPayload : IVRActionEventPayload
    {
        public Guid DataRecordTypeId { get; set; }

        public Dictionary<string, dynamic> OutputRecords { get; set; }
    }
}
