using System.Collections.Generic;
using System.Web.Http;
using TOne.WhS.Deal.Business;
using TOne.WhS.Deal.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.Deal.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "DealBuyRouteRule")]

    public class DealBuyRouteRuleController : Vanrise.Web.Base.BaseAPIController
    {
        DealBuyRouteRuleManager _manager = new DealBuyRouteRuleManager();

        [HttpPost]
        [Route("GetFilteredDealBuyRouteRules")]
        public object GetFilteredDealBuyRouteRules(Vanrise.Entities.DataRetrievalInput<DealBuyRouteRuleQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredDealBuyRouteRules(input), "Deal Buy Route Rules");
        }

        [HttpGet]
        [Route("GetDealBuyRouteRule")]
        public DealBuyRouteRule GetDealBuyRouteRule(long dealBuyRouteRuleId)
        {
            return _manager.GetVRRule(dealBuyRouteRuleId);
        }

        [HttpPost]
        [Route("AddDealBuyRouteRule")]
        public Vanrise.Entities.InsertOperationOutput<DealBuyRouteRuleDetails> AddDealBuyRouteRule(DealBuyRouteRule dealBuyRouteRule)
        {
            return _manager.AddVRRule(dealBuyRouteRule);
        }

        [HttpPost]
        [Route("UpdateDealBuyRouteRule")]
        public Vanrise.Entities.UpdateOperationOutput<DealBuyRouteRuleDetails> UpdateDealBuyRouteRule(DealBuyRouteRule dealBuyRouteRule)
        {
            return _manager.UpdateVRRule(dealBuyRouteRule);
        }

        [HttpPost]
        [Route("GetDealBuyRouteRuleExtendedSettingsConfigs")]
        public IEnumerable<DealBuyRouteRuleExtendedSettingsConfig> GetDealBuyRouteRuleExtendedSettingsConfigs()
        {
            return _manager.GetDealBuyRouteRuleExtendedSettingsConfigs();
        }

        [HttpGet]
        [Route("GetCarrierAccountId")]
        public int GetCarrierAccountId (int dealId)
        {
            return _manager.GetCarrierAccountId(dealId);
        }
    }
}