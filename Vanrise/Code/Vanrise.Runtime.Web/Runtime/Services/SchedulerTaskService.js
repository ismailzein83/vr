(function (appControllers) {

    'use strict';

    SchedulerTaskService.$inject = ['VRModalService', 'VRCommon_ObjectTrackingService', 'DayOfMonthTypeEnum'];

    function SchedulerTaskService(VRModalService, VRCommon_ObjectTrackingService, DayOfMonthTypeEnum) {

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

        function sortDays(days) {
            days.sort(compareDays);
        }
        function compareDays(dayOfMonth1, dayOfMonth2) {

            var day1;
            var day2;

            if (dayOfMonth1.DayOfMonthType == DayOfMonthTypeEnum.LastDay.value) {
                day1 = Number.MAX_SAFE_INTEGER;
            } else {
                day1 = dayOfMonth1.SpecificDay;
            }

            if (dayOfMonth2.DayOfMonthType == DayOfMonthTypeEnum.LastDay.value) {
                day2 = Number.MAX_SAFE_INTEGER;
            } else {
                day2 = dayOfMonth2.SpecificDay;
            }

            if (day1 > day2)
                return 1;

            if (day1 < day2)
                return -1;

            return 0;
        }

        return {
            addTask: addTask,
            editTask: editTask,
            showAddTaskModal: showAddTaskModal,
            addDrillDownDefinition: addDrillDownDefinition,
            getDrillDownDefinition: getDrillDownDefinition,
            registerObjectTrackingDrillDownToSchedulerTaskService: registerObjectTrackingDrillDownToSchedulerTaskService,
            sortDays: sortDays
        };

    }

    appControllers.service('VR_Runtime_SchedulerTaskService', SchedulerTaskService);
})(appControllers);