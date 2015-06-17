using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Web.Base;
using System;

namespace  Vanrise.Fzero.FraudAnalysis.Web.Controllers
{

    public class FraudResultController : BaseAPIController
    {
        [HttpGet]
        public IEnumerable<FraudResult> GetFilteredSuspiciousNumbers(int fromRow, int toRow,DateTime fromDate, DateTime toDate, int strategyId, string suspicionList)
        {
            StrategyManager manager = new StrategyManager();

            return manager.GetFilteredSuspiciousNumbers(fromRow, toRow, fromDate, toDate, strategyId, suspicionList);
        }

    }
}