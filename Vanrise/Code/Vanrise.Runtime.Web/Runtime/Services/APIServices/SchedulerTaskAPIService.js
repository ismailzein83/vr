(function (appControllers) {

    'use strict';

    SchedulerTaskAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Runtime_ModuleConfig', 'SecurityService'];

    function SchedulerTaskAPIService(BaseAPIService, UtilsService, VR_Runtime_ModuleConfig, SecurityService) {
        var controllerName = 'SchedulerTask';

        return ({
            GetFilteredTasks: GetFilteredTasks,
            GetTask: GetTask,
            GetSchedulerTaskTriggerTypes: GetSchedulerTaskTriggerTypes,
            GetSchedulerTaskActionTypes: GetSchedulerTaskActionTypes,
            AddTask: AddTask,
            UpdateTask: UpdateTask,
            DeleteTask: DeleteTask,
            GetSchedulesInfo: GetSchedulesInfo,
            HasAddSchedulerTaskPermission: HasAddSchedulerTaskPermission,
            HasUpdateSchedulerTaskPermission: HasUpdateSchedulerTaskPermission,
            GetUpdated: GetUpdated
        });

        function GetFilteredTasks(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Runtime_ModuleConfig.moduleName, controllerName, 'GetFilteredTasks'), input);
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

        function HasAddSchedulerTaskPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_Runtime_ModuleConfig.moduleName, controllerName, ['AddTask']));
        }

        function HasUpdateSchedulerTaskPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_Runtime_ModuleConfig.moduleName, controllerName, ['UpdateTask']));
        }

        function GetUpdated(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Runtime_ModuleConfig.moduleName, controllerName, 'GetUpdated'), input);
        }
    }

    appControllers.service('SchedulerTaskAPIService', SchedulerTaskAPIService);

})(appControllers);