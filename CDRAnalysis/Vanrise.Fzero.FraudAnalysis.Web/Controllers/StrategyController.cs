using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Web.Base;
using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.Aggregates;

namespace  Vanrise.Fzero.FraudAnalysis.Web.Controllers
{
    public class StrategyController : BaseAPIController
    {
        [HttpGet]
        public IEnumerable<StrategyInfo> GetStrategies(int PeriodId, bool? IsEnabled)
        {
            StrategyManager manager = new StrategyManager();

            return manager.GetStrategiesInfo(PeriodId, IsEnabled);
        }

        [HttpPost]
        public object GetFilteredStrategies(Vanrise.Entities.DataRetrievalInput<StrategyQuery> input)
        {

            StrategyManager manager = new StrategyManager();
                                   
            return GetWebResponse(input, manager.GetFilteredStrategies(input));
        }

        [HttpGet]
        public Strategy GetStrategy(int StrategyId)
        {
            StrategyManager manager = new StrategyManager();

            return manager.GetStrategyById(StrategyId);
        }

        [HttpPost]
        public UpdateOperationOutput<StrategyDetail> UpdateStrategy(Strategy strategyObject)
        {
            StrategyManager manager = new StrategyManager();

            return manager.UpdateStrategy(strategyObject);
        }

        [HttpPost]
        public InsertOperationOutput<StrategyDetail> AddStrategy(Strategy strategyObject)
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
    }
}