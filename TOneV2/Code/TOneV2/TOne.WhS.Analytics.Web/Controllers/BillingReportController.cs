using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.Analytics.Business;
using TOne.WhS.Analytics.Entities;
using TOne.WhS.Analytics.Entities.BillingReport;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.Analytics.Web.Controllers
{
    [JSONWithType]
    [RoutePrefix(Constants.ROUTE_PREFIX + "BillingReport")]
    public class BillingReportController : BaseAPIController
    {
        BillingReportManager _manager = new BillingReportManager();
        ReportDefinitionManager _reportDefinitionManager = new ReportDefinitionManager();

        [HttpPost]
        [Route("ExportCarrierProfile")]
        public object ExportCarrierProfile(BusinessCaseStatusQuery input)
        {
            return GetExcelResponse(_manager.ExportCarrierProfile(input), _reportDefinitionManager.GetRdlcDownloadedFileName("BusinessCase Status", input.fromDate, input.toDate));
        }
    }
}