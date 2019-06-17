(function (appControllers) {

    "use strict";

    trafficStatisticQCDefinitionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_Routing_ModuleConfig'];

    function trafficStatisticQCDefinitionAPIService(BaseAPIService, UtilsService, WhS_Routing_ModuleConfig) {

        var controllerName = "TrafficStatisticQCDefinition";

        function GetTrafficStatisticQCDefinitionData(qualityConfigurationDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, "GetTrafficStatisticQCDefinitionData"), {
                qualityConfigurationDefinitionId: qualityConfigurationDefinitionId
            });
        }

        return {
            GetTrafficStatisticQCDefinitionData: GetTrafficStatisticQCDefinitionData
        };
    }

    appControllers.service('WhS_Routing_TrafficStatisticQCDefinitionAPIService', trafficStatisticQCDefinitionAPIService);
})(appControllers);