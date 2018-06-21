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
    [RoutePrefix(Constants.ROUTE_PREFIX + "AutomatedReportHandler")]
    [JSONWithTypeAttribute]
    public class AutomatedReportHandlerController : BaseAPIController
    {
        [HttpGet]
        [Route("GetFileGeneratorTemplateConfigs")]
        public IEnumerable<VRAutomatedReportFileGeneratorConfig> GetFileGeneratorTemplateConfigs()
        {
            VRAutomatedReportFileGeneratorManager fileGeneratorManager = new VRAutomatedReportFileGeneratorManager();
            return fileGeneratorManager.GetFileGeneratorTemplateConfigs();
        }
    }
}