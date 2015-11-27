(function (appControllers) {

    "use strict";
    rpRouteAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_Routing_ModuleConfig'];

    function rpRouteAPIService(BaseAPIService, UtilsService, WhS_Routing_ModuleConfig) {

        function GetFilteredRPRoutes(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, "RPRoute", "GetFilteredRPRoutes"), input);
        }

        return ({
            GetFilteredRPRoutes: GetFilteredRPRoutes
        });
    }

    appControllers.service('WhS_Routing_RPRouteAPIService', rpRouteAPIService);
})(appControllers);