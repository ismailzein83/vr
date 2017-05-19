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
    //[JSONWithTypeAttribute]
    //[RoutePrefix(Constants.ROUTE_PREFIX + "SwapDealBuyRouteRule")]
    public class SwapDealBuyRouteRuleController :  Vanrise.Web.Base.BaseAPIController
    {
        //SwapDealBuyRouteRuleManager _manager = new SwapDealBuyRouteRuleManager();

        //[HttpPost]
        //[Route("GetFilteredSwapDealBuyRouteRules")]
        //public object GetFilteredSwapDealBuyRouteRules(Vanrise.Entities.DataRetrievalInput<SwapDealBuyRouteRuleQuery> input) 
        //{
        //    return GetWebResponse(input, _manager.GetFilteredSwapDealBuyRouteRules(input));
        //}

        //[HttpGet]
        //[Route("GetVRRule")]
        //public SwapDealBuyRouteRule GetVRRule(long vrRuleId)
        //{
        //    return _manager.GetVRRule(vrRuleId);
        //}

        //[HttpPost]
        //[Route("AddVRRule")]
        //public Vanrise.Entities.InsertOperationOutput<SwapDealBuyRouteRuleDetails> AddVRRule(SwapDealBuyRouteRule swapDealBuyRouteRule)
        //{
        //    return _manager.AddVRRule(swapDealBuyRouteRule);
        //}

        //[HttpPost]
        //[Route("UpdateVRRule")]
        //public Vanrise.Entities.UpdateOperationOutput<SwapDealBuyRouteRuleDetails> UpdateVRRule(SwapDealBuyRouteRule swapDealBuyRouteRule)
        //{
        //    return _manager.UpdateVRRule(swapDealBuyRouteRule);
        //}
    }
}