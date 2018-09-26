using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.Analytics.Business;
using TOne.WhS.Analytics.Entities;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.Analytics.Web.Controllers
{
    [JSONWithType]
    [RoutePrefix(Constants.ROUTE_PREFIX + "VariationReport")]
    public class VariationReportController : BaseAPIController
    {
        VariationReportManager _manager = new VariationReportManager();

        [HttpPost]
        [Route("GetFilteredVariationReportRecords")]
        public object GetFilteredVariationReportRecords(DataRetrievalInput<VariationReportQuery> input)
        {
            if (!_manager.DoesUserHaveVariationReportViewAccess(input.Query.ReportType))
                return GetUnauthorizedResponse();
            return GetWebResponse(input, _manager.GetFilteredVariationReportRecords(input), "Variation Report");
        }
    }
}