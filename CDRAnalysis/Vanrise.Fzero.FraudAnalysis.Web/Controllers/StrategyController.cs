﻿using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Entities;
using Vanrise.Web.Base;
using System;

namespace  Vanrise.Fzero.FraudAnalysis.Web.Controllers
{

    public class StrategyController : BaseAPIController
    {

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