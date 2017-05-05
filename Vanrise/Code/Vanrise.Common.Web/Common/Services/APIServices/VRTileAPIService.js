(function (appControllers) {

    "use strict";
    VRTileAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig', 'SecurityService'];

    function VRTileAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig, SecurityService) {

        var controllerName = 'VRTile';

        function GetTileExtendedSettingsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetTileExtendedSettingsConfigs"));
        }

        return ({
            GetTileExtendedSettingsConfigs: GetTileExtendedSettingsConfigs

        });
    }

    appControllers.service('VRCommon_VRTileAPIService', VRTileAPIService);

})(appControllers);