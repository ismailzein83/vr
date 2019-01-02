using System;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Integration.Entities;
using System.Linq;

namespace Vanrise.Integration.Business
{
    public class ConfigManager
    {
        public Vanrise.Entities.InsertOperationOutput<FileDataSourceDefinition> AddFileDataSourceDefinition(FileDataSourceDefinition fileDataSourceDefinition)
        {
            DataSourceSettingData dataSourceSettingData = this.GetDataSourceSettingData();
            if (dataSourceSettingData == null)
                dataSourceSettingData = new DataSourceSettingData();
            if (dataSourceSettingData.FileDataSourceSettings == null)
                dataSourceSettingData.FileDataSourceSettings = new FileDataSourceSettings();
            if (dataSourceSettingData.FileDataSourceSettings.FileDataSourceDefinitions == null)
                dataSourceSettingData.FileDataSourceSettings.FileDataSourceDefinitions = new List<FileDataSourceDefinition>();

            dataSourceSettingData.FileDataSourceSettings.FileDataSourceDefinitions.Add(fileDataSourceDefinition);

            SettingToEdit dataSourceSettingToEdit = new SettingToEdit()
            {
                Name = "Data Source",
                SettingId = new Guid("0D693835-9A45-4D35-826B-A6DA59011B4B"),
                Data = dataSourceSettingData
            };
            new SettingManager().UpdateSetting(dataSourceSettingToEdit);

            Vanrise.Entities.InsertOperationOutput<FileDataSourceDefinition> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<FileDataSourceDefinition>();
            insertOperationOutput.InsertedObject = fileDataSourceDefinition;
            insertOperationOutput.Result = InsertOperationResult.Succeeded;
            return insertOperationOutput;
        }

        public List<PeakTimeRange> GetPeakTimeRanges()
        {
            FileDataSourceSettings fileDataSourceSettings = GetFileDataSourceSettings();
            return fileDataSourceSettings.PeakTimeRanges;
        }

        public FileDataSourceDefinition GetFileDataSourceDefinition(Guid fileDataSourceDefinitionId)
        {
            FileDataSourceDefinition fileDataSourceDefinition = GetFileDataSourceDefinitions().FindRecord(itm => itm.FileDataSourceDefinitionId == fileDataSourceDefinitionId);
            fileDataSourceDefinition.ThrowIfNull("fileDataSourceDefinition", fileDataSourceDefinitionId);
            return fileDataSourceDefinition;
        }

        public IEnumerable<FileDataSourceDefinition> GetFileDataSourceDefinitions()
        {
            FileDataSourceSettings fileDataSourceSettings = GetFileDataSourceSettings();
            fileDataSourceSettings.FileDataSourceDefinitions.ThrowIfNull("fileDataSourceSettings.FileDataSourceDefinitions");
            return fileDataSourceSettings.FileDataSourceDefinitions;
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