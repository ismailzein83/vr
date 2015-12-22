
app.service('VR_Runtime_SchedulerTaskService', ['VRModalService',
    function (VRModalService) {

        return ({
            addTask: addTask,
            editTask: editTask,
            editTaskNew: editTaskNew
        });

        function addTask(onTaskAdded) {
            var settings = {
            };

            var parameters = {
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onTaskAdded = onTaskAdded;
            };

            VRModalService.showModal('/Client/Modules/Runtime/Views/SchedulerTaskEditor.html', parameters, settings);
        }

        function editTask(taskId, onTaskUpdated) {
            var modalSettings = {
            };
            var parameters = {
                taskId: taskId
            };

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onTaskUpdated = onTaskUpdated;
            };
            VRModalService.showModal('/Client/Modules/Runtime/Views/SchedulerTaskEditor.html', parameters, modalSettings);
        }
        function editTaskNew(taskId, onTaskUpdated) {
            var modalSettings = {
            };
            var parameters = {
                taskId: taskId
            };

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onTaskUpdated = onTaskUpdated;
            };
            VRModalService.showModal('/Client/Modules/Runtime/Views/NewSchedulerTaskEditor/NewSchedulerTaskEditor.html', parameters, modalSettings);
        }

    }]);
