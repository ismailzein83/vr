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

        public IEnumerable<GenericData.RDBDataStorage.RDBDataRecordStorageExpressionFieldSettingsConfig> GetRDBDataRecordStorageExpressionFieldSettingsConfigs()
        {
            var extensionConfigurationManager = new ExtensionConfigurationManager();
            return extensionConfigurationManager.GetExtensionConfigurations<GenericData.RDBDataStorage.RDBDataRecordStorageExpressionFieldSettingsConfig>(GenericData.RDBDataStorage.RDBDataRecordStorageExpressionFieldSettingsConfig.EXTENSION_TYPE);
        }

        public IEnumerable<GenericData.RDBDataStorage.RDBDataRecordStorageJoinSettingsConfig> GetRDBDataRecordStorageJoinSettingsConfigs()
        {
            var extensionConfigurationManager = new ExtensionConfigurationManager();
            return extensionConfigurationManager.GetExtensionConfigurations<GenericData.RDBDataStorage.RDBDataRecordStorageJoinSettingsConfig>(GenericData.RDBDataStorage.RDBDataRecordStorageJoinSettingsConfig.EXTENSION_TYPE);
        }
    }
}
