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
        function TakeTask(taskId) {
            return BaseAPIService.get(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, "BPTask", "TakeTask"), {
                taskId: taskId
            });
        }

        function ReleaseTask(taskId) {
            return BaseAPIService.get(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, "BPTask", "ReleaseTask"), {
                taskId: taskId
            });
        }
        function GetAssignedUsers(taskId) {
            return BaseAPIService.get(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, "BPTask", "GetAssignedUsers"), {
                taskId: taskId
            });
        }

        function AssignTask(taskId, userId) {
            return BaseAPIService.get(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, "BPTask", "AssignTask"), {
                taskId: taskId,
                userId: userId
            });
        }

        function GetInitialBPTaskDefaultActionsState(userId) {
            return BaseAPIService.post(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, "BPTask", "GetInitialBPTaskDefaultActionsState"), {
                UserId: userId
            });
        }

        function GetTaskTypeActions(taskId) {
            return BaseAPIService.get(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, "BPTask", "GetTaskTypeActions"), {
                taskId: taskId
            });
        }
        

        return ({
            GetProcessTaskUpdated: GetProcessTaskUpdated,
            GetProcessTaskBeforeId: GetProcessTaskBeforeId,
            GetMyUpdatedTasks: GetMyUpdatedTasks,
            GetMyTasksBeforeId: GetMyTasksBeforeId,
            ExecuteTask: ExecuteTask,
            GetTask: GetTask,
            TakeTask: TakeTask,
            ReleaseTask: ReleaseTask,
            GetAssignedUsers: GetAssignedUsers,
            AssignTask: AssignTask,
            GetInitialBPTaskDefaultActionsState: GetInitialBPTaskDefaultActionsState,
            GetTaskTypeActions: GetTaskTypeActions
        });
    }

    appControllers.service('BusinessProcess_BPTaskAPIService', BusinessProcess_BPTaskAPIService);

})(appControllers);