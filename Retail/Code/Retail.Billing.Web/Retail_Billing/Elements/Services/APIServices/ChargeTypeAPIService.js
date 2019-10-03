(function (appControllers) {

    "use strict";
    chargeTypeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_Billing_ModuleConfig'];

    function chargeTypeAPIService(BaseAPIService, UtilsService, Retail_Billing_ModuleConfig) {

        var controllerName = "ChargeType";

        function GetChargeTypeExtendedSettingsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_Billing_ModuleConfig.moduleName, controllerName, "GetChargeTypeExtendedSettingsConfigs"));
        }

        function GetRetailBillingChargeTypeInfo() {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_Billing_ModuleConfig.moduleName, controllerName, "GetRetailBillingChargeTypeInfo"));
        }

        function GetRetailBillingChargeType(retailBillingChargeTypeId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_Billing_ModuleConfig.moduleName, controllerName, "GetRetailBillingChargeType"), {
                retailBillingChargeTypeId: retailBillingChargeTypeId
            });
        }

        function TryCompileChargeCustomCode(chargeType) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_Billing_ModuleConfig.moduleName, controllerName, "TryCompileChargeCustomCode"), chargeType);
        }

        return ({
            GetChargeTypeExtendedSettingsConfigs: GetChargeTypeExtendedSettingsConfigs,
            GetRetailBillingChargeTypeInfo: GetRetailBillingChargeTypeInfo,
            GetRetailBillingChargeType: GetRetailBillingChargeType,
            TryCompileChargeCustomCode: TryCompileChargeCustomCode
        });
    }

    appControllers.service('Retail_Billing_ChargeTypeAPIService', chargeTypeAPIService);

})(appControllers);