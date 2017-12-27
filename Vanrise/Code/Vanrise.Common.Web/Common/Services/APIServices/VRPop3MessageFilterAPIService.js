(function (appControllers) {

    "use strict";

    vrPop3MessageFilterAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig', 'SecurityService'];

    function vrPop3MessageFilterAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig, SecurityService) {
        var controllerName = 'VRPop3MessageFilter';

        function GetVRPop3MessageFilterConfigs(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetVRPop3MessageFilterConfigs"));
        }
        return ({
            GetVRPop3MessageFilterConfigs: GetVRPop3MessageFilterConfigs,
        });
    }

    appControllers.service('VRCommon_VRPop3MessageFilterAPIService', vrPop3MessageFilterAPIService);

})(appControllers);
