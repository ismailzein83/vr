using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;

namespace Vanrise.GenericData.Notification
{
    public class DataRecordAlertRuleManager
    {
        public IEnumerable<DataRecordAlertRuleConfig> GetDataRecordAlertRuleConfigs()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<DataRecordAlertRuleConfig>(DataRecordAlertRuleConfig.EXTENSION_TYPE);
        }
    }
}