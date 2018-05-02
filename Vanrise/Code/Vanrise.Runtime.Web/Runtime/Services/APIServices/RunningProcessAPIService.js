(function (appControllers) {

    "use strict";
    runningProcessAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Runtime_ModuleConfig', 'SecurityService'];

    function runningProcessAPIService(BaseAPIService, UtilsService, VRRuntime_ModuleConfig) {

        var controllerName = 'RunningProcess';

        function GetFilteredRunningProcesses(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRRuntime_ModuleConfig.moduleName, controllerName, "GetFilteredRunningProcesses"), input);
        }

        return ({
            GetFilteredRunningProcesses: GetFilteredRunningProcesses,
        });
    }

    appControllers.service('VRRuntime_RunningProcessAPIService', runningProcessAPIService);

})(appControllers);