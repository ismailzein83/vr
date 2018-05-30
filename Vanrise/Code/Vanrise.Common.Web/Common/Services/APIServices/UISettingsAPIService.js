
(function (appControllers) {

    "use strict";
    UISettingsAPIService.$inject = ['BaseAPIService', 'SecurityService', 'UtilsService', 'VRCommon_ModuleConfig'];

    function UISettingsAPIService(BaseAPIService, SecurityService, UtilsService, VRCommon_ModuleConfig) {

        var controllerName = "UISettings";


        function GetUIParameters(input) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'GetUIParameters'), {}, { useCache: true });
        }

       
        return ({
            GetUIParameters: GetUIParameters
        });
    }

    appControllers.service('VRCommon_UISettingsAPIService', UISettingsAPIService);

})(appControllers);