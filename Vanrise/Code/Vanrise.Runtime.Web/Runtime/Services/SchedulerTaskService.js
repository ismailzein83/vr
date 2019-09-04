(function (appControllers) {

    'use strict';

    SchedulerTaskService.$inject = ['VRModalService', 'UtilsService', 'VRCommon_ObjectTrackingService'];

    function SchedulerTaskService(VRModalService, UtilsService, VRCommon_ObjectTrackingService) {

        var drillDownDefinitions = [];

        function addTask(onTaskAdded) {
            var parameters = {};

            var modalSettings = {};
            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onTaskAdded = onTaskAdded;
            };
            VRModalService.showModal('/Client/Modules/Runtime/Views/SchedulerTaskEditor.html', parameters, modalSettings);
        }
        function editTask(taskId, onTaskUpdated) {
            var parameters = {
                taskId: taskId
            };

            var modalSettings = {};
            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onTaskUpdated = onTaskUpdated;
            };
            VRModalService.showModal('/Client/Modules/Runtime/Views/SchedulerTaskEditor.html', parameters, modalSettings);
        }

        function showAddTaskModal(bpDefinitionId, onTaskAdded) {
            //'7a35f562-319b-47b3-8258-ec1a704a82eb' is the related action type id for workflow
            var parameters = {
                additionalParameter: { bpDefinitionID: bpDefinitionId, actionTypeId: '7a35f562-319b-47b3-8258-ec1a704a82eb' }
            };

            var modalSettings = {};
            modalSettings.onScopeReady = function (modalScope) {
                modalScope.title = "Schedule Task";
                modalScope.onTaskAdded = onTaskAdded;
            };
            VRModalService.showModal('/Client/Modules/Runtime/Views/SchedulerTaskEditor.html', parameters, modalSettings);

        }

        function getEntityUniqueName() {
            return "VR_Runtime_SchedulerTask";
        }

        function registerObjectTrackingDrillDownToSchedulerTaskService() {

            var drillDownDefinition = {};
            drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
            drillDownDefinition.directive = "vr-common-objecttracking-grid";

            drillDownDefinition.loadDirective = function (directiveAPI, ScheduelerItem) {
                ScheduelerItem.objectTrackingGridAPI = directiveAPI;

                var query = {
                    ObjectId: ScheduelerItem.Entity.TaskId,
                    EntityUniqueName: getEntityUniqueName(),
                };
                return ScheduelerItem.objectTrackingGridAPI.load(query);
            };

            addDrillDownDefinition(drillDownDefinition);
        }
        function addDrillDownDefinition(drillDownDefinition) {
            drillDownDefinitions.push(drillDownDefinition);
        }

        function getDrillDownDefinition() {
            return drillDownDefinitions;
        }

        return {
            addTask: addTask,
            editTask: editTask,
            showAddTaskModal: showAddTaskModal,
            addDrillDownDefinition: addDrillDownDefinition,
            getDrillDownDefinition: getDrillDownDefinition,
            registerObjectTrackingDrillDownToSchedulerTaskService: registerObjectTrackingDrillDownToSchedulerTaskService
        };

    }

    appControllers.service('VR_Runtime_SchedulerTaskService', SchedulerTaskService);
})(appControllers);