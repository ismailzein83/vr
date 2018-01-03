"use strict";

app.directive("vrRuntimeSchedulertaskGrid", ["VRUIUtilsService", "UtilsService", "VRNotificationService", "SchedulerTaskAPIService", "VR_Runtime_SchedulerTaskService", "VRTimerService", "VR_Runtime_SchedulerTaskStatusEnum",
    function (VRUIUtilsService, UtilsService, VRNotificationService, SchedulerTaskAPIService, VR_Runtime_SchedulerTaskService, VRTimerService, VR_Runtime_SchedulerTaskStatusEnum) {

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
            var context;

            var gridAPI;
            var gridDrillDownTabsObj;

            function initializeController() {
                $scope.schedulerTasks = [];

                $scope.onGridReady = function (api) {
                    gridAPI = api;
                    var drillDownDefinitions = VR_Runtime_SchedulerTaskService.getDrillDownDefinition();
                    gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                        ctrl.onReady(getDirectiveAPI());

                    function getDirectiveAPI() {

                        var directiveAPI = {};

                        directiveAPI.loadGrid = function (query) {
                            input.Filter = query;
                            return gridAPI.retrieveData(query);
                        };

                        directiveAPI.load = function (payload) {
                            context = payload.context;
                            var query = payload.query;
                            return directiveAPI.loadGrid(query);
                        };

                        directiveAPI.onTaskAdded = function (taskObject) {
                            gridDrillDownTabsObj.setDrillDownExtensionObject(taskObject);
                            gridAPI.itemAdded(taskObject);
                        };

                        directiveAPI.updateDataItemsStatuts = function (status) {
                            updateDataItemsStatuts(status);
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
                                if (response.Data != undefined) {
                                    for (var i = 0; i < response.Data.length; i++) {

                                        gridDrillDownTabsObj.setDrillDownExtensionObject(response.Data[i]);
                                    }
                                }
                                onResponseReady(response);
                            })
                            .catch(function (error) {
                                VRNotificationService.notifyException(error, $scope);
                            });
                    }
                    else {
                        return SchedulerTaskAPIService.GetFilteredTasks(dataRetrievalInput)
                            .then(function (response) {
                                if (response.Data != undefined) {
                                    for (var i = 0; i < response.Data.length; i++) {

                                        gridDrillDownTabsObj.setDrillDownExtensionObject(response.Data[i]);
                                    }
                                }
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
                    var staticMenuActions = defineStaticMenuActions(dataItem);
                    if (staticMenuActions != undefined) {
                        for (var index = 0; index < staticMenuActions.length; index++) {
                            menuActions.push(staticMenuActions[index]);
                        }
                    }

                    if (dataItem != undefined) {
                        var entity = dataItem.Entity;

                        if (entity.IsEnabled == true && entity.NextRunTime != undefined && !isSchedulerTaskRunning(entity.StatusDescription) && dataItem.AllowRun == true) {
                            menuActions.push({
                                name: "Run",
                                clicked: runSchedulerTask
                            })
                        }
                        if (entity.IsEnabled && dataItem.AllowEdit == true) {
                            var menuAction1 = {
                                name: "Disable",
                                clicked: disableTask
                            };
                            menuActions.push(menuAction1);
                        } else if (dataItem.AllowEdit == true) {
                            var menuAction2 = {
                                name: "Enable",
                                clicked: enableTask
                            };
                            menuActions.push(menuAction2);
                        }
                    }

                    return menuActions;
                }
            }

            function defineStaticMenuActions(dataItem) {
                var staticMenuActions = [];

                if (dataItem.AllowEdit == true) {
                    staticMenuActions.push({
                        name: "Edit",
                        clicked: editTask
                    });
                }

                return staticMenuActions;
            }
            function editTask(task) {
                var onTaskUpdated = function (updatedItem) {

                    var updatedItemObj = updatedItem;
                    gridDrillDownTabsObj.setDrillDownExtensionObject(updatedItemObj);
                    gridAPI.itemUpdated(updatedItemObj);
                    enableDisableAll();
                };

                VR_Runtime_SchedulerTaskService.editTask(task.Entity.TaskId, onTaskUpdated);
            }
            function hasUpdateSchedulerTaskPermission() {
                return SchedulerTaskAPIService.HasUpdateSchedulerTaskPermission();
            }

            function runSchedulerTask(dataItem) {
                $scope.showLoader = true;
                return SchedulerTaskAPIService.RunSchedulerTask(dataItem.Entity.TaskId).then(function () {
                    setTimeout(function () { $scope.showLoader = false; }, 5000);
                }).catch(function (error) {
                    $scope.showLoader = false;
                });
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
                                    IsEnabled: schedulerTaskState.IsEnabled,
                                    LastRunTime: schedulerTaskState.Entity.LastRunTime,
                                    NextRunTime: schedulerTaskState.Entity.NextRunTime,
                                    StatusDescription: schedulerTaskState.StatusDescription
                                };
                                var obj = {
                                    Entity: Entity,
                                    AllowEdit: $scope.schedulerTasks[j].AllowEdit,
                                    AllowRun: $scope.schedulerTasks[j].AllowRun
                                };
                                gridDrillDownTabsObj.setDrillDownExtensionObject(obj);
                                $scope.schedulerTasks[j] = obj;
                                if (schedulerTaskState.StatusDescription == VR_Runtime_SchedulerTaskStatusEnum.InProgress.description)
                                    $scope.showLoader = false;

                                continue;
                            }
                        }
                    }
                    input.LastUpdateHandle = response.MaxTimeStamp;
                }
            }

            function disableTask(dataItem) {
                var onPermissionDisabled = function (entity) {
                    var gridDataItem = {
                        Entity: entity,
                        AllowEdit: dataItem.AllowEdit
                    };
                    gridDataItem.Entity.IsEnabled = false;
                    gridDrillDownTabsObj.setDrillDownExtensionObject(gridDataItem);
                    $scope.gridMenuActions(gridDataItem);
                    gridAPI.itemUpdated(gridDataItem);
                };

                VRNotificationService.showConfirmation().then(function (confirmed) {
                    if (confirmed) {
                        return SchedulerTaskAPIService.DisableTask(dataItem.Entity.TaskId).then(function () {
                            if (onPermissionDisabled && typeof onPermissionDisabled == 'function') {
                                onPermissionDisabled(dataItem.Entity);
                                enableDisableAll();
                            }
                        }).catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                    }
                });
            }

            function enableTask(dataItem) {
                var onPermissionDisabled = function (entity) {
                    var gridDataItem = {
                        Entity: entity,
                        AllowEdit: dataItem.AllowEdit
                    };
                    gridDataItem.Entity.IsEnabled = true;
                    gridDrillDownTabsObj.setDrillDownExtensionObject(gridDataItem);
                    $scope.gridMenuActions(gridDataItem);
                    gridAPI.itemUpdated(gridDataItem);
                };

                VRNotificationService.showConfirmation().then(function (confirmed) {
                    if (confirmed) {
                        return SchedulerTaskAPIService.EnableTask(dataItem.Entity.TaskId).then(function () {
                            if (onPermissionDisabled && typeof onPermissionDisabled == 'function') {
                                onPermissionDisabled(dataItem.Entity);
                                enableDisableAll();
                            }
                        }).catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                    }
                });
            }

            function updateDataItemsStatuts(status) {
                for (var j = 0; j < $scope.schedulerTasks.length; j++) {
                    if ($scope.schedulerTasks[j].Entity.IsEnabled != status) {
                        var currentSchedulerTask = $scope.schedulerTasks[j];
                        var Entity = {
                            TaskId: currentSchedulerTask.Entity.TaskId,
                            Name: currentSchedulerTask.Entity.Name,
                            IsEnabled: status,
                            LastRunTime: currentSchedulerTask.Entity.LastRunTime,
                            NextRunTime: currentSchedulerTask.Entity.NextRunTime,
                            StatusDescription: currentSchedulerTask.StatusDescription
                        };
                        var obj = {
                            Entity: Entity,
                            AllowEdit: currentSchedulerTask.AllowEdit,
                            AllowRun: currentSchedulerTask.AllowRun
                        };
                        gridDrillDownTabsObj.setDrillDownExtensionObject(obj);
                        $scope.schedulerTasks[j] = obj;
                        gridAPI.itemUpdated(obj);
                    }
                }
            }

            function enableDisableAll() {
                if (context != undefined && typeof (context.getTaskManagmentInfo) == "function") {
                    context.getTaskManagmentInfo();
                }
            }
        }

        return directiveDefinitionObject;
    }]);