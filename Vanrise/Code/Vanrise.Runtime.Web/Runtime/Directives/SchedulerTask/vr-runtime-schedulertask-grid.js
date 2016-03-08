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
        var lastUpdateHandle, lessThanID, nbOfRows, name;
        var input = {
            LastUpdateHandle: lastUpdateHandle,
            LessThanID: lessThanID,
            NbOfRows: nbOfRows,
            Name: name
        };
        var taskIds = [];
        var gridAPI;
        var timer;
        var isMyTaskSelected;
        this.initializeController = initializeController;
        var isGettingData = false;
        function initializeController() {

            $scope.schedulerTasks = [];
            
            $scope.onGridReady = function (api) {
                gridAPI = api;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());
                function getDirectiveAPI() {

                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        input.Filter = query;
                        return gridAPI.retrieveData(query);
                    };
                    directiveAPI.onTaskAdded = function (taskObject) {
                        gridAPI.itemAdded(taskObject);
                    }

                    directiveAPI.isMyTasksSelected = function (isMyTask) {
                        isMyTaskSelected = isMyTask;
                    }
                    return directiveAPI;
                }

            };
            createTimer();
            defineMenuActions();
        }

        function createTimer() {
            if (timer != undefined) {
                clearTimeout(timer);
            }
            timer = setInterval(function () {
                if (!isGettingData) {
                    var pageInfo = gridAPI.getPageInfo();

                    input.NbOfRows = pageInfo.toRow - pageInfo.fromRow + 1;
                    taskIds.length = 0;

                    for (var x = 0; x < $scope.schedulerTasks.length; x++) {
                        taskIds.push($scope.schedulerTasks[x].Entity.TaskId);
                    }
                    input.Filter.TaskIds = taskIds;
                    SchedulerTaskAPIService.GetUpdated(input).then(function (response) {
                        isGettingData = true;
                        

                        if (response != undefined) {
                            for (var i = 0; i < response.ListSchedulerTaskStateDetails.length; i++) {

                                var schedulerTaskState = response.ListSchedulerTaskStateDetails[i];

                                for (var j = 0; j < $scope.schedulerTasks.length; j++) {
                                    if ($scope.schedulerTasks[j].Entity.TaskId == schedulerTaskState.Entity.TaskId) {
                                        var Entity = {
                                            TaskId: $scope.schedulerTasks[j].Entity.TaskId,
                                            Name: $scope.schedulerTasks[j].Entity.Name,
                                            IsEnabled: $scope.schedulerTasks[j].Entity.IsEnabled,
                                            LastRunTime: schedulerTaskState.Entity.LastRunTime,
                                            NextRunTime: schedulerTaskState.Entity.NextRunTime,
                                            StatusDescription: schedulerTaskState.StatusDescription
                                        };
                                        var obj = { Entity: Entity};
                                        $scope.schedulerTasks[j] = obj;
                                        continue;
                                    }
                                }
                            }
                        }
                        input.LastUpdateHandle = response.MaxTimeStamp;
                    })
                    .finally(function () {
                        isGettingData = false;
                    });
                }
            }, 2000);
        };


        $scope.$on("$destroy", function () {
            if (timer != undefined) {
                clearTimeout(timer);
            }
        });

        function defineMenuActions() {
            $scope.gridMenuActions = [{
                name: "Edit",
                clicked: editTask,
                haspermission: hasUpdateSchedulerTaskPermission
            }];

            function hasUpdateSchedulerTaskPermission() {
                return SchedulerTaskAPIService.HasUpdateSchedulerTaskPermission();
            }
        }

        function editTask(task) {
            var onTaskUpdated = function (updatedItem) {
                var updatedItemObj = { Entity: updatedItem };
                gridAPI.itemUpdated(updatedItemObj);
            };
            VR_Runtime_SchedulerTaskService.editTask(task.Entity.TaskId, onTaskUpdated);
        }

        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
            if (isMyTaskSelected) {
                return SchedulerTaskAPIService.GetFilteredMyTasks(dataRetrievalInput)
                    .then(function (response) {
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            }
            else {
                return SchedulerTaskAPIService.GetFilteredTasks(dataRetrievalInput)
                    .then(function (response) {
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            }
        };
    }

    return directiveDefinitionObject;

}]);