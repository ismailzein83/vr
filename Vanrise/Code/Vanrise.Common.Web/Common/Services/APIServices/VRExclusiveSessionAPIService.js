(function (appControllers) {

    "use strict";

    VRExclusiveSessionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig', 'SecurityService'];

    function VRExclusiveSessionAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig, SecurityService) {

        var controllerName = "VRExclusiveSession";

        function GetFilteredVRExclusiveSessions(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetFilteredVRExclusiveSessions"), input);
        }

        return ({
            GetFilteredVRExclusiveSessions: GetFilteredVRExclusiveSessions
        });
    }

    appControllers.service('VRCommon_VRExclusiveSessionAPIService', VRExclusiveSessionAPIService);

})(appControllers);