(function (appControllers) {
    'use strict';

    ExecutionFlowDefinitionController.$inject = ['$scope', 'VR_Queueing_ExecutionFlowDefinitionService', 'VR_Queueing_ExecutionFlowDefinitionAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

    function ExecutionFlowDefinitionController($scope, VR_Queueing_ExecutionFlowDefinitionService, VR_Queueing_ExecutionFlowDefinitionAPIService, UtilsService, VRUIUtilsService, VRNotificationService) {

        var gridAPI;
        var filter = {};
        defineScope();

        function defineScope() {

            $scope.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.loadGrid(filter);
            };

            $scope.search = function () {
                filter=getFilterObject();
                return gridAPI.loadGrid(filter);
            };

            $scope.addExecutionFlowDefinition = function () {
                var onExecutionFlowDefinitionAdded = function (executionFlowDefinitionObj) {
                    if (gridAPI) {
                        gridAPI.onExecutionFlowDefinitionAdded(executionFlowDefinitionObj);
                    }
                };
                VR_Queueing_ExecutionFlowDefinitionService.addExecutionFlowDefinition(onExecutionFlowDefinitionAdded);
            };
            $scope.hasAddExecutionFlowDefinition = function () {
                return VR_Queueing_ExecutionFlowDefinitionAPIService.HasAddExecutionFlowDefinition();
            };
        }

        function getFilterObject() {
            var filter = {
                Name: $scope.name,
                Title: $scope.dtitle
            };
            return filter;

        }
    }

    appControllers.controller('VR_Queueing_ExecutionFlowDefinitionController', ExecutionFlowDefinitionController);

})(appControllers);
