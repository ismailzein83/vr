
app.service('VR_Runtime_SchedulerTaskService', ['VRModalService', 'UtilsService',
    function (VRModalService, UtilsService) {

        return ({
            addTask: addTask,
            editTask: editTask
        });

        function addTask(onTaskAdded) {
            var settings = {
            };

            var parameters = {
            };

            settings.onScopeReady = function (modalScope) {               
                modalScope.onTaskAdded = onTaskAdded;
            };
            var editor = '/Client/Modules/Runtime/Views/SchedulerTaskEditor.html';
            VRModalService.showModal(editor, parameters, settings);

            // '/Client/Modules/Runtime/Views/NewSchedulerTaskEditor/NewSchedulerTaskEditor.html' new 
            // /Client/Modules/Runtime/Views/SchedulerTaskEditor.html old
        }

        function editTask(taskId, onTaskUpdated) {
            var modalSettings = {
            };
            var parameters = {
                taskId: taskId
            };
            var editor = '/Client/Modules/Runtime/Views/SchedulerTaskEditor.html';

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onTaskUpdated = onTaskUpdated;
            };
            VRModalService.showModal(editor, parameters, modalSettings);
        }
        
    }]);
