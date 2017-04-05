using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Notification.Entities;

namespace Vanrise.GenericData.Notification
{
    public class DataRecordAlertRuleExtendedSettings : VRAlertRuleExtendedSettings
    {
        public List<AlertRuleTypeRecordField> AvailableIdentificationFields { get; set;  }

        public TimeSpan MinNotificationInterval { get; set; }

        public DataRecordAlertRuleSettings Settings { get; set; }
    }
}
