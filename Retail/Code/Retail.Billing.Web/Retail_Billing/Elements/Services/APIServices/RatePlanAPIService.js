//(function (appControllers) {

//    "use strict";
//    chargeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_Billing_ModuleConfig'];

//    function chargeAPIService(BaseAPIService, UtilsService, Retail_Billing_ModuleConfig) {

//        var controllerName = "RetailBillingRatePlan";

//        function EvaluateRecurringCharge(input) {
//            return BaseAPIService.post(UtilsService.getServiceURL(Retail_Billing_ModuleConfig.moduleName, controllerName, "EvaluateRecurringCharge"), input);
//        }
//        function EvaluateActionCharge(input) {
//            return BaseAPIService.post(UtilsService.getServiceURL(Retail_Billing_ModuleConfig.moduleName, controllerName, "EvaluateActionCharge"), input);
//        }
//        return ({
//            EvaluateRecurringCharge: EvaluateRecurringCharge,
//            EvaluateActionCharge: EvaluateActionCharge
//        });
//    }

//    appControllers.service('Retail_Billing_RatePlanAPIService', chargeAPIService);

//})(appControllers);