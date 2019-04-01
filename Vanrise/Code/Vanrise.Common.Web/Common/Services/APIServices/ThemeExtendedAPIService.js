(function (appControllers) {

    "use strict";
    ThemeExtendedAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig', 'SecurityService'];

    function ThemeExtendedAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig, SecurityService) {

        var controllerName = 'ThemeExtended';

        function GetThemesExtendedInfo() {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetThemesExtendedInfo"));
        }


        return ({
            GetThemesExtendedInfo: GetThemesExtendedInfo

        });
    }

    appControllers.service('VRCommon_ThemeExtendedAPIService', ThemeExtendedAPIService);

})(appControllers);