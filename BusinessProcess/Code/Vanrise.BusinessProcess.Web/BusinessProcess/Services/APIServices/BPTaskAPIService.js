(function (appControllers) {

    "use strict";
    BusinessProcess_BPTaskAPIService.$inject = ['BaseAPIService', 'UtilsService', 'BusinessProcess_BP_ModuleConfig'];

    function BusinessProcess_BPTaskAPIService(BaseAPIService, UtilsService, BusinessProcess_BP_ModuleConfig) {

        function GetProcessTaskUpdated(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, "BPTask", "GetProcessTaskUpdated"), input);
        }

        function GetProcessTaskBeforeId(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, "BPTask", "GetProcessTaskBeforeId"), input);
        }

        function GetMyUpdatedTasks(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, "BPTask", "GetMyUpdatedTasks"), input);
        }

        function GetMyTasksBeforeId(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, "BPTask", "GetMyTasksBeforeId"), input);
        }

        function ExecuteTask(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, "BPTask", "ExecuteTask"), input);
        }

        function GetBPTaskType(taskTypeId) {
            return BaseAPIService.get(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, "BPTask", "GetBPTaskType"), {
                taskTypeId: taskTypeId
            });
        }

        return ({
            GetProcessTaskUpdated: GetProcessTaskUpdated,
            GetProcessTaskBeforeId: GetProcessTaskBeforeId,
            GetMyUpdatedTasks: GetMyUpdatedTasks,
            GetMyTasksBeforeId: GetMyTasksBeforeId,
            ExecuteTask: ExecuteTask,
            GetBPTaskType: GetBPTaskType
        });
    }

    appControllers.service('BusinessProcess_BPTaskAPIService', BusinessProcess_BPTaskAPIService);

})(appControllers);