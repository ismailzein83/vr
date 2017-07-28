(function (appControllers) {

    "use strict";

    ProductDefinitionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_BE_ModuleConfig', 'SecurityService'];

    function ProductDefinitionAPIService(BaseAPIService, UtilsService, Retail_BE_ModuleConfig, SecurityService) {

        var controllerName = "ProductDefinition";

        function GetProductDefinition(productDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetProductDefinition"), {
                productDefinitionId: productDefinitionId
            });
        }

        function GetProductDefinitionsInfo(serializedFilter) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetProductDefinitionsInfo"), {
                serializedFilter: serializedFilter
            });
        }

        function GetProductDefinitionExtendedSettingsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetProductDefinitionExtendedSettingsConfigs"));
        }


        return ({
            GetProductDefinition: GetProductDefinition,
            GetProductDefinitionsInfo: GetProductDefinitionsInfo,
            GetProductDefinitionExtendedSettingsConfigs: GetProductDefinitionExtendedSettingsConfigs
        });
    }

    appControllers.service('Retail_BE_ProductDefinitionAPIService', ProductDefinitionAPIService);

})(appControllers);

