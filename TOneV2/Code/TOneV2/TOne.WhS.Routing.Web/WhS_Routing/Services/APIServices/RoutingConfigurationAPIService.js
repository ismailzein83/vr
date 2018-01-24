(function (appControllers) {

    "use strict";

    routingConfigurationAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_Routing_ModuleConfig"];

    function routingConfigurationAPIService(BaseAPIService, UtilsService, WhS_Routing_ModuleConfig) {

        var controllerName = "RoutingConfiguration";

        function GetRoutingExcludedDestinationsTemplateConfigs(input) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, "GetRoutingExcludedDestinationsTemplateConfigs"));
        }


        return ({
            GetRoutingExcludedDestinationsTemplateConfigs: GetRoutingExcludedDestinationsTemplateConfigs
        });
    }

    appControllers.service("WhS_Routing_RoutingConfigurationAPIService", routingConfigurationAPIService);
})(appControllers);