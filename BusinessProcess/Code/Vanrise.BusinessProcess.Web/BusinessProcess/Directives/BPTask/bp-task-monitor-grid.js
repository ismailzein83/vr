"use strict";

app.directive("businessprocessBpTaskMonitorGrid", ["BusinessProcess_BPTaskAPIService", "BusinessProcess_GridMaxSize", "BusinessProcess_BPTaskService","VRTimerService",
function (BusinessProcess_BPTaskAPIService, BusinessProcess_GridMaxSize, BusinessProcess_BPTaskService, VRTimerService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
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

        var lastUpdateHandle, lessThanID, nbOfRows, processInstanceId, userId;
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
        }

        var minId = undefined;

        function initializeController() {
            var myTaskSelected;
            $scope.bpTasks = [];
            var isGettingDataFirstTime = true;
            var job;

            $scope.onGridReady = function (api) {
                gridAPI = api;
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        $scope.isLoading = true;
                        input.LastUpdateHandle = undefined;
                        input.LessThanID = undefined;
                        input.NbOfRows = undefined;
                        myTaskSelected = query.MyTaskSelected;
                        input.ProcessInstanceId = query.BPInstanceID;
                        $scope.bpTasks.length = 0;
                        isGettingDataFirstTime = true;
                        minId = undefined;
                        job = createTimer();
                    }

                    directiveAPI.clearTimer = function () {
                        if (job) {
                            VRTimerService.unregisterJob(job);
                        }
                    }
                    return directiveAPI;
                }

                function manipulateDataUpdated(response) {
                    var itemAddedOrUpdatedInThisCall = false;
                    if (response != undefined) {
                        for (var i = 0; i < response.ListBPTaskDetails.length; i++) {
                            var bpTask = response.ListBPTaskDetails[i];

                            if (!isGettingDataFirstTime && bpTask.Entity.BPTaskId < minId) {
                                continue;
                            }

                            var findBPTask = false;

                            for (var j = 0; j < $scope.bpTasks.length; j++) {

                                if ($scope.bpTasks[j].Entity.BPTaskId == bpTask.Entity.BPTaskId) {
                                    $scope.bpTasks[j] = bpTask;
                                    findBPTask = true;
                                    itemAddedOrUpdatedInThisCall = true;
                                    continue;
                                }
                            }
                            if (!findBPTask) {
                                itemAddedOrUpdatedInThisCall = true;
                                $scope.bpTasks.push(bpTask);
                            }
                        }

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
                    input.LastUpdateHandle = response.MaxTimeStamp;
                };


                function createTimer() {
                    if (job) {
                        VRTimerService.unregisterJob(job);
                    }
                    var pageInfo = gridAPI.getPageInfo();
                    input.NbOfRows = pageInfo.toRow - pageInfo.fromRow + 1;
                    if (myTaskSelected) {
                        return VRTimerService.registerJob(onMyTimerElapsed);
                    }
                    else {
                        return VRTimerService.registerJob(onTimerElapsed);
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

                $scope.$on("$destroy", function () {
                    if (job) {
                        VRTimerService.unregisterJob(job);
                    }
                });
            };
        }

        $scope.getStatusColor = function (dataItem) {
            return BusinessProcess_BPTaskService.getStatusColor(dataItem.Entity.Status);
        };

        $scope.taskClicked = function (dataItem) {
            if (dataItem.Entity.Status == 0) {
                return BusinessProcess_BPTaskService.openTask(dataItem.Entity.BPTaskId);
            }
        }


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