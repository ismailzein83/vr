using System;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Integration.Business;
using Vanrise.Integration.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Integration.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "DataSourceSetting")]
    public class DataSourceSettingController : Vanrise.Web.Base.BaseAPIController
    {
        DataSourceSettingManager dataSourceSettingsManager = new DataSourceSettingManager();

        [HttpGet]
        [Route("GetFileDelayCheckerSettingsConfigs")]
        public IEnumerable<FileDelayCheckerSettingsConfig> GetFileDelayCheckerSettingsConfigs()
        {
            return dataSourceSettingsManager.GetFileDelayCheckerSettingsConfigs();
        }

        [HttpGet]
        [Route("GetFileMissingCheckerSettingsConfigs")]
        public IEnumerable<FileMissingCheckerSettingsConfig> GetFileMissingCheckerSettingsConfigs()
        {
            return dataSourceSettingsManager.GetFileMissingCheckerSettingsConfigs();
        }

        [HttpGet]
        [Route("GetFileDataSourceDefinitionInfo")]
        public IEnumerable<FileDataSourceDefinitionInfo> GetFileDataSourceDefinitionInfo(string filter = null)
        {
            FileDataSourceDefinitionInfoFilter fileDataSourceDefinitionInfoFilter = filter != null ? Vanrise.Common.Serializer.Deserialize<FileDataSourceDefinitionInfoFilter>(filter) : null;
            return dataSourceSettingsManager.GetFileDataSourceDefinitionInfo(fileDataSourceDefinitionInfoFilter);
        }

        [HttpPost]
        [Route("AddFileDataSourceDefinition")]
        public Vanrise.Entities.InsertOperationOutput<FileDataSourceDefinition> AddFileDataSourceDefinition(FileDataSourceDefinition fileDataSourceDefinition)
        {
            ConfigManager configManager = new ConfigManager();
            return configManager.AddFileDataSourceDefinition(fileDataSourceDefinition);
        }

        [HttpGet]
        [Route("GetFileDataSourceDefinition")]
        public FileDataSourceDefinition GetFileDataSourceDefinition(Guid fileDataSourceDefinitionId)
        {
            return new ConfigManager().GetFileDataSourceDefinition(fileDataSourceDefinitionId);
        }

    }
}