using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;

namespace Retail.BusinessEntity.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "ChargingPolicy")]
    public class ChargingPolicyController : BaseAPIController
    {
        ChargingPolicyManager _manager = new ChargingPolicyManager();

        [HttpPost]
        [Route("GetFilteredChargingPolicies")]
        public object GetFilteredChargingPolicies(Vanrise.Entities.DataRetrievalInput<ChargingPolicyQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredChargingPolicies(input));
        }

        [HttpGet]
        [Route("GetChargingPolicy")]
        public ChargingPolicy GetChargingPolicy(int chargingPolicyId)
        {
            return _manager.GetChargingPolicy(chargingPolicyId);
        }

        [HttpPost]
        [Route("AddChargingPolicy")]
        public Vanrise.Entities.InsertOperationOutput<ChargingPolicyDetail> AddChargingPolicy(ChargingPolicy chargingPolicy)
        {
            return _manager.AddChargingPolicy(chargingPolicy);
        }

        [HttpPost]
        [Route("UpdateChargingPolicy")]
        public Vanrise.Entities.UpdateOperationOutput<ChargingPolicyDetail> UpdateChargingPolicy(ChargingPolicyToEdit chargingPolicy)
        {
            return _manager.UpdateChargingPolicy(chargingPolicy);
        }
    }
}