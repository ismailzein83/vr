(function (appControllers) {

    "use strict";
    routeSyncDefinitionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_RouteSync_ModuleConfig', 'SecurityService'];

    function routeSyncDefinitionAPIService(BaseAPIService, UtilsService, WhS_RouteSync_ModuleConfig, SecurityService) {
        var controllerName = 'RouteSyncDefinition';

        function GetRouteSyncDefinitionsInfo() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_RouteSync_ModuleConfig.moduleName, controllerName, "GetRouteSyncDefinitionsInfo"));
        }

        return ({
            GetRouteSyncDefinitionsInfo: GetRouteSyncDefinitionsInfo
        });
    }

    appControllers.service('WhS_RouteSync_RouteSyncDefinitionAPIService', routeSyncDefinitionAPIService);

})(appControllers);