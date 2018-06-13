using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Analytic.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "AutomatedReportQueryDefinitionSettings")]
    [JSONWithTypeAttribute]
    public class AutomatedReportQueryDefinitionSettingsController : BaseAPIController
    {

        [HttpGet]
        [Route("GetAutomatedReportTemplateConfigs")]
        public IEnumerable<VRAutomatedReportQueryDefinitionSettingsConfig> GetAutomatedReportTemplateConfigs()
        {
            VRAutomatedReportQueryDefinitionManager vrAutomatedReportQueryDefinitionManager = new VRAutomatedReportQueryDefinitionManager();
            return vrAutomatedReportQueryDefinitionManager.GetAutomatedReportTemplateConfigs();
        }

        [HttpGet]
        [Route("GetVRAutomatedReportQueryDefinitionsInfo")]
        public IEnumerable<RecordSearchQueryDefinitionInfo> GetVRAutomatedReportQueryDefinitionsInfo(RecordSearchQueryDefinitionInfoFilter filter)
        {
            VRAutomatedReportQueryDefinitionManager vrAutomatedReportQueryDefinitionManager = new VRAutomatedReportQueryDefinitionManager();
            return vrAutomatedReportQueryDefinitionManager.GetVRAutomatedReportQueryDefinitionsInfo(filter);
        }

        [HttpGet]
        [Route("GetVRAutomatedReportQueryDefinitionSettings")]
        public VRAutomatedReportQueryDefinitionSettings GetVRAutomatedReportQueryDefinitionSettings(Guid vrComponentTypeId)
        {
            VRAutomatedReportQueryDefinitionManager vrAutomatedReportQueryDefinitionManager = new VRAutomatedReportQueryDefinitionManager();
            return vrAutomatedReportQueryDefinitionManager.GetVRAutomatedReportQueryDefinitionSettings(vrComponentTypeId);
        }

        [HttpPost]
        [Route("GetAutomatedReportDataSchema")]
        public Dictionary<Guid,VRAutomatedReportDataSchema> GetAutomatedReportDataSchema(AutomatedReportQueries input)
        {
            VRAutomatedReportQueryDefinitionManager vrAutomatedReportQueryDefinitionManager = new VRAutomatedReportQueryDefinitionManager();
            return vrAutomatedReportQueryDefinitionManager.GetAutomatedReportDataSchema(input);
        }

        [HttpPost]
        [Route("ValidateQueryAndHandlerSettings")]
        public ValidateQueryAndHandlerSettingsResult ValidateQueryAndHandlerSettings(ValidateQueryAndHandlerSettingsInput input)
        {
            VRAutomatedReportQueryDefinitionManager vrAutomatedReportQueryDefinitionManager = new VRAutomatedReportQueryDefinitionManager();
            return vrAutomatedReportQueryDefinitionManager.ValidateQueryAndHandlerSettings(input);
        }
    }
}