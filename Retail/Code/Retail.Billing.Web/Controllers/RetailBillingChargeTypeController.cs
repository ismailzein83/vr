using Retail.Billing.Business;
using Retail.Billing.Entities;
using System;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Web.Base;

namespace Retail.Billing.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "RetailBillingChargeType")]
    [JSONWithTypeAttribute]
    public class RetailBillingChargeTypeController : BaseAPIController
    {
        [HttpGet]
        [Route("GetRetailBillingChargeTypeInfo")]
        public IEnumerable<RetailBillingChargeTypeInfo> GetRetailBillingChargeTypeInfo()
        {
            return new RetailBillingChargeTypeManager().GetRetailBillingChargeTypeInfo();
        }

        [HttpGet]
        [Route("GetRetailBillingChargeType")]
        public RetailBillingChargeType GetRetailBillingChargeType(Guid retailBillingChargeTypeId)
        {
            return new RetailBillingChargeTypeManager().GetRetailBillingChargeType(retailBillingChargeTypeId);
        }

        [HttpGet]
        [Route("GetChargeTypeExtendedSettingsConfigs")]
        public IEnumerable<RetailBillingChargeTypeExtendedSettingsConfig> GetChargeTypeExtendedSettingsConfigs()
        {
            return new RetailBillingChargeTypeManager().GetChargeTypeExtendedSettingsConfigs();
        }
    }
}