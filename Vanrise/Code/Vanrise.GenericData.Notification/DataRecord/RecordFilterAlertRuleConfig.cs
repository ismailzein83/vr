using System;
using System.Collections.Generic;
using Vanrise.GenericData.Entities;
using Vanrise.Notification.Entities;

namespace Vanrise.GenericData.Notification
{
    public class RecordAlertRuleSettings : DataRecordAlertRuleSettings
    {
        public override Guid ConfigId { get { return new Guid("AAF0C72D-1C8F-47DA-AC11-DD7819B93351"); } }

        public List<RecordAlertRuleConfig> RecordAlertRuleConfigs { get; set; }
    }

    public class RecordAlertRuleConfig
    {
        public RecordFilterGroup FilterGroup { get; set; }

        public List<VRAction> Actions { get; set; }
    }
}