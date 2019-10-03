using Retail.Billing.Business;
using Retail.Billing.Entities;
using System;
using System.Web.Http;
using Vanrise.Web.Base;

namespace Retail.Billing.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "CustomCodeChargeType")]
    [JSONWithTypeAttribute]
    public class RetailBillingCustomCodeChargeTypeController : BaseAPIController
    {
        [HttpPost]
        [Route("TryCompileChargeCustomCode")]
        public RetailBillingCompilationOutput TryCompileChargeCustomCode(TryCompileChargeCustomCodeIput input)
        {
            return new RetailBillingCustomCodeChargeTypeManager().TryCompileChargeCustomCode(input.TargetRecordTypeId ,input.ChargeSettingsRecordTypeId,input.PricingLogic);
        }
    }

    public class TryCompileChargeCustomCodeIput
    {
        public Guid? TargetRecordTypeId { get; set; }
        public Guid? ChargeSettingsRecordTypeId { get; set; }
        public string PricingLogic { get; set; }
    }
}