using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Entities;
using Vanrise.Web.Base;
using System;

namespace  Vanrise.Fzero.FraudAnalysis.Web.Controllers
{
    public class SuspicionAnalysisController : BaseAPIController
    {

        [HttpGet]
        public IEnumerable<CDR> GetNormalCDRs(int fromRow, int toRow, DateTime fromDate, DateTime toDate, string msisdn)
        {
            StrategyManager manager = new StrategyManager();

            return manager.GetNormalCDRs(fromRow, toRow, fromDate, toDate, msisdn);
        }

        [HttpGet]
        public IEnumerable<NumberProfile> GetNumberProfiles(int fromRow, int toRow, DateTime fromDate, DateTime toDate, string subscriberNumber)
        {
            StrategyManager manager = new StrategyManager();

            return manager.GetNumberProfiles(fromRow, toRow, fromDate, toDate, subscriberNumber);
        }


        [HttpGet]
        public IEnumerable<FraudResult> GetFilteredSuspiciousNumbers(int fromRow, int toRow, DateTime fromDate, DateTime toDate, int? strategyId, string suspicionLevelsList)
        {
            FraudManager manager = new FraudManager(null);

            return manager.GetFilteredSuspiciousNumbers("FraudResult", fromRow, toRow, fromDate, toDate, strategyId, suspicionLevelsList);
        }



    }
}