(function (appControllers) {

    "use strict";
    chargeTypeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_Billing_ModuleConfig'];

    function chargeTypeAPIService(BaseAPIService, UtilsService, Retail_Billing_ModuleConfig) {

        var controllerName = "RetailBillingChargeType";

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

        return ({
            GetChargeTypeExtendedSettingsConfigs: GetChargeTypeExtendedSettingsConfigs,
            GetRetailBillingChargeTypeInfo: GetRetailBillingChargeTypeInfo,
            GetRetailBillingChargeType: GetRetailBillingChargeType
        });
    }

    appControllers.service('Retail_Billing_ChargeTypeAPIService', chargeTypeAPIService);

})(appControllers);