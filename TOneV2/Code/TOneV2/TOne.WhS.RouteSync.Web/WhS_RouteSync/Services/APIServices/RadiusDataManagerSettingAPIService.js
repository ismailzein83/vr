(function (appControllers) {

    "use strict";
    radiusDataManagerSetting.$inject = ['BaseAPIService', 'UtilsService', 'WhS_RouteSync_ModuleConfig', 'SecurityService'];

    function radiusDataManagerSetting(BaseAPIService, UtilsService, WhS_RouteSync_ModuleConfig, SecurityService) {
        var controllerName = 'RadiusDataManagerSettings';

        function GetRadiusDataManagerExtensionConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_RouteSync_ModuleConfig.moduleName, controllerName, "GetRadiusDataManagerExtensionConfigs"));
        }

        return ({
            GetRadiusDataManagerExtensionConfigs: GetRadiusDataManagerExtensionConfigs
        });
    }

    appControllers.service('WhS_RouteSync_RadiusDataManagerSettingAPIService', radiusDataManagerSetting);

})(appControllers);