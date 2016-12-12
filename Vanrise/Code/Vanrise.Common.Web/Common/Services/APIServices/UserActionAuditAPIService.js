(function (appControllers) {

    "use strict";
    userActionAudit.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig', 'SecurityService'];

    function userActionAudit(BaseAPIService, UtilsService, VRCommon_ModuleConfig, SecurityService) {

        var controllerName = 'UserActionAudit';

        function GetFilteredUserActionAudits(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetFilteredUserActionAudits"), input);
        }

        function HasViewUserActionAuditsPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VRCommon_ModuleConfig.moduleName, controllerName, ['GetFilteredUserActionAudits']));
        }

        return ({
            GetFilteredUserActionAudits: GetFilteredUserActionAudits,
            HasViewUserActionAuditsPermission: HasViewUserActionAuditsPermission
        });
    }

    appControllers.service('VRCommon_UserActionAuditAPIService', userActionAudit);

})(appControllers);