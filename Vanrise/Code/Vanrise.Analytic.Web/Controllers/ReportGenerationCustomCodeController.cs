using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Analytic.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "ReportGenerationCustomCode")]
    public class ReportGenerationCustomCodeController : BaseAPIController
    {
        ReportGenerationCustomCodeManager _manager = new ReportGenerationCustomCodeManager();

        [HttpGet]
        [Route("GetReportGenerationCustomCodeSettingsInfo")]
        public IEnumerable<ReportGenerationCustomCodeDefinitionInfo> GetReportGenerationCustomCodeSettingsInfo()
        {
            return _manager.GetReportGenerationCustomCodeSettingsInfo();
        }
    }
}