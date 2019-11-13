//using Retail.Billing.Business;
//using Retail.Billing.Entities;
//using System;
//using System.Collections.Generic;
//using System.Web.Http;
//using Vanrise.Web.Base;

//namespace Retail.Billing.Web.Controllers
//{
//    [RoutePrefix(Constants.ROUTE_PREFIX + "RetailBillingCharge")]
//    [JSONWithTypeAttribute]
//    public class RetailBillingChargeController : BaseAPIController
//    {
//        [HttpPost]
//        [Route("EvaluateRetailBillingCharge")]
//        public Decimal? EvaluateRetailBillingCharge(EvaluateRetailBillingChargeInput input)
//        {
//            return new RetailBillingChargeManager().EvaluateRetailBillingCharge(input.Charge, input.TargetFieldValues);
//        }
       
//    }

//    public class EvaluateRetailBillingChargeInput
//    {
//        public RetailBillingCharge Charge { get; set; }
//        public Dictionary<string, Object> TargetFieldValues { get; set; }
//    }

//    public class GetChargeDescriptionInput
//    {
//        public RetailBillingCharge Charge { get; set; }
//    }
//}