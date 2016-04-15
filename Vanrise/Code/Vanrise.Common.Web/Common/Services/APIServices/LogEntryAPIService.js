(function (appControllers) {

    "use strict";
    logEntryAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig', 'SecurityService'];

    function logEntryAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig, SecurityService) {
        var controllerName = 'LogAttribute';

        function GetFilteredLoggers(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetFilteredLoggers"), input);
        }

        function HasViewSystemLogPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VRCommon_ModuleConfig.moduleName, controllerName, ['GetFilteredLoggers']));
        }


        return ({
            GetFilteredLoggers: GetFilteredLoggers,
            HasViewSystemLogPermission: HasViewSystemLogPermission
        });
    }

    appControllers.service('VRCommon_LogEntryAPIService', logEntryAPIService);

})(appControllers);