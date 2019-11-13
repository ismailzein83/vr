//(function (appControllers) {

//    "use strict";
//    chargeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_Billing_ModuleConfig'];

//    function chargeAPIService(BaseAPIService, UtilsService, Retail_Billing_ModuleConfig) {

//        var controllerName = "RetailBillingCharge";

//        function CalculateRetailBillingCharge(input) {
//            return BaseAPIService.post(UtilsService.getServiceURL(Retail_Billing_ModuleConfig.moduleName, controllerName, "CalculateRetailBillingCharge"), input);
//        }

//        return ({
//            CalculateRetailBillingCharge: CalculateRetailBillingCharge
//        });
//    }

//    appControllers.service('Retail_Billing_ChargeAPIService', chargeAPIService);

//})(appControllers);