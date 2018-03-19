(function (appControllers) {

    "use strict";

    qualityConfigurationDefinitionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_Routing_ModuleConfig'];

    function qualityConfigurationDefinitionAPIService(BaseAPIService, UtilsService, WhS_Routing_ModuleConfig) {

        var controllerName = "QualityConfigurationDefinition";

        function GetQualityConfigurationDefinition(qualityConfigurationDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, "GetQualityConfigurationDefinition"), {
                qualityConfigurationDefinitionId: qualityConfigurationDefinitionId
            });
        }

        function GetQualityConfigurationDefinitionInfo(serializedFilter) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, "GetQualityConfigurationDefinitionInfo"), {
                filter: serializedFilter
            });
        }

        function GetQualityConfigurationDefinitionExtendedSettingsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, "GetQualityConfigurationDefinitionExtendedSettingsConfigs"));
        }


        return ({
            GetQualityConfigurationDefinition: GetQualityConfigurationDefinition,
            GetQualityConfigurationDefinitionInfo: GetQualityConfigurationDefinitionInfo,
            GetQualityConfigurationDefinitionExtendedSettingsConfigs: GetQualityConfigurationDefinitionExtendedSettingsConfigs
        });
    }

    appControllers.service('WhS_Routing_QualityConfigurationDefinitionAPIService', qualityConfigurationDefinitionAPIService);
})(appControllers);