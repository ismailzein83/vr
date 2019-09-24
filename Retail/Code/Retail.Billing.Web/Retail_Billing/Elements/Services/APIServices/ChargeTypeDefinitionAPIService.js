(function (appControllers) {

    "use strict";
    chargeTypeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_Billing_ModuleConfig'];

    function chargeTypeAPIService(BaseAPIService, UtilsService, Retail_Billing_ModuleConfig) {

        var controllerName = "ChargeType";
    
        function GetChargeTypeExtendedSettingsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_Billing_ModuleConfig.moduleName, controllerName, "GetChargeTypeExtendedSettingsConfigs"));
        }
     
        return ({
            GetChargeTypeExtendedSettingsConfigs: GetChargeTypeExtendedSettingsConfigs,
        });
    }

    appControllers.service('Retail_Billing_ChargeTypeAPIService', chargeTypeAPIService);

})(appControllers);