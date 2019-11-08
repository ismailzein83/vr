(function (appControllers) {

    "use strict";
    chargeTypeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_Billing_ModuleConfig'];

    function chargeTypeAPIService(BaseAPIService, UtilsService, Retail_Billing_ModuleConfig) {

        var controllerName = "RetailBillingChargeType";

        function GetChargeTypeExtendedSettingsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_Billing_ModuleConfig.moduleName, controllerName, "GetChargeTypeExtendedSettingsConfigs"));
        }

        function GetRetailBillingChargeTypeInfo(targetRecordTypeId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_Billing_ModuleConfig.moduleName, controllerName, "GetRetailBillingChargeTypeInfo"), {
                targetRecordTypeId: targetRecordTypeId
            });
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