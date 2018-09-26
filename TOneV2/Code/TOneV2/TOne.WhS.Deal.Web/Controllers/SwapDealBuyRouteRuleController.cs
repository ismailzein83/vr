using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.Deal.Business;
using Vanrise.Web.Base;
using TOne.WhS.Deal.Entities;

namespace TOne.WhS.Deal.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "SwapDealBuyRouteRule")]
    public class SwapDealBuyRouteRuleController :  Vanrise.Web.Base.BaseAPIController
    {
        SwapDealBuyRouteRuleManager _manager = new SwapDealBuyRouteRuleManager();

        [HttpPost]
        [Route("GetFilteredSwapDealBuyRouteRules")]
        public object GetFilteredSwapDealBuyRouteRules(Vanrise.Entities.DataRetrievalInput<SwapDealBuyRouteRuleQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredSwapDealBuyRouteRules(input), "Swap Deal Buy Route Rules");
        }

        [HttpGet]
        [Route("GetSwapDealBuyRouteRule")]
        public SwapDealBuyRouteRule GetSwapDealBuyRouteRule(long swapDealBuyRouteRuleId)
        {
            return _manager.GetVRRule(swapDealBuyRouteRuleId);
        }

        [HttpPost]
        [Route("AddSwapDealBuyRouteRule")]
        public Vanrise.Entities.InsertOperationOutput<SwapDealBuyRouteRuleDetails> AddSwapDealBuyRouteRule(SwapDealBuyRouteRule swapDealBuyRouteRule)
        {
            return _manager.AddVRRule(swapDealBuyRouteRule);
        }

        [HttpPost]
        [Route("UpdateSwapDealBuyRouteRule")]
        public Vanrise.Entities.UpdateOperationOutput<SwapDealBuyRouteRuleDetails> UpdateSwapDealBuyRouteRule(SwapDealBuyRouteRule swapDealBuyRouteRule)
        {
            return _manager.UpdateVRRule(swapDealBuyRouteRule);
        }

        [HttpPost]
        [Route("GetSwapDealBuyRouteRuleExtendedSettingsConfigs")]
        public IEnumerable<SwapDealBuyRouteRuleExtendedSettingsConfig> GetSwapDealBuyRouteRuleExtendedSettingsConfigs()
        {
            return _manager.GetSwapDealBuyRouteRuleExtendedSettingsConfigs();
        }
    }
}