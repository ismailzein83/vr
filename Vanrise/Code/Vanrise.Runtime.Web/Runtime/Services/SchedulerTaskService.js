(function (appControllers) {

    'use strict';

    SchedulerTaskService.$inject = ['VRModalService', 'UtilsService', 'VRCommon_ObjectTrackingService'];

    function SchedulerTaskService(VRModalService, UtilsService, VRCommon_ObjectTrackingService) {
        var drillDownDefinitions = [];
        return ({
            addTask: addTask,
            editTask: editTask,
            getDrillDownDefinition: getDrillDownDefinition,
            registerObjectTrackingDrillDownToSchedulerTaskService: registerObjectTrackingDrillDownToSchedulerTaskService
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

           // drillDownDefinitions.push(drillDownDefinition);
        }

        function getDrillDownDefinition() {
            return drillDownDefinitions;
        }
    }

    appControllers.service('VR_Runtime_SchedulerTaskService', SchedulerTaskService);

})(appControllers);

