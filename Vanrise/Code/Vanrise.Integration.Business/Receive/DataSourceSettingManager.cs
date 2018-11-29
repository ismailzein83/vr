using System.Collections.Generic;
using Vanrise.Common.Business;
using Vanrise.Integration.Entities;

namespace Vanrise.Integration.Business
{
    public class DataSourceSettingManager
    {
        public IEnumerable<FileDelayCheckerSettingsConfig> GetFileDelayCheckerSettingsConfigs()
        {
            var extensionConfigurationManager = new ExtensionConfigurationManager();
            return extensionConfigurationManager.GetExtensionConfigurations<FileDelayCheckerSettingsConfig>(FileDelayCheckerSettingsConfig.EXTENSION_TYPE);
        }

        public IEnumerable<FileMissingCheckerSettingsConfig> GetFileMissingCheckerSettingsConfigs()
        {
            var extensionConfigurationManager = new ExtensionConfigurationManager();
            return extensionConfigurationManager.GetExtensionConfigurations<FileMissingCheckerSettingsConfig>(FileMissingCheckerSettingsConfig.EXTENSION_TYPE);
        }
    }
}

