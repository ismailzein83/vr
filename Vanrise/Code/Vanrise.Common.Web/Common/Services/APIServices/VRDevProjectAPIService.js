
(function (appControllers) {

    "use strict";
    DevProjectAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig', 'SecurityService'];

    function DevProjectAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig, SecurityService) {

        var controllerName = "DevProject";
      
        function GetVRDevProjectsInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetVRDevProjectsInfo"), {
                filter: filter
            });
        }

        return ({
            GetVRDevProjectsInfo: GetVRDevProjectsInfo
        });
    }

    appControllers.service('VR_Common_DevProjectAPIService', DevProjectAPIService);

})(appControllers);