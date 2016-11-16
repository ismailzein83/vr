(function (appControllers) {
    'use strict';

    ExecutionFlowController.$inject = ['$scope', 'VR_Queueing_ExecutionFlowService', 'VR_Queueing_ExecutionFlowAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

    function ExecutionFlowController($scope, VR_Queueing_ExecutionFlowService, VR_Queueing_ExecutionFlowAPIService , UtilsService, VRUIUtilsService, VRNotificationService) {

        var gridApi;
        

        var executionFlowDefinitionSelectorAPI;
        var executionFlowDirectionSelectorReadyDeferred = UtilsService.createPromiseDeferred();


        defineScope();
        load();

        function defineScope() {

            $scope.onExecutionFlowDefinitionSelectorReady = function (api) {
                executionFlowDefinitionSelectorAPI = api;
                executionFlowDirectionSelectorReadyDeferred.resolve();
            };

            $scope.selectedExecutionFlowDefinition = [];

            $scope.onGridReady = function (api) {
                gridApi = api;
               var filter = {};
             gridApi.loadGrid(filter);
            };

            $scope.search = function () {
               return gridApi.loadGrid(getFilterObject());
            };

            $scope.addExecutionFlow = function () {
                var onExecutionFlowAdded = function (executionFlowObj) {
                    if (gridApi) {
                        gridApi.onExecutionFlowAdded(executionFlowObj);
                    }
                };
                VR_Queueing_ExecutionFlowService.addExecutionFlow(onExecutionFlowAdded);
            };
            $scope.hasAddExecutionFlow = function () {
                return VR_Queueing_ExecutionFlowAPIService.HasAddExecutionFlow();
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
          var  filter = {
                DefinitionId: executionFlowDefinitionSelectorAPI.getSelectedIds(),
                Name: $scope.name
            };
            return filter;
            
        }
    }

    appControllers.controller('VR_Queueing_ExecutionFlowController', ExecutionFlowController);

})(appControllers);
