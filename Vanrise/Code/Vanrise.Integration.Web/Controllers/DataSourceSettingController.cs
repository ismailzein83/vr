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
    }
}