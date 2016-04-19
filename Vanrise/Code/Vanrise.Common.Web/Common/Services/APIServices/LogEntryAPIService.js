(function (appControllers) {

    "use strict";
    logEntryAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig', 'SecurityService'];

    function logEntryAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig, SecurityService) {
        var controllerName = 'LogEntry';

        function GetFilteredLogs(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetFilteredLogs"), input);
        }

        function HasViewSystemLogPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VRCommon_ModuleConfig.moduleName, controllerName, ['GetFilteredLogs']));
        }


        return ({
            GetFilteredLogs: GetFilteredLogs,
            HasViewSystemLogPermission: HasViewSystemLogPermission
        });
    }

    appControllers.service('VRCommon_LogEntryAPIService', logEntryAPIService);

})(appControllers);