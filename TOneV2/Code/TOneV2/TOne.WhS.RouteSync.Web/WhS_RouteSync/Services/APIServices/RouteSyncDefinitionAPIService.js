(function (appControllers) {

    "use strict";
    routeSyncDefinitionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_RouteSync_ModuleConfig', 'SecurityService'];

    function routeSyncDefinitionAPIService(BaseAPIService, UtilsService, WhS_RouteSync_ModuleConfig, SecurityService) {
        var controllerName = 'RouteSyncDefinition';


        function GetFilteredRouteSyncDefinitions(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_RouteSync_ModuleConfig.moduleName, controllerName, 'GetFilteredRouteSyncDefinitions'), input);
        }

        function GetRouteSyncDefinition(RouteSyncDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_RouteSync_ModuleConfig.moduleName, controllerName, 'GetRouteSyncDefinition'), {
                RouteSyncDefinitionId: RouteSyncDefinitionId
            });
        }

        function AddRouteSyncDefinition(RouteSyncDefinitionItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_RouteSync_ModuleConfig.moduleName, controllerName, 'AddRouteSyncDefinition'), RouteSyncDefinitionItem);
        }

        function UpdateRouteSyncDefinition(RouteSyncDefinitionItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_RouteSync_ModuleConfig.moduleName, controllerName, 'UpdateRouteSyncDefinition'), RouteSyncDefinitionItem);
        }

        function GetRouteReaderExtensionConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_RouteSync_ModuleConfig.moduleName, controllerName, "GetRouteReaderExtensionConfigs"));
        }

        function GetRouteSyncDefinitionsInfo() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_RouteSync_ModuleConfig.moduleName, controllerName, "GetRouteSyncDefinitionsInfo"));
        }


        return ({
            GetFilteredRouteSyncDefinitions: GetFilteredRouteSyncDefinitions,
            GetRouteSyncDefinition: GetRouteSyncDefinition,
            AddRouteSyncDefinition: AddRouteSyncDefinition,
            UpdateRouteSyncDefinition: UpdateRouteSyncDefinition,
            GetRouteReaderExtensionConfigs: GetRouteReaderExtensionConfigs,
            GetRouteSyncDefinitionsInfo: GetRouteSyncDefinitionsInfo
        });
    }

    appControllers.service('WhS_RouteSync_RouteSyncDefinitionAPIService', routeSyncDefinitionAPIService);

})(appControllers);