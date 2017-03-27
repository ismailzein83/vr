using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Notification.Entities;

namespace Vanrise.GenericData.Notification
{
    public class DataRecordActionRuleExtendedSettings : VRAlertRuleExtendedSettings
    {
        public List<ActionRuleTypeRecordField> AvailableIdentificationFields { get; set;  }

        public TimeSpan MinNotificationInterval { get; set; }

        public DataRecordAlertRuleSettings Settings { get; set; }
    }
}
