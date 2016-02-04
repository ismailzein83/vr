"use strict";

app.directive("vrQueueingQueueinstanceGrid", ["VR_Queueing_QueueInstanceAPIService", "VR_Queueing_ExecutionFlowService", 'VRNotificationService', 'VR_Queueing_QueueItemStatusEnum','LabelColorsEnum',
    function (VR_Queueing_QueueInstanceAPIService, VR_Queueing_ExecutionFlowService, VRNotificationService, VR_Queueing_QueueItemStatusEnum, LabelColorsEnum) {

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
            var isFirstTimeLoaded = 0;
            this.initializeController = initializeController;

            function initializeController() {


                $scope.getNotProcessedColor = function (dataItem) {
                    var color = undefined;
                    var queueStatus = dataItem.notProcessedCount;
                    if (queueStatus != undefined) {
                        switch (true) {
                            case queueStatus >=5 && queueStatus < 15:
                                color = LabelColorsEnum.Warning.color;
                                break;
                            case queueStatus > 15:
                                color = LabelColorsEnum.Error.color;
                                break;
                        }
                    }

                    return color;
                }


                $scope.getSuspendedColor = function (dataItem) {
                    var color = undefined;
                    var queueStatus = dataItem.suspendedCount;
                    if (queueStatus != undefined) {
                        if (queueStatus > 0)
                            color = LabelColorsEnum.Error.color;
                    }

                    return color;
                }

                


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
                        }

                        return directiveAPI;
                    }
                };


                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    var allResult = [];
                    return VR_Queueing_QueueInstanceAPIService.GetFilteredQueueInstances(dataRetrievalInput)
                        .then(function (queueInstancesResponse) {
                            onResponseReady(queueInstancesResponse);
                            if (isFirstTimeLoaded == 0) {
                                isFirstTimeLoaded += 1;
                                RefreshDataGrid();
                            }
                        })
                        .catch(function (error) {
                            VRNotificationService.notifyExceptionWithClose(error, $scope);
                        });
                };
            }

            function RefreshDataGrid() {

                if ($scope.queueInstances != undefined) {
                    VR_Queueing_QueueInstanceAPIService.GetItemStatusSummary().then(function (itemStatusSummaryResponse) {
                        var notProcessedCount = 0;
                        var suspendedCount = 0;
                        if ($scope.queueInstances != undefined) {
                            for (var i = 0; i < $scope.queueInstances.length; i++) {
                                notProcessedCount = 0;
                                suspendedCount = 0;
                                for (var j = 0; j < itemStatusSummaryResponse.length; j++) {
                                    var Status = itemStatusSummaryResponse[j].Status;
                                    if (itemStatusSummaryResponse[j].QueueId == $scope.queueInstances[i].Entity.QueueInstanceId) {
                                        if (Status == VR_Queueing_QueueItemStatusEnum.New.value || Status == VR_Queueing_QueueItemStatusEnum.Processing.value || Status == VR_Queueing_QueueItemStatusEnum.Failed.value) {
                                            notProcessedCount += itemStatusSummaryResponse[j].Count;
                                        }
                                        else if (Status == VR_Queueing_QueueItemStatusEnum.Suspended.value) {
                                            suspendedCount += itemStatusSummaryResponse[j].Count;
                                        }
                                    }
                                }

                                $scope.queueInstances[i].notProcessedCount = notProcessedCount;
                                $scope.queueInstances[i].suspendedCount = suspendedCount;
                            }
                        }
                    })
                }
                setTimeout(RefreshDataGrid, 5000)

            }

        }



        return directiveDefinitionObject;

    }]);