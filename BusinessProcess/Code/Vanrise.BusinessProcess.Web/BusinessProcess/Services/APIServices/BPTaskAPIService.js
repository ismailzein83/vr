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

        function GetTask(taskId) {
            return BaseAPIService.get(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, "BPTask", "GetTask"), {
                taskId: taskId
            });
        }

        

        return ({
            GetProcessTaskUpdated: GetProcessTaskUpdated,
            GetProcessTaskBeforeId: GetProcessTaskBeforeId,
            GetMyUpdatedTasks: GetMyUpdatedTasks,
            GetMyTasksBeforeId: GetMyTasksBeforeId,
            ExecuteTask: ExecuteTask,
            GetTask: GetTask
        });
    }

    appControllers.service('BusinessProcess_BPTaskAPIService', BusinessProcess_BPTaskAPIService);

})(appControllers);