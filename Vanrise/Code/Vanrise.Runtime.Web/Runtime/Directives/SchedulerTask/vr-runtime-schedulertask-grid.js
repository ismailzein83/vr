"use strict";

app.directive("vrRuntimeSchedulertaskGrid", ["UtilsService", "VRNotificationService", "SchedulerTaskAPIService", "VR_Runtime_SchedulerTaskService", "VRTimerService", "VR_Runtime_SchedulerTaskStatusEnum",
    function (UtilsService, VRNotificationService, SchedulerTaskAPIService, VR_Runtime_SchedulerTaskService, VRTimerService, VR_Runtime_SchedulerTaskStatusEnum) {

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
            this.initializeController = initializeController;

            var taskIds = [];
            var isMyTaskSelected;
            var lastUpdateHandle, lessThanID, nbOfRows, name;
            var input = {
                LastUpdateHandle: lastUpdateHandle,
                LessThanID: lessThanID,
                NbOfRows: nbOfRows,
                Name: name
            };


            var gridAPI;

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
                        };

                        directiveAPI.isMyTasksSelected = function (isMyTask) {
                            isMyTaskSelected = isMyTask;
                        };

                        return directiveAPI;
                    }

                    createTimer();
                };

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

                $scope.gridMenuActions = function (dataItem) {
                    var menuActions = [];

                    //Static Menu Actions
                    var staticMenuActions = defineStaticMenuActions();
                    if (staticMenuActions != undefined) {
                        for (var index = 0; index < staticMenuActions.length; index++) {
                            menuActions.push(staticMenuActions[index]);
                        }
                    }

                    if (dataItem != undefined) {
                        var entity = dataItem.Entity;

                        if (entity.IsEnabled == true && entity.NextRunTime != undefined && !isSchedulerTaskRunning(entity.StatusDescription)) {
                            menuActions.push({
                                name: "Run",
                                clicked: runSchedulerTask,
                                haspermission: hasRunSchedulerTaskPermission
                            })
                        }
                    }

                    return menuActions;
                }
            }

            function runSchedulerTask(dataItem) {
                return SchedulerTaskAPIService.RunSchedulerTask(dataItem.Entity.TaskId);
            }

            function hasRunSchedulerTaskPermission() {
                return SchedulerTaskAPIService.HasRunSchedulerTaskPermission();
            }

            function isSchedulerTaskRunning(statusDescription) {

                switch (statusDescription) {
                    case VR_Runtime_SchedulerTaskStatusEnum.NotStarted.description:
                    case VR_Runtime_SchedulerTaskStatusEnum.Completed.description:
                    case VR_Runtime_SchedulerTaskStatusEnum.Failed.description: return false;

                    case undefined:
                    case VR_Runtime_SchedulerTaskStatusEnum.InProgress.description:
                    case VR_Runtime_SchedulerTaskStatusEnum.WaitingEvent.description:
                    default: return true;
                }
            }

            function createTimer() {
                if ($scope.job) {
                    VRTimerService.unregisterJob($scope.job);
                }
                var pageInfo = gridAPI.getPageInfo();

                input.NbOfRows = pageInfo.toRow - pageInfo.fromRow + 1;
                VRTimerService.registerJob(onTimerElapsed, $scope);
            }

            function onTimerElapsed() {
                taskIds.length = 0;

                for (var x = 0; x < $scope.schedulerTasks.length; x++) {
                    taskIds.push($scope.schedulerTasks[x].Entity.TaskId);
                }
                input.Filter.TaskIds = taskIds;

                return SchedulerTaskAPIService.GetUpdated(input).then(function (response) {
                    manipulateDataUpdated(response);
                    $scope.isLoading = false;
                },
                function (excpetion) {
                    $scope.isLoading = false;
                });
            }

            function manipulateDataUpdated(response) {
                var itemAddedOrUpdatedInThisCall = false;
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
                                var obj = { Entity: Entity };
                                $scope.schedulerTasks[j] = obj;
                                continue;
                            }
                        }
                    }
                    input.LastUpdateHandle = response.MaxTimeStamp;
                }
            }

            function defineStaticMenuActions() {

                var staticMenuActions = [{
                    name: "Edit",
                    clicked: editTask,
                    haspermission: hasUpdateSchedulerTaskPermission
                }];

                return staticMenuActions;
            }
            function editTask(task) {
                var onTaskUpdated = function (updatedItem) {
                    var updatedItemObj = { Entity: updatedItem };
                    gridAPI.itemUpdated(updatedItemObj);
                };

                VR_Runtime_SchedulerTaskService.editTask(task.Entity.TaskId, onTaskUpdated);
            }
            function hasUpdateSchedulerTaskPermission() {
                return SchedulerTaskAPIService.HasUpdateSchedulerTaskPermission();
            }
        }

        return directiveDefinitionObject;
    }]);