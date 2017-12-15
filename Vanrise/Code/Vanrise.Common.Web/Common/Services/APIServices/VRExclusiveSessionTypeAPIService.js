(function (appControllers) {

    "use strict";

    VRExclusiveSessionTypeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig', 'SecurityService'];

    function VRExclusiveSessionTypeAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig, SecurityService) {

        var controllerName = "VRExclusiveSessionType";

        function GetVRExclusiveSessionTypeExtendedSettingsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetVRExclusiveSessionTypeExtendedSettingsConfigs"));
        }

        function TryTakeSession(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "TryTakeSession"), input);
        }

        function TryKeepSession(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "TryKeepSession"), input);
        }

        function ReleaseSession(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "ReleaseSession"), input);
        }

        function GetSessionLockHeartbeatIntervalInSeconds() {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetSessionLockHeartbeatIntervalInSeconds"));

        }

        return ({
            GetVRExclusiveSessionTypeExtendedSettingsConfigs: GetVRExclusiveSessionTypeExtendedSettingsConfigs,
            TryTakeSession: TryTakeSession,
            TryKeepSession: TryKeepSession,
            ReleaseSession: ReleaseSession,
            GetSessionLockHeartbeatIntervalInSeconds: GetSessionLockHeartbeatIntervalInSeconds,
        });
    }

    appControllers.service('VRCommon_VRExclusiveSessionTypeAPIService', VRExclusiveSessionTypeAPIService);

})(appControllers);