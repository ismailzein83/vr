using System;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Web.Base;

namespace  Vanrise.Fzero.FraudAnalysis.Web.Controllers
{

    public class DashboardController : BaseAPIController
    {

        [HttpGet]
        public IEnumerable<StrategyCases> GetStrategyCases(DateTime fromDate, DateTime toDate)
        {
            DashboardManager manager = new DashboardManager();
            return manager.GetStrategyCases(fromDate, toDate);
        }

        [HttpPost]
        public object GetCasesSummary(Vanrise.Entities.DataRetrievalInput<DashboardResultQuery> input)
        {
            DashboardManager manager = new DashboardManager();
            return GetWebResponse(input, manager.GetCasesSummary(input));
        }

        [HttpPost]
        public object GetBTSCases(Vanrise.Entities.DataRetrievalInput<DashboardResultQuery> input)
        {
            DashboardManager manager = new DashboardManager();
            return GetWebResponse(input, manager.GetBTSCases(input));
        }

        [HttpPost]
        public object GetTop10BTSHighValue(Vanrise.Entities.DataRetrievalInput<DashboardResultQuery> input)
        {
            DashboardManager manager = new DashboardManager();
            return GetWebResponse(input, manager.GetTop10BTSHighValue(input));
        }

        [HttpPost]
        public object GetDailyVolumeLooses(Vanrise.Entities.DataRetrievalInput<DashboardResultQuery> input)
        {
            DashboardManager manager = new DashboardManager();
            return GetWebResponse(input, manager.GetDailyVolumeLooses(input));
        }



    }
}