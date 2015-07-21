using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Entities;
using Vanrise.Web.Base;
using System;
using System.Linq;

namespace  Vanrise.Fzero.FraudAnalysis.Web.Controllers
{
    public class SuspicionAnalysisController : BaseAPIController
    {

        [HttpGet]
        public IEnumerable<SubscriberThreshold> GetSubscriberThresholds(int fromRow, int toRow, DateTime fromDate, DateTime toDate, string msisdn)
        {
            StrategyManager manager = new StrategyManager();

            return manager.GetSubscriberThresholds(fromRow, toRow, fromDate, toDate, msisdn);
        }


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
        public IEnumerable<FraudResult> GetFilteredSuspiciousNumbers(string tempTableKey, int fromRow, int toRow, DateTime fromDate, DateTime toDate, string strategiesList, string suspicionLevelsList)
        {
            FraudManager manager = new FraudManager();

            List<int> strategiesIntList = new List<int>();
            if (strategiesList!=null)
                strategiesIntList =strategiesList.Split(',').Select(h => int.Parse(h)).ToList();



            List<int> suspicionLevelsIntList = new List<int>();
            if (suspicionLevelsList != null)
                suspicionLevelsIntList = suspicionLevelsList.Split(',').Select(h => int.Parse(h)).ToList();



            return manager.GetFilteredSuspiciousNumbers( tempTableKey, fromRow, toRow, fromDate, toDate, strategiesIntList, suspicionLevelsIntList);
        }



    }
}