using System;
using System.Collections.Generic;
using System.Web.Http;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.Routing.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "QualityConfigurationDefinition")]
    [JSONWithTypeAttribute]
    public class QualityConfigurationDefinitionController : Vanrise.Web.Base.BaseAPIController
    {
        QualityConfigurationDefinitionManager _manager = new QualityConfigurationDefinitionManager();

        [HttpGet]
        [Route("GetQualityConfigurationDefinition")]
        public QualityConfigurationDefinition GetQualityConfigurationDefinition(Guid qualityConfigurationDefinitionId)
        {
            return _manager.GetQualityConfigurationDefinition(qualityConfigurationDefinitionId);
        }

        [HttpGet]
        [Route("GetQualityConfigurationDefinitionInfo")]
        public IEnumerable<QualityConfigurationDefinitionInfo> GetQualityConfigurationDefinitionInfo(string filter = null)
        {
            var qualityConfigurationDefinitionFilter = filter != null ? Vanrise.Common.Serializer.Deserialize<QualityConfigurationDefinitionFilter>(filter) : null;
            return _manager.GetQualityConfigurationDefinitionInfo(qualityConfigurationDefinitionFilter);
        }

        [HttpGet]
        [Route("GetQualityConfigurationDefinitionExtendedSettingsConfigs")]
        public IEnumerable<QualityConfigurationDefinitionExtendedSettingsConfig> GetQualityConfigurationDefinitionExtendedSettingsConfigs()
        {
            return _manager.GetQualityConfigurationDefinitionExtendedSettingsConfigs(); 
        }
    }
}