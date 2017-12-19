(function (appControllers) {

    "use strict";

    vrPop3FilterAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig', 'SecurityService'];

    function vrPop3MessageFilterAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig, SecurityService) {
        var controllerName = 'VRPop3Filter';

        function GetVRPop3Filters(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetVRPop3MessageFilters"));
        };
        return ({
            GetVRPop3Filters: GetVRPop3Filters,
        });
    }

    appControllers.service('VRCommon_VRPop3FilterAPIService', vrPop3FilterAPIService);

})(appControllers);
