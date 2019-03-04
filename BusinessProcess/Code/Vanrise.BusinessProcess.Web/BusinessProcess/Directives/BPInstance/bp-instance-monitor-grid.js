﻿"use strict";

app.directive("businessprocessBpInstanceMonitorGrid", ["BusinessProcess_BPInstanceAPIService", "BusinessProcess_BPInstanceService", "BusinessProcess_GridMaxSize", "VRTimerService",
    function (BusinessProcess_BPInstanceAPIService, BusinessProcess_BPInstanceService, BusinessProcess_GridMaxSize, VRTimerService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var bpInstanceGrid = new BPInstanceGrid($scope, ctrl, $attrs);
                bpInstanceGrid.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/BusinessProcess/Directives/BPInstance/Templates/BPInstanceMonitorGridTemplate.html"
        };

        function BPInstanceGrid($scope, ctrl) {
            this.initializeController = initializeController;

            //Variables
            var isGettingDataFirstTime = true;
            var minId;

            var lastUpdateHandle, lessThanID, nbOfRows, definitionsId, entityId, taskId;

            var input = {
                LastUpdateHandle: lastUpdateHandle,
                LessThanID: lessThanID,
                NbOfRows: nbOfRows,
                DefinitionsId: definitionsId,
                EntityId: entityId,
                TaskId: taskId
            };

            //API
            var gridAPI;

            function initializeController() {
                $scope.bpInstances = [];

                $scope.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.loadMoreData = function () {
                    return getData();
                };

                $scope.getStatusColor = function (dataItem) {
                    return BusinessProcess_BPInstanceService.getStatusColor(dataItem.Entity.Status);
                };

                $scope.processInstanceClicked = function (dataItem) {
                    BusinessProcess_BPInstanceService.openProcessTracking(dataItem.Entity.ProcessInstanceID);
                };
            }
            function defineAPI() {
                var api = {};

                api.loadGrid = function (query) {
                    $scope.isLoading = true;
                    input.LastUpdateHandle = undefined;
                    input.LessThanID = undefined;
                    input.NbOfRows = undefined;
                    input.DefinitionsId = query.DefinitionsId;
                    input.ParentId = query.BPInstanceID;
                    input.EntityIds = query.EntityIds;
                    input.TaskId = query.TaskId;

                    $scope.bpInstances.length = 0;
                    isGettingDataFirstTime = true;
                    minId = undefined;

                    createTimer();
                };

                api.clearTimer = function () {
                    if ($scope.jobIds) {
                        VRTimerService.unregisterJobByIds($scope.jobIds);
                        $scope.jobIds.length = 0;
                    }
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function getData() {
                var pageInfo = gridAPI.getPageInfo();
                input.LessThanID = minId;
                input.NbOfRows = pageInfo.toRow - pageInfo.fromRow + 1;

                return BusinessProcess_BPInstanceAPIService.GetBeforeId(input).then(function (response) {
                    if (response != undefined && response) {
                        for (var i = 0; i < response.length; i++) {
                            var bpInstance = response[i];
                            minId = response[i].Entity.ProcessInstanceID;
                            $scope.bpInstances.push(bpInstance);
                        }
                    }
                });
            }

            function createTimer() {
                if ($scope.jobIds) {
                    VRTimerService.unregisterJobByIds($scope.jobIds);
                    $scope.jobIds.length = 0;
                }

                var pageInfo = gridAPI.getPageInfo();
                input.NbOfRows = pageInfo.toRow - pageInfo.fromRow + 1;
                VRTimerService.registerJob(onTimerElapsed, $scope);
            }
            function onTimerElapsed() {
                return BusinessProcess_BPInstanceAPIService.GetUpdated(input).then(function (response) {
                    manipulateDataUpdated(response);
                }).finally(function () {
                    $scope.isLoading = false;
                });
            }
            function manipulateDataUpdated(response) {
                var itemAddedOrUpdatedInThisCall = false;

                if (response != undefined) {
                    for (var i = 0; i < response.ListBPInstanceDetails.length; i++) {
                        var bpInstance = response.ListBPInstanceDetails[i];

                        if (!isGettingDataFirstTime && bpInstance.Entity.ProcessInstanceID < minId) {
                            continue;
                        }

                        var findBPInstance = false;

                        for (var j = 0; j < $scope.bpInstances.length; j++) {
                            if ($scope.bpInstances[j].Entity.ProcessInstanceID == bpInstance.Entity.ProcessInstanceID) {
                                $scope.bpInstances[j] = bpInstance;
                                findBPInstance = true;
                                itemAddedOrUpdatedInThisCall = true;
                                continue;
                            }
                        }
                        if (!findBPInstance) {
                            itemAddedOrUpdatedInThisCall = true;
                            $scope.bpInstances.push(bpInstance);
                        }
                    }

                    if (itemAddedOrUpdatedInThisCall) {
                        if ($scope.bpInstances.length > 0) {
                            $scope.bpInstances.sort(function (a, b) {
                                return b.Entity.ProcessInstanceID - a.Entity.ProcessInstanceID;
                            });

                            if ($scope.bpInstances.length > BusinessProcess_GridMaxSize.maximumCount) {
                                $scope.bpInstances.length = BusinessProcess_GridMaxSize.maximumCount;
                            }
                            minId = $scope.bpInstances[$scope.bpInstances.length - 1].Entity.ProcessInstanceID;
                            isGettingDataFirstTime = false;
                        }
                    }
                }
                input.LastUpdateHandle = response.LastUpdateHandle;
            }
        }

        return directiveDefinitionObject;
    }]);