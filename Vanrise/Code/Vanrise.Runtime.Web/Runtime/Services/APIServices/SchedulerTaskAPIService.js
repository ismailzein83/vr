(function (appControllers) {

    'use strict';

    SchedulerTaskAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Runtime_ModuleConfig'];

    function SchedulerTaskAPIService(BaseAPIService, UtilsService, VR_Runtime_ModuleConfig) {
        var controllerName = 'SchedulerTask';

        return ({
            GetFilteredTasks: GetFilteredTasks,
            GetTask: GetTask,
            GetSchedulerTaskTriggerTypes: GetSchedulerTaskTriggerTypes,
            GetSchedulerTaskActionTypes: GetSchedulerTaskActionTypes,
            AddTask: AddTask,
            UpdateTask: UpdateTask,
            DeleteTask: DeleteTask,
            GetSchedulesInfo: GetSchedulesInfo
        });

        function GetFilteredTasks(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Runtime_ModuleConfig.moduleName, controllerName, 'GetFilteredTasks'));
        }

        function GetTask(taskId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Runtime_ModuleConfig.moduleName, controllerName, 'GetTask'), {
                taskId: taskId
            });
        }

        function GetSchedulerTaskTriggerTypes() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Runtime_ModuleConfig.moduleName, controllerName, 'GetSchedulerTaskTriggerTypes'));
        }

        function GetSchedulerTaskActionTypes() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Runtime_ModuleConfig.moduleName, controllerName, 'GetSchedulerTaskActionTypes'));
        }

        function AddTask(task) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Runtime_ModuleConfig.moduleName, controllerName, 'AddTask'), task);
        }

        function UpdateTask(task) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Runtime_ModuleConfig.moduleName, controllerName, 'UpdateTask'), task);
        }

        function DeleteTask(taskId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Runtime_ModuleConfig.moduleName, controllerName, 'DeleteTask'), {
                taskId: taskId
            });
        }

        function GetSchedulesInfo() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Runtime_ModuleConfig.moduleName, controllerName, 'GetSchedulesInfo'));
        }
    }

    appControllers.service('SchedulerTaskAPIService', SchedulerTaskAPIService);

})(appControllers);