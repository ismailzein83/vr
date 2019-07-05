﻿"use strict";

app.directive("businessprocessBpTaskMonitorGrid", ["UtilsService", "BusinessProcess_BPTaskAPIService", "BusinessProcess_GridMaxSize", "BusinessProcess_BPTaskService", "VRTimerService", "BPTaskStatusEnum",
    function (UtilsService, BusinessProcess_BPTaskAPIService, BusinessProcess_GridMaxSize, BusinessProcess_BPTaskService, VRTimerService, BPTaskStatusEnum) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope: {
                onReady: "=",
                enableAutoOpenTask: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var bpTaskGrid = new BPTaskGrid($scope, ctrl, $attrs);
                bpTaskGrid.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/BusinessProcess/Directives/BPTask/Templates/BPTaskMonitorGridTemplate.html"

        };

        function BPTaskGrid($scope, ctrl) {

            var lastUpdateHandle, lessThanID, nbOfRows, processInstanceId, userId, context, myAssignedPendingTasks = [], isWarned = false;
            var myTaskSelected;
            var input = {
                LastUpdateHandle: lastUpdateHandle,
                LessThanID: lessThanID,
                NbOfRows: nbOfRows,
                ProcessInstanceId: processInstanceId,
                UserId: userId
            };

            var gridAPI;
            this.initializeController = initializeController;

            $scope.loadMoreData = function () {
                return getData();
            };

            var minId = undefined;

            function initializeController() {
                $scope.bpTasks = [];
                var isGettingDataFirstTime = true;
                var isCallingSearchFirstTime = true;

                $scope.onGridReady = function (api) {
                    gridAPI = api;
                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                        ctrl.onReady(getDirectiveAPI());

                    function getDirectiveAPI() {
                        var directiveAPI = {};
                        directiveAPI.loadGrid = function (query) {
                            context = query.context;

                            $scope.isLoading = true;
                            gridAPI.clearDataAndContinuePaging();
                            input.LastUpdateHandle = undefined;
                            input.LessThanID = undefined;
                            input.NbOfRows = undefined;
                            myTaskSelected = query.MyTaskSelected;
                            processInstanceId = query.BPInstanceID;
                            input.ProcessInstanceId = query.BPInstanceID;
                            input.BPTaskFilter = query.BPTaskFilter;
                            $scope.bpTasks.length = 0;
                            isGettingDataFirstTime = true;
                            isCallingSearchFirstTime = true;
                            minId = undefined;
                            createTimer();
                        };

                        directiveAPI.clearTimer = function () {
                            if ($scope.jobIds) {
                                VRTimerService.unregisterJobByIds($scope.jobIds);
                                $scope.jobIds.length = 0;
                            }
                        };
                        return directiveAPI;
                    }

                    function manipulateDataUpdated(response) {
                        var autoTask;
                        var openWarningModel;
                        var itemAddedOrUpdatedInThisCall = false;
                        if (response != undefined) {
                            for (var i = 0; i < response.ListBPTaskDetails.length; i++) {
                                var bpTask = response.ListBPTaskDetails[i];
                                var statusTypeObj = UtilsService.getEnum(BPTaskStatusEnum, 'value', bpTask.Entity.Status);

                                if (bpTask.IsAssignedToCurrentUser) {
                                    var index = myAssignedPendingTasks.indexOf(bpTask.Entity.BPTaskId);
                                    if (statusTypeObj.IsClosed) {
                                        if (index > -1)
                                            myAssignedPendingTasks.splice(index, 1);
                                        if (myAssignedPendingTasks.length == 0)
                                            isWarned = false;
                                    }
                                    else {
                                        if (index < 0)
                                            myAssignedPendingTasks.push(bpTask.Entity.BPTaskId);
                                    }
                                }

                                if (!isGettingDataFirstTime && bpTask.Entity.BPTaskId < minId) {
                                    continue;
                                }

                                if (processInstanceId != undefined && !isCallingSearchFirstTime && (minId == undefined || bpTask.Entity.BPTaskId > minId) && !bpTask.IsAssignedToCurrentUser && !statusTypeObj.IsClosed) {
                                    openWarningModel = true;
                                }
                                var findBPTask = false;


                                var removeTask = (processInstanceId == undefined) && (statusTypeObj.IsClosed || !bpTask.ShowOnGrid);

                                for (var j = 0; j < $scope.bpTasks.length; j++) {

                                    if ($scope.bpTasks[j].Entity.BPTaskId == bpTask.Entity.BPTaskId) {
                                        $scope.bpTasks[j] = bpTask;
                                        findBPTask = true;
                                        itemAddedOrUpdatedInThisCall = true;
                                        continue;
                                    }
                                }

                                if (!findBPTask && !removeTask) {
                                    itemAddedOrUpdatedInThisCall = true;
                                    $scope.bpTasks.push(bpTask);
                                    if (!myTaskSelected && autoTask == undefined && bpTask.AutoOpenTask && bpTask.IsAssignedToCurrentUser && bpTask.Entity.Status == BPTaskStatusEnum.New.value) {
                                        autoTask = bpTask;
                                    }
                                }
                                else {
                                    if (findBPTask && removeTask) {
                                        var index = $scope.bpTasks.indexOf(bpTask);
                                        $scope.bpTasks.splice(index, 1);
                                    }
                                }
                            }

                            if (openWarningModel && !isWarned && myAssignedPendingTasks.length == 0 && context != undefined && context.openWarningModal != undefined && typeof (context.openWarningModal) == "function") {
                                {
                                    isWarned = true;
                                    context.openWarningModal();
                                }

                            };

                            if (itemAddedOrUpdatedInThisCall) {
                                if ($scope.bpTasks.length > 0) {
                                    $scope.bpTasks.sort(function (a, b) {
                                        return b.Entity.BPTaskId - a.Entity.BPTaskId;
                                    });

                                    if ($scope.bpTasks.length > BusinessProcess_GridMaxSize.maximumCount) {
                                        $scope.bpTasks.length = BusinessProcess_GridMaxSize.maximumCount;
                                    }
                                    minId = $scope.bpTasks[$scope.bpTasks.length - 1].Entity.BPTaskId;
                                    isGettingDataFirstTime = false;
                                }
                            }

                        }

                        isCallingSearchFirstTime = false;
                        input.LastUpdateHandle = response.LastUpdateHandle;

                        if (ctrl.enableAutoOpenTask && autoTask != undefined)
                            BusinessProcess_BPTaskService.openTask(autoTask.Entity.BPTaskId);
                    };


                    function createTimer() {
                        if ($scope.jobIds) {
                            VRTimerService.unregisterJobByIds($scope.jobIds);
                            $scope.jobIds.length = 0;
                        }
                        var pageInfo = gridAPI.getPageInfo();
                        input.NbOfRows = pageInfo.toRow - pageInfo.fromRow + 1;
                        if (myTaskSelected) {
                            VRTimerService.registerJob(onMyTimerElapsed, $scope);
                        }
                        else {
                            VRTimerService.registerJob(onTimerElapsed, $scope);
                        }
                    }

                    function onTimerElapsed() {
                        return BusinessProcess_BPTaskAPIService.GetProcessTaskUpdated(input).then(function (response) {
                            manipulateDataUpdated(response);
                            $scope.isLoading = false;
                        },
                            function (excpetion) {
                                console.log(excpetion);
                                $scope.isLoading = false;
                            });
                    }

                    function onMyTimerElapsed() {
                        return BusinessProcess_BPTaskAPIService.GetMyUpdatedTasks(input).then(function (response) {
                            manipulateDataUpdated(response);
                            $scope.isLoading = false;
                        },
                            function (excpetion) {
                                console.log(excpetion);
                                $scope.isLoading = false;
                            });
                    }
                };
            }

            $scope.getStatusColor = function (dataItem) {
                return BusinessProcess_BPTaskService.getStatusColor(dataItem.Entity.Status);
            };

            $scope.taskClicked = function (dataItem) {
                if (dataItem.Entity.Status == 0) {
                    return BusinessProcess_BPTaskService.openTask(dataItem.Entity.BPTaskId);
                }
            };

            function manipulateDataBefore(response) {
                if (response != undefined && response) {
                    for (var i = 0; i < response.length; i++) {
                        var bpTask = response[i];
                        minId = response[i].Entity.BPTaskId;
                        $scope.bpTasks.push(bpTask);
                    }
                }
            };

            function getData() {

                var pageInfo = gridAPI.getPageInfo();
                input.LessThanID = minId;
                input.NbOfRows = pageInfo.toRow - pageInfo.fromRow + 1;

                if (myTaskSelected) {
                    return BusinessProcess_BPTaskAPIService.GetMyTasksBeforeId(input).then(function (response) {
                        manipulateDataBefore(response);
                    });
                }
                else {
                    return BusinessProcess_BPTaskAPIService.GetProcessTaskBeforeId(input).then(function (response) {
                        manipulateDataBefore(response);
                    });
                }
            }
        }
        return directiveDefinitionObject;

    }]);