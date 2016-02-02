"use strict";

app.directive("vrExecutionflowGrid", ["VR_Queueing_ExecutionFlowAPIService", "VR_Queueing_ExecutionFlowService", 'VRNotificationService', 'VRUIUtilsService',
    function (VR_Queueing_ExecutionFlowAPIService, VR_Queueing_ExecutionFlowService, VRNotificationService, VRUIUtilsService) {

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
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
            };
            defineMenuActions();
        }
        

    

        function defineMenuActions() {
            $scope.gridMenuActions = [{
                name: "Edit",
               clicked: editExecutionFlow
            }];
        }

        function editExecutionFlow(executionFlowObj) {
            var onExecutionFlowUpdated = function (executionFlowObj) {
                gridAPI.itemUpdated(executionFlowObj);
            }

            VR_Queueing_ExecutionFlowService.editExecutionFlow(executionFlowObj.Entity.ExecutionFlowId, onExecutionFlowUpdated);
        }

        
    }

    return directiveDefinitionObject;

}]);