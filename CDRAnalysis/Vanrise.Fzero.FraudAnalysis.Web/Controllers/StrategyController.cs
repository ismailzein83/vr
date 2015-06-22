using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Web.Base;
using System;

namespace  Vanrise.Fzero.FraudAnalysis.Web.Controllers
{

    public class StrategyController : BaseAPIController
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
        public IEnumerable<FraudResult> GetFilteredSuspiciousNumbers(int fromRow, int toRow, DateTime fromDate, DateTime toDate, int strategyId, string suspicionLevelsList)
        {
            StrategyManager manager = new StrategyManager();

            return manager.GetFilteredSuspiciousNumbers( fromRow, toRow, fromDate, toDate, strategyId, suspicionLevelsList);
        }

        [HttpGet]

        public IEnumerable<Strategy> GetAllStrategies()
        {
            StrategyManager manager = new StrategyManager();

            return manager.GetAllStrategies();
        }

        [HttpGet]
        public IEnumerable<Strategy> GetFilteredStrategies(int fromRow, int toRow,string name, string description)
        {
            StrategyManager manager = new StrategyManager();

            return manager.GetFilteredStrategies(fromRow, toRow, name, description);
        }

        [HttpGet]
        public Strategy GetStrategy(int StrategyId)
        {
            StrategyManager manager = new StrategyManager();

            return manager.GetStrategy(StrategyId);
        }


        [HttpPost]
        public Vanrise.Fzero.FraudAnalysis.Entities.UpdateOperationOutput<Strategy> UpdateStrategy(Strategy strategyObject)
        {
            StrategyManager manager = new StrategyManager();

            return manager.UpdateStrategy(strategyObject);
        }

        [HttpPost]
        public Vanrise.Fzero.FraudAnalysis.Entities.InsertOperationOutput<Strategy> AddStrategy(Strategy strategyObject)
        {
            StrategyManager manager = new StrategyManager();

            return manager.AddStrategy(strategyObject);
        }


        [HttpGet]
        public Dictionary<int, CriteriaDefinition> GetFilters()
        {
            CriteriaManager manager = new CriteriaManager();

            return manager.GetCriteriaDefinitions();
        }


        [HttpGet]
        public List<Period> GetPeriods()
        {
            StrategyManager manager = new StrategyManager();

            return manager.GetPeriods();
        }


    }
}