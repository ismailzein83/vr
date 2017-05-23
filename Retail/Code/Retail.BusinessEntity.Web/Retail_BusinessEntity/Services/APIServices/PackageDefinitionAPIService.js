(function (appControllers) {

    "use strict";
    packageAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_BE_ModuleConfig', 'SecurityService'];

    function packageAPIService(BaseAPIService, UtilsService, Retail_BE_ModuleConfig, SecurityService) {

        var controllerName = "PackageDefinition";
    
        function GetPackageDefinitionExtendedSettingsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetPackageDefinitionExtendedSettingsConfigs"));
        }
        function GetPackageDefinitionsInfo(serializedFilter) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetPackageDefinitionsInfo"), {
                serializedFilter: serializedFilter
            });
        }
        function GetRecurringChargeEvaluatorConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetRecurringChargeEvaluatorConfigs"));
        }
        function GetPackageDefinition(packageDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetPackageDefinition"), {
                packageDefinitionId: packageDefinitionId
            });
        }
        return ({
            GetPackageDefinitionsInfo: GetPackageDefinitionsInfo,
            GetPackageDefinitionExtendedSettingsConfigs: GetPackageDefinitionExtendedSettingsConfigs,
            GetRecurringChargeEvaluatorConfigs: GetRecurringChargeEvaluatorConfigs,
            GetPackageDefinition: GetPackageDefinition
        });
    }

    appControllers.service('Retail_BE_PackageDefinitionAPIService', packageAPIService);

})(appControllers);