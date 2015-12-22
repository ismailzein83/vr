"use strict";

app.directive("vrRuntimeSchedulertaskGrid", ["UtilsService", "VRNotificationService", "SchedulerTaskAPIService", "VR_Runtime_SchedulerTaskService",
function (UtilsService, VRNotificationService, SchedulerTaskAPIService, VR_Runtime_SchedulerTaskService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var schedulerTaskGrid = new SchedulerTaskGrid($scope, ctrl, $attrs);
            schedulerTaskGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Runtime/Directives/SchedulerTask/Templates/SchedulerTaskGridTemplate.html"

    };

    function SchedulerTaskGrid($scope, ctrl) {

        var gridAPI;
        this.initializeController = initializeController;
        
        function initializeController() {

            $scope.schedulerTasks = [];

            $scope.onGridReady = function (api) {
                gridAPI = api;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());
                function getDirectiveAPI() {

                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {

                        return gridAPI.retrieveData(query);
                    };
                    return directiveAPI;
                }

            };

            defineMenuActions();
        }

        function defineMenuActions() {
            $scope.gridMenuActions = [{
                name: "Edit",
                clicked: editTask
            }
            ];
        }

        function editTask(task) {
            var onTaskUpdated = function (updatedItem) {
                gridAPI.itemUpdated(updatedItem);
            };
            VR_Runtime_SchedulerTaskService.editTask(task.TaskId, onTaskUpdated);
        }

        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
            return SchedulerTaskAPIService.GetFilteredTasks(dataRetrievalInput)
                .then(function (response) {
                    onResponseReady(response);
                })
                .catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
        };
    }

    return directiveDefinitionObject;

}]);