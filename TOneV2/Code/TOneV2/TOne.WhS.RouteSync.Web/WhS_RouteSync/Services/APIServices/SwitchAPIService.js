(function (appControllers) {

    "use strict";

    SwitchAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_RouteSync_ModuleConfig', 'SecurityService'];

    function SwitchAPIService(BaseAPIService, UtilsService, WhS_RouteSync_ModuleConfig, SecurityService) {

        var controllerName = 'Switch';

        function GetAllSwitches() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_RouteSync_ModuleConfig.moduleName, controllerName, "GetAllSwitches"));
        }

        return ({
            GetAllSwitches: GetAllSwitches
        });
    }

    appControllers.service('WhS_RouteSync_SwitchAPIService', SwitchAPIService);

})(appControllers);