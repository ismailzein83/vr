(function (appControllers) {

    "use strict";
    chargeTypeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_Billing_ModuleConfig'];

    function chargeTypeAPIService(BaseAPIService, UtilsService, Retail_Billing_ModuleConfig) {

        var controllerName = "RetailBillingCustomCodeChargeType";

        function TryCompileChargeTypeCustomCode(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_Billing_ModuleConfig.moduleName, controllerName, "TryCompileChargeTypeCustomCode"), input);
        }

        return ({
            TryCompileChargeTypeCustomCode: TryCompileChargeTypeCustomCode
        });
    }
    appControllers.service('Retail_Billing_CustomCodeChargeTypeAPIService', chargeTypeAPIService);

})(appControllers);