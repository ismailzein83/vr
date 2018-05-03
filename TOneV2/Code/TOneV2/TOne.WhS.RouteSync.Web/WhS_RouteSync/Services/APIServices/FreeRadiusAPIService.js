(function (appControllers) {

    "use strict";

    FreeRadiusAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_RouteSync_ModuleConfig', 'SecurityService'];

    function FreeRadiusAPIService(BaseAPIService, UtilsService, WhS_RouteSync_ModuleConfig, SecurityService) {

        var controllerName = 'FreeRadius';

        function GetFreeRadiusDataManagerConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_RouteSync_ModuleConfig.moduleName, controllerName, "GetFreeRadiusDataManagerConfigs"));
        }

        return ({
            GetFreeRadiusDataManagerConfigs: GetFreeRadiusDataManagerConfigs
        });
    }

    appControllers.service('WhS_RouteSync_FreeRadiusAPIService', FreeRadiusAPIService);

})(appControllers);