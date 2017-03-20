(function (appControllers) {

    'use strict';

    SchedulerTaskAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Runtime_ModuleConfig', 'SecurityService'];

    function SchedulerTaskAPIService(BaseAPIService, UtilsService, VR_Runtime_ModuleConfig, SecurityService) {
        var controllerName = 'SchedulerTask';

        function GetFilteredTasks(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Runtime_ModuleConfig.moduleName, controllerName, 'GetFilteredTasks'), input);
        }

        function GetFilteredMyTasks(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Runtime_ModuleConfig.moduleName, controllerName, 'GetFilteredMyTasks'), input);
        }

        function GetTask(taskId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Runtime_ModuleConfig.moduleName, controllerName, 'GetTask'), {
                taskId: taskId
            });
        }

        function GetSchedulerTaskTriggerTypes() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Runtime_ModuleConfig.moduleName, controllerName, 'GetSchedulerTaskTriggerTypes'));
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

        function GetMySchedulesInfo() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Runtime_ModuleConfig.moduleName, controllerName, 'GetMySchedulesInfo'));
        }

        function HasAddSchedulerTaskPermission() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Runtime_ModuleConfig.moduleName, controllerName, 'DoesUserHaveAddAccess'));
        }

        function GetUpdated(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Runtime_ModuleConfig.moduleName, controllerName, 'GetUpdated'), input);
        }

        function RunSchedulerTask(taskId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Runtime_ModuleConfig.moduleName, controllerName, 'RunSchedulerTask'), {
                taskId: taskId
            });
        }

        function HasRunSchedulerTaskPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_Runtime_ModuleConfig.moduleName, controllerName, ['RunSchedulerTask']));
        }


        return ({
            GetMySchedulesInfo: GetMySchedulesInfo,
            GetFilteredTasks: GetFilteredTasks,
            GetTask: GetTask,
            GetSchedulerTaskTriggerTypes: GetSchedulerTaskTriggerTypes,
            GetSchedulerTaskActionTypes: GetSchedulerTaskActionTypes,
            AddTask: AddTask,
            UpdateTask: UpdateTask,
            DeleteTask: DeleteTask,
            GetSchedulesInfo: GetSchedulesInfo,
            HasAddSchedulerTaskPermission: HasAddSchedulerTaskPermission,
            GetUpdated: GetUpdated,
            GetFilteredMyTasks: GetFilteredMyTasks,
            RunSchedulerTask: RunSchedulerTask
        });
    }

    appControllers.service('SchedulerTaskAPIService', SchedulerTaskAPIService);

})(appControllers);