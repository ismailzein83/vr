(function (appControllers) {

    "use strict";

    VRExclusiveSessionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig', 'SecurityService'];

    function VRExclusiveSessionAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig, SecurityService) {

        var controllerName = "VRExclusiveSession";

        function GetFilteredVRExclusiveSessions(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetFilteredVRExclusiveSessions"), input);
        }

        function ForceReleaseSession(vrExclusiveSessionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "ForceReleaseSession"), { vrExclusiveSessionId: vrExclusiveSessionId });
        }

        function ForceReleaseAllSessions() {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "ForceReleaseAllSessions"));
        }

        function HasForceReleaseSessionPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VRCommon_ModuleConfig.moduleName, controllerName, ['ForceReleaseSession']));
        }

        function HasForceReleaseAllSessionsPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VRCommon_ModuleConfig.moduleName, controllerName, ['ForceReleaseAllSessions']));
        }

        return ({
            GetFilteredVRExclusiveSessions: GetFilteredVRExclusiveSessions,
            ForceReleaseSession: ForceReleaseSession,
            ForceReleaseAllSessions: ForceReleaseAllSessions,
            HasForceReleaseSessionPermission: HasForceReleaseSessionPermission,
            HasForceReleaseAllSessionsPermission: HasForceReleaseAllSessionsPermission
        });
    }

    appControllers.service('VRCommon_VRExclusiveSessionAPIService', VRExclusiveSessionAPIService);

})(appControllers);