(function (appControllers) {

    "use strict";

    qualityConfigurationAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_Routing_ModuleConfig'];

    function qualityConfigurationAPIService(BaseAPIService, UtilsService, WhS_Routing_ModuleConfig) {

        var controllerName = "QualityConfiguration";

        function GetQualityConfigurationInfo(serializedFilter) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, "GetQualityConfigurationInfo"), { filter: serializedFilter });
        }

        function ValidateRouteRuleQualityConfiguration(serializedRouteRuleQualityConfiguration) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, "ValidateRouteRuleQualityConfiguration"), {
                serializedRouteRuleQualityConfiguration: serializedRouteRuleQualityConfiguration
            });
        }


        return ({
            GetQualityConfigurationInfo: GetQualityConfigurationInfo,
            ValidateRouteRuleQualityConfiguration: ValidateRouteRuleQualityConfiguration
        });
    }

    appControllers.service('WhS_Routing_QualityConfigurationAPIService', qualityConfigurationAPIService);
})(appControllers);