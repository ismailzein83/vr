using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;

namespace Vanrise.GenericData.RDBDataStorage
{
    public class RDBDataRecordStorageManager
    {
        public IEnumerable<RDBDataRecordStorageSettingsFilterConfig> GetRDBDataRecordStorageSettingsFilterConfigs()
        {
            var extensionConfigurationManager = new ExtensionConfigurationManager();
            return extensionConfigurationManager.GetExtensionConfigurations<RDBDataRecordStorageSettingsFilterConfig>(RDBDataRecordStorageSettingsFilterConfig.EXTENSION_TYPE);
        }

        public IEnumerable<RDBDataRecordStorageExpressionFieldSettingsConfig> GetRDBDataRecordStorageExpressionFieldSettingsConfigs()
        {
            var extensionConfigurationManager = new ExtensionConfigurationManager();
            return extensionConfigurationManager.GetExtensionConfigurations<RDBDataRecordStorageExpressionFieldSettingsConfig>(RDBDataRecordStorageExpressionFieldSettingsConfig.EXTENSION_TYPE);
        }

        public IEnumerable<RDBDataRecordStorageJoinSettingsConfig> GetRDBDataRecordStorageJoinSettingsConfigs()
        {
            var extensionConfigurationManager = new ExtensionConfigurationManager();
            return extensionConfigurationManager.GetExtensionConfigurations<RDBDataRecordStorageJoinSettingsConfig>(RDBDataRecordStorageJoinSettingsConfig.EXTENSION_TYPE);
        }
    }
}
