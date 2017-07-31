(function (appControllers) {

    "use strict";
    qualityConfigurationAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_Routing_ModuleConfig'];

    function qualityConfigurationAPIService(BaseAPIService, UtilsService, WhS_Routing_ModuleConfig) {
        var controllerName = "RouteRuleSettings";

        function GetQualityConfigurationFields(input) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, "GetQualityConfigurationFields"), input);
        }

        return ({
            GetQualityConfigurationFields: GetQualityConfigurationFields
        });
    }

    appControllers.service('WhS_Routing_QualityConfigurationAPIService', qualityConfigurationAPIService);
})(appControllers);