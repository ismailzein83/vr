(function (appControllers) {

    "use strict";

    DynamicBusinessProcessAPIService.$inject = ['BaseAPIService', 'UtilsService', 'SecurityService'];

    function DynamicBusinessProcessAPIService(BaseAPIService, UtilsService, SecurityService) {

        function StartProcess(processName, input) {
            return BaseAPIService.post(UtilsService.getServiceURL('DynamicBusinessProcess_BP', processName, "StartProcess"), input);
        }

        return ({
            StartProcess: StartProcess
        });
    }

    appControllers.service('BusinessProcess_DynamicBusinessProcessAPIService', DynamicBusinessProcessAPIService);
})(appControllers);