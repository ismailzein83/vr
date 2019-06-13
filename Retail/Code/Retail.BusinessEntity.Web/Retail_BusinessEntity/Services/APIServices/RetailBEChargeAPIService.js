(function (appControllers) {
    "use strict";
    RetailBEChargeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_BE_ModuleConfig', 'SecurityService'];

    function RetailBEChargeAPIService(BaseAPIService, UtilsService, Retail_BE_ModuleConfig, SecurityService) {

        var controllerName = "RetailBECharge";

        function GetRetailBEChargeSettingsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'GetRetailBEChargeSettingsConfigs'), {
            });
        }


        return ({
            GetRetailBEChargeSettingsConfigs: GetRetailBEChargeSettingsConfigs
        });
    }

    appControllers.service('Retail_BE_RetailBEChargeAPIService', RetailBEChargeAPIService);
})(appControllers);