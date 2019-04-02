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
        #region Public Methods

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
            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<FileDataSourceDefinition>();

            UpdateOperationOutput<SettingDetail> updateOperationOutput = new SettingManager().UpdateSetting(dataSourceSettingToEdit);
            if (updateOperationOutput.Result == UpdateOperationResult.Succeeded && updateOperationOutput.UpdatedObject != null)
            {
                insertOperationOutput.InsertedObject = fileDataSourceDefinition;
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
            }
            else
            {
                insertOperationOutput.InsertedObject = null;
                insertOperationOutput.Result = InsertOperationResult.Failed;
            }

            return insertOperationOutput;
        }

        public List<PeakTimeRange> GetPeakTimeRanges()
        {
            FileDataSourceSettings fileDataSourceSettings = GetFileDataSourceSettings();
            if (fileDataSourceSettings == null)
                return null;

            return fileDataSourceSettings.PeakTimeRanges;
        }

        public FileDataSourceDefinition GetFileDataSourceDefinition(Guid fileDataSourceDefinitionId)
        {
            IEnumerable<FileDataSourceDefinition> fileDataSourceDefinitions = this.GetFileDataSourceDefinitions();
            fileDataSourceDefinitions.ThrowIfNull("fileDataSourceDefinitions", fileDataSourceDefinitionId);

            FileDataSourceDefinition fileDataSourceDefinition = fileDataSourceDefinitions.FindRecord(itm => itm.FileDataSourceDefinitionId == fileDataSourceDefinitionId);
            fileDataSourceDefinition.ThrowIfNull("fileDataSourceDefinition", fileDataSourceDefinitionId);
            return fileDataSourceDefinition;
        }

        public IEnumerable<FileDataSourceDefinition> GetFileDataSourceDefinitions()
        {
            FileDataSourceSettings fileDataSourceSettings = this.GetFileDataSourceSettings();
            if (fileDataSourceSettings == null)
                return null;

            return fileDataSourceSettings.FileDataSourceDefinitions;
        }

        public bool IsFileDataSourceDefinitionInUse(Guid fileDataSourceDefinitionId)
        {
            var allDataSources = new DataSourceManager().GetAllDataSources();
            if (allDataSources == null || allDataSources.Count() == 0)
                return false;

            foreach (var dataSource in allDataSources)
            {
                DataSourceSettings settings = dataSource.Settings;
                if (settings == null || settings.AdapterArgument == null)
                    continue;

                if (settings.AdapterArgument.IsFileDataSourceDefinitionInUse(fileDataSourceDefinitionId))
                    return true;
            }

            return false;
        }

        #endregion

        #region Private Methods

        private FileDataSourceSettings GetFileDataSourceSettings()
        {
            DataSourceSettingData dataSourceSettingData = GetDataSourceSettingData();
            if (dataSourceSettingData == null)
                return null;

            return dataSourceSettingData.FileDataSourceSettings;
        }

        private DataSourceSettingData GetDataSourceSettingData()
        {
            SettingManager settingManager = new SettingManager();
            DataSourceSettingData dataSourceSettingData = settingManager.GetSetting<DataSourceSettingData>(Constants.DataSourceSettings);
            return dataSourceSettingData;
        }

        #endregion
    }
}