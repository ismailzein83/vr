(function (appControllers) {

    "use strict";

    idbDataManagerSetting.$inject = ['BaseAPIService', 'UtilsService', 'WhS_RouteSync_ModuleConfig', 'SecurityService'];

    function idbDataManagerSetting(BaseAPIService, UtilsService, WhS_RouteSync_ModuleConfig, SecurityService) {

        var controllerName = 'IdbDataManagerSettings';

        function GetIdbDataManagerExtensionConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_RouteSync_ModuleConfig.moduleName, controllerName, "GetIdbDataManagerExtensionConfigs"));
        }

        return ({
            GetIdbDataManagerExtensionConfigs: GetIdbDataManagerExtensionConfigs
        });
    }

    appControllers.service('WhS_RouteSync_IdbDataManagerSettingAPIService', idbDataManagerSetting);

})(appControllers);