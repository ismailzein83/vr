(function (appControllers) {

    "use strict";
    VRIconPathAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig', 'SecurityService'];

    function VRIconPathAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig, SecurityService) {

        var controllerName = 'VRIconPath';

        function GetVRIconPathsInfo(paths) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetVRIconPathsInfo"), {
                paths: paths
            });
        }


        return ({
            GetVRIconPathsInfo: GetVRIconPathsInfo

        });
    }

    appControllers.service('VRCommon_VRIconPathAPIService', VRIconPathAPIService);

})(appControllers);