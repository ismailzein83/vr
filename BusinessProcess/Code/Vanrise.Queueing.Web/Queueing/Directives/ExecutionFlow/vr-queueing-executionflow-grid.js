"use strict";

app.directive("vrQueueingExecutionflowGrid", ["VR_Queueing_ExecutionFlowAPIService", "VR_Queueing_ExecutionFlowService", 'VRNotificationService', 'VRUIUtilsService','VR_Queueing_QueueItemStatusEnum',
    function (VR_Queueing_ExecutionFlowAPIService, VR_Queueing_ExecutionFlowService, VRNotificationService, VRUIUtilsService, VR_Queueing_QueueItemStatusEnum) {

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
        templateUrl: "/Client/Modules/Queueing/Directives/ExecutionFlow/Templates/ExecutionFlowGrid.html"

    };

    function ExecutionFlowGrid($scope, ctrl, $attrs) {

        var gridAPI;
        var gridDrillDownTabsObj;
        this.initializeController = initializeController;

        function initializeController() {

            $scope.executionFlows = [];
            $scope.ongridReady = function (api) {
                gridAPI = api;

                var drillDownDefinitions = VR_Queueing_ExecutionFlowService.getDrillDownDefinition();
                 gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {

                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                     
                        return gridAPI.retrieveData(query);
                    }

                    directiveAPI.onExecutionFlowAdded = function (executionFlowObject) {
                        gridDrillDownTabsObj.setDrillDownExtensionObject(executionFlowObject);
                        gridAPI.itemAdded(executionFlowObject);
                    }


                    return directiveAPI;
                }
            };


            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return VR_Queueing_ExecutionFlowAPIService.GetFilteredExecutionFlows(dataRetrievalInput)
                    .then(function (response) {
                        if (response.Data != undefined) {
                            for (var i = 0; i < response.Data.length; i++) {
                                gridDrillDownTabsObj.setDrillDownExtensionObject(response.Data[i]);
                            }
                        }

                        onResponseReady(response);
                        refreshExecutionFlowGrid();
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
            };
            defineMenuActions();
        }
        
        function refreshExecutionFlowGrid() {
            VR_Queueing_ExecutionFlowAPIService.GetExecutionFlowStatusSummary().then(function (response) {
                if ($scope.executionFlows != undefined) {
                    var notProcessedCount;
                    var suspendedCount;
                    for (var i = 0; i < $scope.executionFlows.length; i++) {
                        notProcessedCount = 0;
                        suspendedCount = 0;
                        for (var j = 0; j < response.length; j++) {
                            if ($scope.executionFlows[i].Entity.ExecutionFlowId == response[j].ExecutionFlowId) {
                                if (response[j].Status == VR_Queueing_QueueItemStatusEnum.New.value || response[j].Status == VR_Queueing_QueueItemStatusEnum.Processing.value || response[j].Status == VR_Queueing_QueueItemStatusEnum.Failed.value) {
                                    notProcessedCount += response[j].Count;
                                }
                                else if (response[j].Status == VR_Queueing_QueueItemStatusEnum.Suspended.value) {
                                    suspendedCount += response[j].Count;
                                }

                            }
                        }
                        $scope.executionFlows[i].notProcessedCount = notProcessedCount;
                        $scope.executionFlows[i].suspendedCount = suspendedCount;

                    }
                }
            })

            setTimeout(refreshExecutionFlowGrid, 5000);

        }

        function defineMenuActions() {
            $scope.gridMenuActions = [{
                name: "Edit",
               clicked: editExecutionFlow
            }];
        }

        function editExecutionFlow(executionFlowObj) {
            var onExecutionFlowUpdated = function (executionFlowObj) {
                gridDrillDownTabsObj.setDrillDownExtensionObject(executionFlowObj);
                gridAPI.itemUpdated(executionFlowObj);
            }

            VR_Queueing_ExecutionFlowService.editExecutionFlow(executionFlowObj.Entity.ExecutionFlowId, onExecutionFlowUpdated);
        }

        
    }

    return directiveDefinitionObject;

}]);