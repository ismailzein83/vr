using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Web.Base;

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
        public Dictionary<int, string> GetFilters()
        {
            CriteriaManager manager = new CriteriaManager();

            return manager.GetCriteriaNames();
        }


        [HttpGet]
        public List<string> GetAggregates()
        {
            AggregateManager manager = new AggregateManager();
            return manager.GetAggregateNames();
        }


        [HttpGet]
        public List<Period> GetPeriods()
        {
            PredefinedManager manager = new PredefinedManager();

            return manager.GetPeriods();
        }


    }
}