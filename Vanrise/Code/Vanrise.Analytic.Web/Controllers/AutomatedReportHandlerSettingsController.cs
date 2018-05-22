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
    [RoutePrefix(Constants.ROUTE_PREFIX + "AutomatedReportHandlerSettings")]
    [JSONWithTypeAttribute]
    public class AutomatedReportHandlerSettingsController : BaseAPIController
    {

        [HttpGet]
        [Route("GetAutomatedReportHandlerTemplateConfigs")]
        public IEnumerable<VRAutomatedReportHandlerSettingsConfig> GetAutomatedReportTemplateHandlerConfigs()
        {
            VRAutomatedReportHandlerManager vrAutomatedReportHandlerManager = new VRAutomatedReportHandlerManager();
            return vrAutomatedReportHandlerManager.GetAutomatedReportHandlerTemplateConfigs();
        }
    }
}