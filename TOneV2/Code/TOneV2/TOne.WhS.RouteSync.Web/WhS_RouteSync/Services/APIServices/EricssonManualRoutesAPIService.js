(function (appControllers) {

    "use strict";

    EricssonManualRoutesAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_RouteSync_ModuleConfig'];

    function EricssonManualRoutesAPIService(BaseAPIService, UtilsService, WhS_RouteSync_ModuleConfig) {

        var controllerName = 'EricssonManualRoutes';

        function GetManualRouteActionTypeExtensionConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_RouteSync_ModuleConfig.moduleName, controllerName, "GetManualRouteActionTypeExtensionConfigs"));
        }

        function GetManualRouteDestinationsTypeExtensionConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_RouteSync_ModuleConfig.moduleName, controllerName, "GetManualRouteDestinationsTypeExtensionConfigs"));
        }

        function GetManualRouteOriginationsTypeExtensionConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_RouteSync_ModuleConfig.moduleName, controllerName, "GetManualRouteOriginationsTypeExtensionConfigs"));
        }
        function GetSpecialRoutingTypeExtensionConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_RouteSync_ModuleConfig.moduleName, controllerName, "GetSpecialRoutingTypeExtensionConfigs"));
        }
        return ({
            GetManualRouteActionTypeExtensionConfigs: GetManualRouteActionTypeExtensionConfigs,
            GetManualRouteDestinationsTypeExtensionConfigs: GetManualRouteDestinationsTypeExtensionConfigs,
            GetManualRouteOriginationsTypeExtensionConfigs: GetManualRouteOriginationsTypeExtensionConfigs,
            GetSpecialRoutingTypeExtensionConfigs: GetSpecialRoutingTypeExtensionConfigs
        });
    }

    appControllers.service('WhS_RouteSync_EricssonManualRoutesAPIService', EricssonManualRoutesAPIService);

})(appControllers);