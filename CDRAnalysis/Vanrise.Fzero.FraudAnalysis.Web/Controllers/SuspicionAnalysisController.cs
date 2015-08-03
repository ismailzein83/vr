﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Web.Base;

namespace  Vanrise.Fzero.FraudAnalysis.Web.Controllers
{
    public class SuspicionAnalysisController : BaseAPIController
    {

        [HttpPost]
        public object GetSubscriberThresholds(Vanrise.Entities.DataRetrievalInput<SubscriberThresholdResultQuery> input)
        {
            StrategyManager manager = new StrategyManager();
            return GetWebResponse(input, manager.GetSubscriberThresholds(input));
        }



        [HttpPost]
        public object GetFilteredSuspiciousNumbers(Vanrise.Entities.DataRetrievalInput<FraudResultQuery> input)
        {
            FraudManager manager = new FraudManager();
            return GetWebResponse(input, manager.GetFilteredSuspiciousNumbers(input));
        }




        [HttpGet]
        public FraudResult GetFraudResult(DateTime fromDate, DateTime toDate, string strategiesList, string suspicionLevelsList, string subscriberNumber)
        {
            FraudManager manager = new FraudManager();

            List<int> strategiesIntList = new List<int>();
            if (strategiesList != null)
                strategiesIntList = strategiesList.Split(',').Select(h => int.Parse(h)).ToList();



            List<int> suspicionLevelsIntList = new List<int>();
            if (suspicionLevelsList != null)
                suspicionLevelsIntList = suspicionLevelsList.Split(',').Select(h => int.Parse(h)).ToList();



            return manager.GetFraudResult(fromDate, toDate, strategiesIntList, suspicionLevelsIntList, subscriberNumber);
        }

        


    }
}