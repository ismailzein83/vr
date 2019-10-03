(function (appControllers) {

    "use strict";
    chargeTypeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_Billing_ModuleConfig'];

    function chargeTypeAPIService(BaseAPIService, UtilsService, Retail_Billing_ModuleConfig) {

        var controllerName = "CustomCodeChargeType";

        function TryCompileChargeCustomCode(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_Billing_ModuleConfig.moduleName, controllerName, "TryCompileChargeCustomCode"), input);
        }

        return ({
            TryCompileChargeCustomCode: TryCompileChargeCustomCode
        });
    }

    appControllers.service('Retail_Billing_CustomCodeChargeTypeAPIService', chargeTypeAPIService);

})(appControllers);