(function (appControllers) {

    'use strict';

    SchedulerTaskService.$inject = ['VRModalService', 'UtilsService'];

    function SchedulerTaskService(VRModalService, UtilsService) {
        return ({
            addTask: addTask,
            editTask: editTask
        });

        function addTask(onTaskAdded) {
            var settings = {};
            var parameters = {};

            settings.onScopeReady = function (modalScope) {               
                modalScope.onTaskAdded = onTaskAdded;
            };
            var editor = '/Client/Modules/Runtime/Views/SchedulerTaskEditor.html';
            VRModalService.showModal(editor, parameters, settings);
        }

        function editTask(taskId, onTaskUpdated) {
            var modalSettings = {};
            var parameters = {
                taskId: taskId
            };
            var editor = '/Client/Modules/Runtime/Views/SchedulerTaskEditor.html';

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onTaskUpdated = onTaskUpdated;
            };
            VRModalService.showModal(editor, parameters, modalSettings);
        }
    }

    appControllers.service('VR_Runtime_SchedulerTaskService', SchedulerTaskService);

})(appControllers);