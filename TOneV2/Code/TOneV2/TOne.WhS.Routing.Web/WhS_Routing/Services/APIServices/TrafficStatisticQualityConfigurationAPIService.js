(function (appControllers) {

    "use strict";

    trafficStatisticQualityConfigurationAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_Routing_ModuleConfig'];

    function trafficStatisticQualityConfigurationAPIService(BaseAPIService, UtilsService, WhS_Routing_ModuleConfig) {

        var controllerName = "TrafficStatisticQualityConfiguration";

        function GetTrafficStatisticQualityConfigurationMeasures(qualityConfigurationDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, "GetTrafficStatisticQualityConfigurationMeasures"), {
                qualityConfigurationDefinitionId: qualityConfigurationDefinitionId
            });
        }

        return ({
            GetTrafficStatisticQualityConfigurationMeasures: GetTrafficStatisticQualityConfigurationMeasures
        });
    }

    appControllers.service('WhS_Routing_TrafficStatisticQualityConfigurationAPIService', trafficStatisticQualityConfigurationAPIService);
})(appControllers);