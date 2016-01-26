(function (appControllers) {
    'use strict';

    ExecutionFlowController.$inject = ['$scope', 'ExecutionFlowService', 'UtilsService'];

    function ExecutionFlowController($scope, ExecutionFlowService, UtilsService) {

        var gridAPI;
        var filter = {};

        var executionFlowDirectionSelectorAPI;
        var executionFlowDirectionSelectorReadyDeferred = UtilsService.createPromiseDeferred();


        defineScope();
        load();

        function defineScope() {

            $scope.onExecutionFlowDefinitionSelectorReady = function (api) {
                executionFlowDirectionSelectorAPI = api;
                executionFlowDirectionSelectorReadyDeferred.resolve();
            };


            $scope.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.loadGrid(filter);
            };

            $scope.search = function () {
                getFilterObject();
                gridAPI.loadGrid(filter);
            };

            $scope.addExecutionFlow = function () {
                var onExecutionFlowAdded = function (executionFlowObj) {
                    if (gridAPI) {
                        gridAPI.onExecutionFlowAdded(executionFlowObj);
                    }
                };
                ExecutionFlowService.addExecutionFlow(onExecutionFlowAdded);
            };
        }

        function load() {

        }

        function getFilterObject() {
            filter = {
                ID: $scope.name,
                Name: $scope.email
            };
        }
    }

    appControllers.controller('ExecutionFlowController', ExecutionFlowController);

})(appControllers);
