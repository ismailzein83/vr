using System;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Integration.Entities;

namespace Vanrise.Integration.Business
{
    public class ConfigManager
    {
        public FileDataSourceDefinition GetFileDataSourceDefinition(Guid fileDataSourceDefinitionId)
        {
            FileDataSourceSettings fileDataSourceSettings = GetFileDataSourceSettings();
            fileDataSourceSettings.FileDataSourceDefinitions.ThrowIfNull("fileDataSourceSettings.FileDataSourceDefinitions");
            FileDataSourceDefinition fileDataSourceDefinition = fileDataSourceSettings.FileDataSourceDefinitions.FindRecord(itm => itm.FileDataSourceDefinitionId == fileDataSourceDefinitionId);
            fileDataSourceDefinition.ThrowIfNull("fileDataSourceDefinition", fileDataSourceDefinitionId);

            return fileDataSourceDefinition;
        }

        private FileDataSourceSettings GetFileDataSourceSettings()
        {
            DataSourceSettingData dataSourceSettingData = GetDataSourceSettingData();
            dataSourceSettingData.FileDataSourceSettings.ThrowIfNull("dataSourceSettingData.FileDataSourceSettings");

            return dataSourceSettingData.FileDataSourceSettings;
        }

        private DataSourceSettingData GetDataSourceSettingData()
        {
            SettingManager settingManager = new SettingManager();
            DataSourceSettingData dataSourceSettingData = settingManager.GetSetting<DataSourceSettingData>(Constants.DataSourceSettings);
            dataSourceSettingData.ThrowIfNull("dataSourceSettingData");

            return dataSourceSettingData;
        }
    }
}