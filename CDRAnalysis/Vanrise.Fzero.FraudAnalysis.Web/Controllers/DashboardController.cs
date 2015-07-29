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
        public IEnumerable<CasesSummary> GetCasesSummary(DateTime fromDate, DateTime toDate)
        {
            DashboardManager manager = new DashboardManager();
            return manager.GetCasesSummary(fromDate, toDate);
        }

        [HttpGet]
        public IEnumerable<StrategyCases> GetStrategyCases(DateTime fromDate, DateTime toDate)
        {
            DashboardManager manager = new DashboardManager();
            return manager.GetStrategyCases(fromDate, toDate);
        }

        [HttpGet]
        public IEnumerable<BTSCases> GetBTSCases(DateTime fromDate, DateTime toDate)
        {
            DashboardManager manager = new DashboardManager();
            return manager.GetBTSCases(fromDate, toDate);
        }

        [HttpGet]
        public IEnumerable<CellCases> GetCellCases(DateTime fromDate, DateTime toDate)
        {
            DashboardManager manager = new DashboardManager();
            return manager.GetCellCases(fromDate, toDate);
        }



    }
}