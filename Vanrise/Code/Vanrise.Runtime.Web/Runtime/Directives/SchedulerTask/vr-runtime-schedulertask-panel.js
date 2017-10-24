"use strict";

app.directive("vrRuntimeSchedulertaskPanel", ["VRUIUtilsService", "UtilsService", "VRNotificationService", "VR_Runtime_SchedulerTaskService",
    function (VRUIUtilsService, UtilsService, VRNotificationService, VR_Runtime_SchedulerTaskService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var schedulerTaskPanel = new SchedulerTaskPanel($scope, ctrl, $attrs);
                schedulerTaskPanel.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Runtime/Directives/SchedulerTask/Templates/SchedulerTaskPanelTemplate.html"
        };

        function SchedulerTaskPanel($scope, ctrl) {
            var gridAPI;
            var bpDefinitionId;
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.showAddSchedulerTask = false;

                $scope.scopeModel.addSchedulerTask = function () {
                    var onScheduleTaskAdded = function (scheduleTaskObj) {
                        gridAPI.onTaskAdded(scheduleTaskObj);
                    };

                    VR_Runtime_SchedulerTaskService.showAddTaskModal(bpDefinitionId, onScheduleTaskAdded);
                };

             
                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };
            }

            function defineAPI() {
                var api = {};

                api.loadPanel = function (payload) {
                    if (payload != undefined) { 
                        bpDefinitionId = payload.bpDefinitionId,
                        $scope.scopeModel.showAddSchedulerTask = payload.showAddSchedulerTask
                    }
                    return gridAPI.loadGrid(getGridQuery());
                };
            

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(api);
            }

            function getGridQuery() {

                var filter = {
                    $type: "Vanrise.BusinessProcess.Extensions.WFTaskAction.Arguments.WFTaskBPDefinitionFilter, Vanrise.BusinessProcess.Extensions.WFTaskAction.Arguments",
                    BPDefinitionId: bpDefinitionId
                };
                var payload = { Filters: [] };
                payload.Filters.push(filter);

                return payload;
            }
        }

           
        return directiveDefinitionObject;
    }]);