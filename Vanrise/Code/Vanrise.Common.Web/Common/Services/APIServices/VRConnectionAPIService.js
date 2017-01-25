(function (appControllers) {

    "use strict";

    vrConnectionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig'];

    function vrConnectionAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig) {
        var controllerName = 'VRConnection';

        function GetVRConnectionInfos(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetVRConnectionInfos"), {
                filter: filter
            });
        };

        return ({
            GetVRConnectionInfos: GetVRConnectionInfos
        });
    }

    appControllers.service('VRCommon_ConnectionAPIService', vrConnectionAPIService);

})(appControllers);