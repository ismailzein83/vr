using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.Aggregates;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Web.Base;

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
        [Route("GetStrategiesInfo")]
        public IEnumerable<StrategyInfo> GetStrategiesInfo(string filter = null)
        {
            StrategyInfoFilter deserializedFilter = filter != null ? Vanrise.Common.Serializer.Deserialize<StrategyInfoFilter>(filter) : null;
            return _manager.GetStrategiesInfo(deserializedFilter);
        }

        [HttpGet]
        [Route("GetStrategy")]
        public Strategy GetStrategy(int StrategyId)
        {
            return _manager.GetStrategy(StrategyId);
        }

        [HttpGet]
        [Route("GetFilters")]
        public List<FilterInfo> GetFilters(string filter = null)
        {
            FilterForFilter deserializedFilter = filter != null ? Vanrise.Common.Serializer.Deserialize<FilterForFilter>(filter) : null;
            FilterManager manager = new FilterManager();
            return manager.GetCriteriaNames(deserializedFilter);
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
        [HttpGet]
        [Route("GetStrategyCriteriaTemplateConfigs")]
        public IEnumerable<StrategyCriteriaConfig> GetStrategyCriteriaTemplateConfigs()
        {
            return _manager.GetStrategyCriteriaTemplateConfigs();
        }
    }
}