using System;
using System.Collections.Generic;
using Vanrise.Notification.Entities;

namespace Vanrise.GenericData.Notification
{
    public class DataRecordAlertRuleNotification
    {
        public List<VRAction> Actions { get; set; }
        public Guid AlertLevelId { get; set; }
        public Dictionary<string, dynamic> FieldValues { get; set; }
        public string GroupingKey { get; set; }
    }
}