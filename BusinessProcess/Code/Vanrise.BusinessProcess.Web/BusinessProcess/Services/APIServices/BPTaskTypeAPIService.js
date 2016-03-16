(function (appControllers) {

    "use strict";
    BusinessProcess_BPTaskTypeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'BusinessProcess_BP_ModuleConfig'];

    function BusinessProcess_BPTaskTypeAPIService(BaseAPIService, UtilsService, BusinessProcess_BP_ModuleConfig) {
        function GetBPTaskTypeByTaskId(taskId) {
            return BaseAPIService.get(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, "BPTaskType", "GetBPTaskTypeByTaskId"), {
                taskId: taskId
            });
        }

        return ({
            GetBPTaskTypeByTaskId: GetBPTaskTypeByTaskId
        });
    }

    appControllers.service('BusinessProcess_BPTaskTypeAPIService', BusinessProcess_BPTaskTypeAPIService);

})(appControllers);