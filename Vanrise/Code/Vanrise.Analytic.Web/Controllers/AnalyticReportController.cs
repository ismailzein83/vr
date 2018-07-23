using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Analytic.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "AnalyticReport")]
    public class AnalyticReportController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpGet]
        [Route("GetAnalyticReportsInfo")]
        public IEnumerable<AnalyticReportInfo> GetAnalyticReportsInfo(string filter = null)
        {
            AnalyticReportManager manager = new AnalyticReportManager();
            AnalyticReportInfoFilter serializedFilter = filter != null ? Vanrise.Common.Serializer.Deserialize<AnalyticReportInfoFilter>(filter) : null;
            return manager.GetAnalyticReportsInfo(serializedFilter);
        }
        [HttpGet]
        [Route("GetAnalyticReportById")]
        public AnalyticReport GetAnalyticReportById(Guid analyticReportId)
        {
            AnalyticReportManager manager = new AnalyticReportManager();
            return manager.GetAnalyticReportById(analyticReportId);
        }
        [HttpPost]
        [Route("UpdateAnalyticReport")]
        public Vanrise.Entities.UpdateOperationOutput<AnalyticReportDetail> UpdateAnalyticReport(AnalyticReport analyticReport)
        {
            AnalyticReportManager manager = new AnalyticReportManager();
            return manager.UpdateAnalyticReport(analyticReport);
        }
        [HttpPost]
        [Route("AddAnalyticReport")]
        public Vanrise.Entities.InsertOperationOutput<AnalyticReportDetail> AddAnalyticReport(AnalyticReport analyticReport)
        {
            AnalyticReportManager manager = new AnalyticReportManager();
            return manager.AddAnalyticReport(analyticReport);
        }
        [HttpPost]
        [Route("GetFilteredAnalyticReports")]
        public object GetFilteredAnalyticReports(Vanrise.Entities.DataRetrievalInput<AnalyticReportQuery> input)
        {
            AnalyticReportManager manager = new AnalyticReportManager();
            return GetWebResponse(input, manager.GetFilteredAnalyticReports(input));
        }
        [HttpGet]
        [Route("GetAnalyticReportConfigTypes")]
        public IEnumerable<AnalyticReportConfiguration> GetAnalyticReportConfigTypes()
        {
            AnalyticReportManager manager = new AnalyticReportManager();
            return manager.GetAnalyticReportConfigTypes();
        }

        [HttpGet]
        [Route("CheckRecordStoragesAccess")]
        public IEnumerable<string> CheckRecordStoragesAccess(Guid analyticReportId)
        {
            AnalyticReportManager manager = new AnalyticReportManager();
            return manager.CheckRecordStoragesAccess(analyticReportId);
        }
    }
}