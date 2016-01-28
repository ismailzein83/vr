using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Web.Base;
using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.Aggregates;

namespace  Vanrise.Fzero.FraudAnalysis.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "Strategy")]
    public class StrategyController : BaseAPIController
    {
        StrategyManager _manager;
        public StrategyController()
        {
            _manager = new StrategyManager();
        }

        [HttpPost]
        [Route("GetFilteredStrategies")]
        public object GetFilteredStrategies(Vanrise.Entities.DataRetrievalInput<StrategyQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredStrategies(input));
        }

        [HttpGet]
        [Route("GetStrategies")]
        public IEnumerable<StrategyInfo> GetStrategies(int PeriodId, bool? IsEnabled)
        {
            return _manager.GetStrategiesInfo(PeriodId, IsEnabled);
        }

        [HttpGet]
        [Route("GetStrategy")]
        public Strategy GetStrategy(int StrategyId)
        {
            return _manager.GetStrategy(StrategyId);
        }

        [HttpGet]
        [Route("GetFilters")]
        public List<FilterDefinitionInfo> GetFilters()
        {
            FilterManager manager = new FilterManager();
            return manager.GetCriteriaNames();
        }

        [HttpGet]
        [Route("GetAggregates")]
        public List<AggregateDefinitionInfo> GetAggregates()
        {
            AggregateManager manager = new AggregateManager();
            return manager.GetAggregateDefinitionsInfo();
        }

        [HttpPost]
        [Route("AddStrategy")]
        public InsertOperationOutput<StrategyDetail> AddStrategy(Strategy strategyObject)
        {
            return _manager.AddStrategy(strategyObject);
        }

        [HttpPost]
        [Route("UpdateStrategy")]
        public UpdateOperationOutput<StrategyDetail> UpdateStrategy(Strategy strategyObject)
        {
            return _manager.UpdateStrategy(strategyObject);
        }
    }
}