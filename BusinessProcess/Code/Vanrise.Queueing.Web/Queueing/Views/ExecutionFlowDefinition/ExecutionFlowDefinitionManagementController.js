(function (appControllers) {
    'use strict';

    ExecutionFlowDefinitionController.$inject = ['$scope', 'VR_Queueing_ExecutionFlowDefinitionService', 'VR_Queueing_ExecutionFlowDefinitionAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

    function ExecutionFlowDefinitionController($scope, VR_Queueing_ExecutionFlowDefinitionService, VR_Queueing_ExecutionFlowDefinitionAPIService, UtilsService, VRUIUtilsService, VRNotificationService) {

        var gridAPI;
        var devProjectDirectiveApi;
        var devProjectPromiseReadyDeferred = UtilsService.createPromiseDeferred();

        var filter = {};
        defineScope();
        load();
        function defineScope() {

            $scope.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.loadGrid(filter);
            };
            $scope.onDevProjectSelectorReady = function (api) {
                devProjectDirectiveApi = api;
                devProjectPromiseReadyDeferred.resolve();
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
        function load() {
            $scope.isLoading = true;
            loadAllControls();
        }
        function loadAllControls() {
            return UtilsService.waitPromiseNode({ promises: [loadDevProjectSelector()] })
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
                .finally(function () {
                    $scope.isLoading = false;
                });
        }
        function loadDevProjectSelector() {
            var devProjectPromiseLoadDeferred = UtilsService.createPromiseDeferred();
            devProjectPromiseReadyDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(devProjectDirectiveApi, undefined, devProjectPromiseLoadDeferred);
            });
            return devProjectPromiseLoadDeferred.promise;
        }
        function getFilterObject() {
            var filter = {
                Name: $scope.name,
                Title: $scope.dtitle,
                DevProjectIds: devProjectDirectiveApi != undefined ? devProjectDirectiveApi.getSelectedIds() : undefined
            };
            return filter;
        }
    }

    appControllers.controller('VR_Queueing_ExecutionFlowDefinitionController', ExecutionFlowDefinitionController);

})(appControllers);
