(function (appControllers) {

    "use strict";
    routeSynchronizerAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_RouteSync_ModuleConfig', 'SecurityService'];

    function routeSynchronizerAPIService(BaseAPIService, UtilsService, WhS_RouteSync_ModuleConfig, SecurityService) {
        var controllerName = 'SwitchRouteSynchronizer';

        function GetSwitchRouteSynchronizerExtensionConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_RouteSync_ModuleConfig.moduleName, controllerName, "GetSwitchRouteSynchronizerExtensionConfigs"));
        }

        return ({
            GetSwitchRouteSynchronizerExtensionConfigs: GetSwitchRouteSynchronizerExtensionConfigs
        });
    }

    appControllers.service('WhS_RouteSync_SwitchRouteSynchronizerAPIService', routeSynchronizerAPIService);

})(appControllers);