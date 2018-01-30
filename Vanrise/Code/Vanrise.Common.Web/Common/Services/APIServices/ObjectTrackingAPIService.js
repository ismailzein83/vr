﻿(function (appControllers) {

    "use strict";
    objectTrackingAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig', 'SecurityService'];

    function objectTrackingAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig, SecurityService) {

        var controllerName = 'VRObjectTracking';

        function GetFilteredObjectTracking(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetFilteredObjectTracking"), input);
        }

        function GetVRLoggableEntitySettings(uniqueName) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetVRLoggableEntitySettings"), {
                uniqueName: uniqueName
            });
        }
        function GetObjectTrackingChangeInfo(objectTrackingId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetObjectTrackingChangeInfo"), {
                objectTrackingId: objectTrackingId,
            });
}
        return ({
            GetFilteredObjectTracking: GetFilteredObjectTracking,
            GetVRLoggableEntitySettings: GetVRLoggableEntitySettings,
            GetObjectTrackingChangeInfo:GetObjectTrackingChangeInfo
        });
    }

    appControllers.service('VRCommon_ObjectTrackingAPIService', objectTrackingAPIService);

})(appControllers);