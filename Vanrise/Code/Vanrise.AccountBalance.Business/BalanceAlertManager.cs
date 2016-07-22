using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;
using Vanrise.Common.Business;

namespace Vanrise.AccountBalance.Business
{
    public class BalanceAlertManager
    {
        public IEnumerable<BalanceAlertThresholdConfig> GetBalanceAlertThresholdConfigs()
        {
            var extensionConfiguration = new ExtensionConfigurationManager();
            return extensionConfiguration.GetExtensionConfigurations<BalanceAlertThresholdConfig>(BalanceAlertThresholdConfig.EXTENSION_TYPE);
        }
    }
}
