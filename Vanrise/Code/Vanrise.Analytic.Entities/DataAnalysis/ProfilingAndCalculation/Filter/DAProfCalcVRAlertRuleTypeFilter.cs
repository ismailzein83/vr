using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Notification.Entities;

namespace Vanrise.Analytic.Entities
{
    class DAProfCalcVRAlertRuleTypeFilter : IVRAlertRuleTypeFilter
    {
        Guid _configId { get { return new Guid("57033e80-65cb-4359-95f6-22a57084d027"); } }
        public bool IsMatch(VRAlertRuleType alertRuleType)
        {
            if (alertRuleType == null)
                throw new NullReferenceException("alertRuleType");

            if (alertRuleType.Settings == null)
                throw new NullReferenceException("alertRuleType.Settings");

            if (alertRuleType.Settings.ConfigId != _configId)
                return false;

            return true;
        }
    }
}
