"use strict";

app.directive("vrExecutionflowGrid", ["VR_Queueing_ExecutionFlowAPIService", "VR_Queueing_ExecutionFlowService", 'VRNotificationService', function (VR_Queueing_ExecutionFlowAPIService, VR_Queueing_ExecutionFlowService, VRNotificationService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var userGrid = new UsersGrid($scope, ctrl, $attrs);
            userGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Queueing/Directives/ExecutionFlow/Templates/ExecutionFlowGrid.html"

    };

    function UsersGrid($scope, ctrl, $attrs) {

        var gridAPI;
        this.initializeController = initializeController;

        function initializeController() {

            $scope.executionFlows = [];

            $scope.onGridReady = function (api) {
                gridAPI = api;
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {

                    var directiveAPI = {};

                    directiveAPI.loadGrid = function (query) {
                        return gridAPI.retrieveData(query);
                    }

                    directiveAPI.onUserAdded = function (userObject) {
                        gridAPI.itemAdded(userObject);
                    }

                    return directiveAPI;
                }
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return VR_Queueing_ExecutionFlowAPIService.GetFilteredExecutionFlows(dataRetrievalInput)
                    .then(function (response) {
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

            VR_Queueing_ExecutionFlowService.editExecutionFlow(executionFlowObj.ID, onExecutionFlowUpdated);
        }

    }

    return directiveDefinitionObject;

}]);