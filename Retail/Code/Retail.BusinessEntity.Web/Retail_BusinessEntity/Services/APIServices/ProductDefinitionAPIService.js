(function (appControllers) {

    "use strict";

    productAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_BE_ModuleConfig', 'SecurityService'];

    function productAPIService(BaseAPIService, UtilsService, Retail_BE_ModuleConfig, SecurityService) {

        var controllerName = "ProductDefinition";

        function GetProductDefinitionsInfo(serializedFilter) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetProductDefinitionsInfo"), {
                serializedFilter: serializedFilter
            });
        }

        function GetProductDefinitionExtendedSettingsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetProductDefinitionExtendedSettingsConfigs"));
        }


        return ({
            GetProductDefinitionsInfo: GetProductDefinitionsInfo,
            GetProductDefinitionExtendedSettingsConfigs: GetProductDefinitionExtendedSettingsConfigs
        });
    }

    appControllers.service('Retail_BE_ProductDefinitionAPIService', productAPIService);

})(appControllers);

