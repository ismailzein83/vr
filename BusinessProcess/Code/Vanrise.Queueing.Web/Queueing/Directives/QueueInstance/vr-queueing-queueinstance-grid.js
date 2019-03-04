"use strict";

app.directive("vrQueueingQueueinstanceGrid", ["VR_Queueing_QueueInstanceAPIService", "VR_Queueing_QueueInstanceService", "VR_Queueing_QueueItemHeaderAPIService", "VRNotificationService", "VR_Queueing_QueueItemStatusEnum", "LabelColorsEnum", "VRTimerService",
    function (VR_Queueing_QueueInstanceAPIService, VR_Queueing_QueueInstanceService, VR_Queueing_QueueItemHeaderAPIService, VRNotificationService, VR_Queueing_QueueItemStatusEnum, LabelColorsEnum, VRTimerService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var grid = new ExecutionFlowGrid($scope, ctrl, $attrs);
                grid.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Queueing/Directives/QueueInstance/Templates/QueueInstanceGrid.html"
        };

        function ExecutionFlowGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;

            function initializeController() {
                $scope.queueInstances = [];

                $scope.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VR_Queueing_QueueInstanceAPIService.GetFilteredQueueInstances(dataRetrievalInput).then(function (queueInstancesResponse) {
                        onResponseReady(queueInstancesResponse);
                        createTimer();
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
                };

                $scope.getNewItemsColor = function (dataItem) {
                    var color = undefined;
                    var newItemsCount = dataItem.newItemsCount;
                    if (newItemsCount != undefined) {
                        if (newItemsCount >= 5 && newItemsCount < 15)
                            color = LabelColorsEnum.Warning.color;
                        else if (newItemsCount > 15)
                            color = LabelColorsEnum.Error.color;
                    }
                    return color;
                };

                $scope.getUnderProcessingColor = function (dataItem) {
                    var color = undefined;
                    var underProcessingCount = dataItem.underProcessingCount;
                    if (underProcessingCount != undefined) {

                        if (underProcessingCount >= 5 && underProcessingCount < 15)
                            color = LabelColorsEnum.Warning.color;

                        else if (underProcessingCount > 15)
                            color = LabelColorsEnum.Error.color;

                    }

                    return color;
                };

                $scope.getSuspendedColor = function (dataItem) {
                    var color = undefined;
                    var suspendedCount = dataItem.suspendedCount;
                    if (suspendedCount != undefined) {
                        if (suspendedCount > 0)
                            color = LabelColorsEnum.Error.color;
                    }

                    return color;
                };

                defineMenuActions();
            }

            function defineAPI() {
                var api = {};

                api.loadGrid = function (query) {
                    return gridAPI.retrieveData(query);
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {
                    ctrl.onReady(api);
                }
            }

            function createTimer() {
                if ($scope.jobIds) {
                    VRTimerService.unregisterJobByIds($scope.jobIds);
                    $scope.jobIds.length = 0;
                }

                VRTimerService.registerJob(onTimerElapsed, $scope, 5);
            }

            function onTimerElapsed() {
                return VR_Queueing_QueueItemHeaderAPIService.GetItemStatusSummary().then(function (itemStatusSummaryResponse) {
                    manipulateDataUpdated(itemStatusSummaryResponse);
                }).finally(function () {
                    $scope.isLoading = false;
                });
            }

            function manipulateDataUpdated(itemStatusSummaryResponse) {

                for (var i = 0; i < $scope.queueInstances.length; i++) {
                    var newItemsCount = 0;
                    var underProcessingCount = 0;
                    var suspendedCount = 0;

                    for (var j = 0; j < itemStatusSummaryResponse.length; j++) {
                        var status = itemStatusSummaryResponse[j].Status;
                        if (itemStatusSummaryResponse[j].QueueId == $scope.queueInstances[i].Entity.QueueInstanceId) {
                            if (status == VR_Queueing_QueueItemStatusEnum.New.value) {
                                newItemsCount += itemStatusSummaryResponse[j].Count;
                            }
                            else if (status == VR_Queueing_QueueItemStatusEnum.Processing.value || status == VR_Queueing_QueueItemStatusEnum.Failed.value) {
                                underProcessingCount += itemStatusSummaryResponse[j].Count;
                            }
                            else if (status == VR_Queueing_QueueItemStatusEnum.Suspended.value) {
                                suspendedCount += itemStatusSummaryResponse[j].Count;
                            }
                        }
                    }

                    $scope.queueInstances[i].newItemsCount = newItemsCount;
                    $scope.queueInstances[i].underProcessingCount = underProcessingCount;
                    $scope.queueInstances[i].suspendedCount = suspendedCount;
                }
            }

            function defineMenuActions() {
                $scope.gridMenuActions = [{
                    name: "Queue Items",
                    clicked: function (dataItem) {
                        var parameters = {
                            queueID: dataItem.Entity.QueueInstanceId
                        };
                        VR_Queueing_QueueInstanceService.showQueueItemInstances(parameters);
                    }
                }];
            }
        }

        return directiveDefinitionObject;
    }]);