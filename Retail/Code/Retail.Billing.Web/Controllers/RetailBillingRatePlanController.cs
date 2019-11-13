//using Retail.Billing.Business;
//using Retail.Billing.Entities;
//using System;
//using System.Collections.Generic;
//using System.Web.Http;
//using Vanrise.Web.Base;

//namespace Retail.Billing.Web.Controllers
//{
//    [RoutePrefix(Constants.ROUTE_PREFIX + "RetailBillingRatePlan")]
//    [JSONWithTypeAttribute]
//    public class RetailBillingRatePlanController : BaseAPIController
//    {
//        [HttpPost]
//        [Route("EvaluateRecurringCharge")]
//        public RatePlanEvaluateRecurringChargeOutput EvaluateRecurringCharge(RatePlanEvaluateRecurringChargeInput input)
//        {
//            return new RatePlanManager().EvaluateRecurringCharge(input);
//        }

//        [HttpPost]
//        [Route("EvaluateActionCharge")]
//        public RatePlanServiceActionChargeEvaluationOutput EvaluateActionCharge(RatePlanServiceActionChargeEvaluationInput input)
//        {
//            return new RatePlanManager().EvaluateActionCharge(input);
//        }
//    }
//}