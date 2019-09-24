using Retail.Billing.Business;
using Retail.Billing.Entities;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Web.Base;

namespace Retail.Billing.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "ChargeType")]
    [JSONWithTypeAttribute]
    public class RetailBillingChargeTypeController : BaseAPIController
    {
        RetailBillingChargeTypeManager manager = new RetailBillingChargeTypeManager();

        [HttpGet]
        [Route("GetChargeTypeExtendedSettingsConfigs")]
        public IEnumerable<RetailBillingChargeTypeExtendedSettingsConfig> GetChargeTypeExtendedSettingsConfigs()
        {
            return manager.GetChargeTypeExtendedSettingsConfigs();
        }
        
    }
}