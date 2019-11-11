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
        [Route("TryCompileChargeTypeCustomCodePriceLogic")]
        public RetailBillingCompilationOutput TryCompileChargeTypeCustomCodePriceLogic(TryCompileChargeCustomCodePriceInput input)
        {
            var result = new RetailBillingCustomCodeChargeTypeManager().TryCompileChargeTypeCustomCodePriceLogic(input.TargetRecordTypeId, input.ChargeSettingsRecordTypeId, input.PricingLogic, out List<String> errorMessages);
            return new RetailBillingCompilationOutput()
            {
                Result = result,
                ErrorMessages = errorMessages
            };
        }

        [HttpPost]
        [Route("TryCompileChargeTypeCustomCodeDescriptionLogic")]
        public RetailBillingCompilationOutput TryCompileChargeTypeCustomCodeDescriptionLogic(TryCompileChargeCustomCodeDescriptionInput input)
        {
            var result = new RetailBillingCustomCodeChargeTypeManager().TryCompileChargeTypeCustomCodeDescriptionLogic(input.ChargeSettingsRecordTypeId, input.DescriptionLogic, out List<String> errorMessages);
            return new RetailBillingCompilationOutput()
            {
                Result = result,
                ErrorMessages = errorMessages
            };
        }
    }

    public class TryCompileChargeCustomCodePriceInput
    {
        public Guid? TargetRecordTypeId { get; set; }
        public Guid? ChargeSettingsRecordTypeId { get; set; }
        public string PricingLogic { get; set; }
    }
    public class TryCompileChargeCustomCodeDescriptionInput
    {
        public Guid? ChargeSettingsRecordTypeId { get; set; }
        public string DescriptionLogic { get; set; }
    }
}