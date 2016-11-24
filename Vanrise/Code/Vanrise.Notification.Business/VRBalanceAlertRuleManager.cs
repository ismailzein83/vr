using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Notification.Entities;

namespace Vanrise.Notification.Business
{
    public class VRBalanceAlertRuleManager : VRGenericAlertRuleManager
    {
        public IEnumerable<VRBalanceAlertRuleThresholdConfig> GetVRBalanceAlertThresholdConfigs(string extensionType)
        {
            var extensionConfiguration = new ExtensionConfigurationManager();
            return extensionConfiguration.GetExtensionConfigurations<VRBalanceAlertRuleThresholdConfig>(extensionType);
        }
    }
}
