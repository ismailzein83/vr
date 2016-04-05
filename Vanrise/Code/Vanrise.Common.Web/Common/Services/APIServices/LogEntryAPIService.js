(function (appControllers) {

    "use strict";
    logEntryAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig', 'SecurityService'];

    function logEntryAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig, SecurityService) {
        var controllerName = 'LogAttribute';

        function GetFilteredLoggers(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetFilteredLoggers"), input);
        }

        return ({
            GetFilteredLoggers: GetFilteredLoggers
        });
    }

    appControllers.service('VRCommon_LogEntryAPIService', logEntryAPIService);

})(appControllers);