using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Web.Base;
using Vanrise.Entities;

namespace  Vanrise.Fzero.FraudAnalysis.Web.Controllers
{

    public class StrategyController : BaseAPIController
    {

        [HttpGet]

        public IEnumerable<Strategy> GetStrategies(int PeriodId, bool? IsEnabled)
        {
            StrategyManager manager = new StrategyManager();

            return manager.GetStrategies(PeriodId, IsEnabled);
        }


        [HttpPost]
        public object GetFilteredStrategies(Vanrise.Entities.DataRetrievalInput<StrategyResultQuery> input)
        {

            StrategyManager manager = new StrategyManager();
                                   
            return GetWebResponse(input, manager.GetFilteredStrategies(input));
        }



        [HttpGet]
        public Strategy GetStrategy(int StrategyId)
        {
            StrategyManager manager = new StrategyManager();

            return manager.GetStrategy(StrategyId);
        }


        [HttpPost]
        public UpdateOperationOutput<Strategy> UpdateStrategy(Strategy strategyObject)
        {
            StrategyManager manager = new StrategyManager();

            return manager.UpdateStrategy(strategyObject);
        }

        [HttpPost]
        public InsertOperationOutput<Strategy> AddStrategy(Strategy strategyObject)
        {
            StrategyManager manager = new StrategyManager();

            return manager.AddStrategy(strategyObject);
        }


        [HttpGet]
        public List<FilterDefinitionInfo> GetFilters()
        {
            FilterManager manager = new FilterManager();

            return manager.GetCriteriaNames();
        }


        [HttpGet]
        public List<AggregateDefinitionInfo> GetAggregates()
        {
            AggregateManager manager = new AggregateManager();

            return manager.GetAggregateDefinitionsInfo();
        }


        [HttpGet]
        public List<Period> GetPeriods()
        {
            PeriodManager manager = new PeriodManager();

            return manager.GetPeriods();
        }


    }
}