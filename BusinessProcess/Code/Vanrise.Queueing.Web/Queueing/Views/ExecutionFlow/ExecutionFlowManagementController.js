(function (appControllers) {
    'use strict';

    ExecutionFlowController.$inject = ['$scope', 'VR_Queueing_ExecutionFlowService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

    function ExecutionFlowController($scope, VR_Queueing_ExecutionFlowService, UtilsService, VRUIUtilsService, VRNotificationService) {

        var gridAPI;
        var filter = {};

        var executionFlowDefinitionSelectorAPI;
        var executionFlowDirectionSelectorReadyDeferred = UtilsService.createPromiseDeferred();


        defineScope();
        load();

        function defineScope() {

            $scope.onExecutionFlowDefinitionSelectorReady = function (api) {
                executionFlowDefinitionSelectorAPI = api;
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
                VR_Queueing_ExecutionFlowService.addExecutionFlow(onExecutionFlowAdded);
            };
        }

        function load() {
            loadAllControls();
        }

        function loadAllControls() {
            $scope.isLoading = true;
            return UtilsService.waitMultipleAsyncOperations([loadExecutionFlowDefinition])
              .catch(function (error) {
                  VRNotificationService.notifyExceptionWithClose(error, $scope);
              })
             .finally(function () {
                 $scope.isLoading = false;
             });
        }


        function loadExecutionFlowDefinition() {
            var executionFlowDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            executionFlowDirectionSelectorReadyDeferred.promise.then(function () {
                var payload = {
                    filter: null,
                    selectedIds: null
                };

                VRUIUtilsService.callDirectiveLoad(executionFlowDefinitionSelectorAPI, payload, executionFlowDefinitionSelectorLoadDeferred);
            });
            return executionFlowDefinitionSelectorLoadDeferred.promise;
        }



        function getFilterObject() {
            filter = {
                ID: $scope.selectedExecutionFlowDefinition,
                Name: $scope.name
            };
        }
    }

    appControllers.controller('ExecutionFlowController', ExecutionFlowController);

})(appControllers);
