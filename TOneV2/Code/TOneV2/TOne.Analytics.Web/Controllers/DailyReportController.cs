using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.Analytics.Business;
using TOne.Analytics.Entities;

namespace TOne.Analytics.Web.Controllers
{
    public class DailyReportController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpPost]
        public object GetFilteredDailyReportCalls(Vanrise.Entities.DataRetrievalInput<DailyReportQuery> input)
        {
            DailyReportManager manager = new DailyReportManager();
            return GetWebResponse(input, manager.GetFilteredDailyReportCalls(input));
        }
    }
}