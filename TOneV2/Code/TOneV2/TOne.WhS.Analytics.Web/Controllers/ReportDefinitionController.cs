using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.Analytics.Business;
using TOne.WhS.Analytics.Entities.BillingReport;

namespace TOne.WhS.Analytics.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "ReportDefinition")]
    public class ReportDefinitionController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpGet]
        [Route("GetAllRDLCReportDefinition")]
        public List<RDLCReportDefinition> GetAllRDLCReportDefinition()
        {
            ReportDefinitionManager manager = new ReportDefinitionManager();
            return manager.GetAllRDLCReportDefinition();
        }
    }
}