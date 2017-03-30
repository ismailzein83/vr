using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Notification.Entities;
using Vanrise.Common;

namespace Vanrise.GenericData.Notification
{
    public class DataRecordAlertRuleTypeFilter : IVRAlertRuleTypeFilter
    {
        public Guid DataRecordTypeId { get; set; }

        public bool IsMatch(VRAlertRuleType alertRuleType)
        {
            if (alertRuleType == null)
                throw new NullReferenceException("alertRuleType");

            if (alertRuleType.Settings == null)
                throw new NullReferenceException("alertRuleType.Settings");

            if (alertRuleType.Settings.ConfigId != DataRecordAlertRuleTypeSettings.s_configID)
                return false;

            var dataRecordAlertRuleTypeSettings = alertRuleType.Settings as DataRecordAlertRuleTypeSettings;
            if (dataRecordAlertRuleTypeSettings.DataRecordTypeId != this.DataRecordTypeId)
                return false;

            return true;
        }
    }
}
