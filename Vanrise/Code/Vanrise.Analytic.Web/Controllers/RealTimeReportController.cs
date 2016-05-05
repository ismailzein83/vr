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
    [RoutePrefix(Constants.ROUTE_PREFIX + "RealTimeReport")]
    public class RealTimeReportController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpGet]
        [Route("GetRealTimeReportsInfo")]
        public IEnumerable<RealTimeReportInfo> GetRealTimeReportsInfo(string filter = null)
        {
            RealTimeReportManager manager = new RealTimeReportManager();
            RealTimeReportInfoFilter serializedFilter = filter != null ? Vanrise.Common.Serializer.Deserialize<RealTimeReportInfoFilter>(filter) : null;
            return manager.GetRealTimeReportsInfo(serializedFilter);
        }
        [HttpGet]
        [Route("GetRealTimeReportById")]
        public RealTimeReport GetRealTimeReportById(int realTimeReportId)
        {
            RealTimeReportManager manager = new RealTimeReportManager();
            return manager.GetRealTimeReportById(realTimeReportId);
        }
        [HttpPost]
        [Route("UpdateRealTimeReport")]
        public Vanrise.Entities.UpdateOperationOutput<RealTimeReport> UpdateRealTimeReport(RealTimeReport realTimeReport)
        {
            RealTimeReportManager manager = new RealTimeReportManager();
            return manager.UpdateRealTimeReport(realTimeReport);
        }
        [HttpPost]
        [Route("AddRealTimeReport")]
        public Vanrise.Entities.InsertOperationOutput<RealTimeReport> AddRealTimeReport(RealTimeReport realTimeReport)
        {
            RealTimeReportManager manager = new RealTimeReportManager();
            return manager.AddRealTimeReport(realTimeReport);
        }
    }
}