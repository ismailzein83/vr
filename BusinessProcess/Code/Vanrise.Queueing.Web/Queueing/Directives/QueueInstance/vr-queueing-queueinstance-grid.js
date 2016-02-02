"use strict";

app.directive("vrQueueingQueueinstanceGrid", ["VR_Queueing_QueueInstanceAPIService", "VR_Queueing_ExecutionFlowService", 'VRNotificationService', 'VR_Queueing_QueueItemStatusEnum',
    function (VR_Queueing_QueueInstanceAPIService, VR_Queueing_ExecutionFlowService, VRNotificationService, VR_Queueing_QueueItemStatusEnum) {

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
            this.initializeController = initializeController;

            function initializeController() {



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
                            for (var i = 0; i < queueInstancesResponse.Data.length; i++) {

                                queueInstancesResponse.Data[i].notProcessedCount = 0;
                                queueInstancesResponse.Data[i].suspendedCount = 0;

                            }
                            var result = { Data: queueInstancesResponse.Data };
                            onResponseReady(result);
                            RefreshDataGrid();
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
                                    if (itemStatusSummaryResponse[j].QueueId == $scope.queueInstances[i].Entity.QueueInstanceId) {
                                        if (itemStatusSummaryResponse[j].Status == VR_Queueing_QueueItemStatusEnum.New.value || itemStatusSummaryResponse[j].Status == VR_Queueing_QueueItemStatusEnum.Processing.value || itemStatusSummaryResponse[j].Status == VR_Queueing_QueueItemStatusEnum.Failed.value) {
                                            notProcessedCount += itemStatusSummaryResponse[j].Count;
                                        }
                                        else if (itemStatusSummaryResponse[j].Status == VR_Queueing_QueueItemStatusEnum.Suspended.value) {
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