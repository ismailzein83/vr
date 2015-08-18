using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.Analytics.Business;
using TOne.Analytics.Entities;

namespace TOne.Analytics.Web.Controllers
{
    public class HourlyReportController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpPost]
        public object GetHourlyReportData(Vanrise.Entities.DataRetrievalInput<HourlyReportInput> input)
        {
            HourlyReportManager manager = new HourlyReportManager();
            return GetWebResponse(input, manager.GetHourlyReportData(input));

        }
        [HttpGet]
        public IEnumerable<HourlyReport> GetHourlyReport(TrafficStatisticGroupKeys filterByColumn, string columnFilterValue, DateTime from, DateTime to)
        {
            HourlyReportManager manager = new HourlyReportManager();
            return manager.GetHourlyReport(filterByColumn, columnFilterValue, from, to);
        }
    }
}