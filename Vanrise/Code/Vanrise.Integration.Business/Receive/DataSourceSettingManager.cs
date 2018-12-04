using System;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Integration.Entities;

namespace Vanrise.Integration.Business
{
    public class DataSourceSettingManager
    {
        Vanrise.Common.Business.VRComponentTypeManager _vrComponentTypeManager = new Common.Business.VRComponentTypeManager();
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

        public IEnumerable<FileDataSourceDefinitionInfo> GetFileDataSourceDefinitionInfo(FileDataSourceDefinitionInfoFilter filter)
        {
            var fileDataSourceDefinitions = new ConfigManager().GetFileDataSourceDefinitions();
            return fileDataSourceDefinitions.MapRecords(FileDataSourceDefinitionInfoMapper);
        }

        private FileDataSourceDefinitionInfo FileDataSourceDefinitionInfoMapper(FileDataSourceDefinition fileDataSourceDefinition)
        {
            return new FileDataSourceDefinitionInfo
            {
                FileDataSourceDefinitionId = fileDataSourceDefinition.FileDataSourceDefinitionId,
                Name = fileDataSourceDefinition.Name
            };
        }
    }
}