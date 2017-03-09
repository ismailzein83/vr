(function (appControllers) {

    "use strict";
    objectTrackingAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig', 'SecurityService'];

    function objectTrackingAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig, SecurityService) {

        var controllerName = 'VRObjectTracking';

        function GetFilteredObjectTracking(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetFilteredObjectTracking"), input);
        }

        return ({
            GetFilteredObjectTracking: GetFilteredObjectTracking
        });
    }

    appControllers.service('VRCommon_ObjectTrackingAPIService', objectTrackingAPIService);

})(appControllers);