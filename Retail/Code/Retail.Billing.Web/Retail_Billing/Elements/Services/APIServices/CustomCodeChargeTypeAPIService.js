(function (appControllers) {

    "use strict";
    chargeTypeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_Billing_ModuleConfig'];

    function chargeTypeAPIService(BaseAPIService, UtilsService, Retail_Billing_ModuleConfig) {

        var controllerName = "RetailBillingCustomCodeChargeType";

        function TryCompileChargeTypeCustomCodePriceLogic(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_Billing_ModuleConfig.moduleName, controllerName, "TryCompileChargeTypeCustomCodePriceLogic"), input);
        }
        function TryCompileChargeTypeCustomCodeDescriptionLogic(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_Billing_ModuleConfig.moduleName, controllerName, "TryCompileChargeTypeCustomCodeDescriptionLogic"), input);
        }

        return ({
            TryCompileChargeTypeCustomCodePriceLogic: TryCompileChargeTypeCustomCodePriceLogic,
            TryCompileChargeTypeCustomCodeDescriptionLogic: TryCompileChargeTypeCustomCodeDescriptionLogic
        });
    }
    appControllers.service('Retail_Billing_CustomCodeChargeTypeAPIService', chargeTypeAPIService);

})(appControllers);