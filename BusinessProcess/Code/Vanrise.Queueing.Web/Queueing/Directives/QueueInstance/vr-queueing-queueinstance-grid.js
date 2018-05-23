"use strict";

app.directive("vrQueueingQueueinstanceGrid", ["VR_Queueing_QueueInstanceAPIService", "VR_Queueing_QueueInstanceService", "VR_Queueing_QueueItemHeaderAPIService", "VR_Queueing_ExecutionFlowService", 'VRNotificationService', 'VR_Queueing_QueueItemStatusEnum', 'LabelColorsEnum',
    function (VR_Queueing_QueueInstanceAPIService, VR_Queueing_QueueInstanceService, VR_Queueing_QueueItemHeaderAPIService, VR_Queueing_ExecutionFlowService, VRNotificationService, VR_Queueing_QueueItemStatusEnum, LabelColorsEnum) {

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
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Queueing/Directives/QueueInstance/Templates/QueueInstanceGrid.html"

        };

        function ExecutionFlowGrid($scope, ctrl, $attrs) {

            var gridAPI;
            var isFirstTimeLoaded = false;
            this.initializeController = initializeController;

            function initializeController() {

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

                


                $scope.queueInstances = [];
                $scope.ongridReady = function (api) {
                    gridAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {
                        ctrl.onReady(getDirectiveAPI());

                    }

                    function getDirectiveAPI() {

                        var directiveAPI = {};
                        directiveAPI.loadGrid = function (query) {

                            return gridAPI.retrieveData(query);
                        };

                        return directiveAPI;
                    }
                };


                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    var allResult = [];
                    return VR_Queueing_QueueInstanceAPIService.GetFilteredQueueInstances(dataRetrievalInput)
                        .then(function (queueInstancesResponse) {
                            onResponseReady(queueInstancesResponse);
                            RefreshDataGrid();
                            var timer;
                            if (!isFirstTimeLoaded) {
                                isFirstTimeLoaded = true;
                                RefreshDataGrid();
                                timer = setInterval(function(){
                                    RefreshDataGrid();                                
                                }, 5000);
                            }

                            $scope.$on('$destroy', function () {                               
                                clearInterval(timer);
                            });
                        })
                        .catch(function (error) {
                            VRNotificationService.notifyExceptionWithClose(error, $scope);
                        });
                };

                defineMenuActions();
            }

            function RefreshDataGrid() {

                if ($scope.queueInstances != undefined) {
                    VR_Queueing_QueueItemHeaderAPIService.GetItemStatusSummary().then(function (itemStatusSummaryResponse) {
                        var newItemsCount = 0;
                        var underProcessingCount = 0;
                        var suspendedCount = 0;
                        if ($scope.queueInstances != undefined) {
                            for (var i = 0; i < $scope.queueInstances.length; i++) {
                                newItemsCount = 0;
                                underProcessingCount = 0;
                                suspendedCount = 0;
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
                    });
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