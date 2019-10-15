using Retail.Billing.Business;
using Retail.Billing.Entities;
using System;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Web.Base;

namespace Retail.Billing.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "RetailBillingCustomCodeChargeType")]
    [JSONWithTypeAttribute]
    public class RetailBillingCustomCodeChargeTypeController : BaseAPIController
    {
        [HttpPost]
        [Route("TryCompileChargeTypeCustomCode")]
        public RetailBillingCompilationOutput TryCompileChargeTypeCustomCode(TryCompileChargeCustomCodeInput input)
        {
            var result = new RetailBillingCustomCodeChargeTypeManager().TryCompileChargeTypeCustomCode(input.TargetRecordTypeId, input.ChargeSettingsRecordTypeId, input.PricingLogic, out List<String> errorMessages);
            return new RetailBillingCompilationOutput()
            {
                Result = result,
                ErrorMessages = errorMessages
            };
        }
    }

    public class TryCompileChargeCustomCodeInput
    {
        public Guid? TargetRecordTypeId { get; set; }
        public Guid? ChargeSettingsRecordTypeId { get; set; }
        public string PricingLogic { get; set; }
    }
}