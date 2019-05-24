(function (appControllers) {

    'use strict';

    GenericRuleDefinitionManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VR_GenericData_GenericRuleDefinitionService', 'VR_GenericData_GenericRuleDefinitionAPIService','VRNotificationService'];

    function GenericRuleDefinitionManagementController($scope, UtilsService, VRUIUtilsService, VR_GenericData_GenericRuleDefinitionService, genericRuleDefinitionAPIService, VRNotificationService) {

        var gridAPI;
        var gridQuery = {};
        var devProjectDirectiveApi;
        var devProjectPromiseReadyDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();

        function defineScope() {
            $scope.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.loadGrid(gridQuery);
            };

            $scope.search = function () {
                setGridQuery();
                return gridAPI.loadGrid(gridQuery);
            };
            $scope.hasAddGenericRuleDefinition = function () {
                return genericRuleDefinitionAPIService.HasAddGenericRuleDefinition();
            };
            $scope.addGenericRuleDefinition = function () {
                var onGenericRuleDefinitionAdded = function (addedGenericRuleDefinition) {
                    gridAPI.onGenericRuleDefinitionAdded(addedGenericRuleDefinition);
                };
                VR_GenericData_GenericRuleDefinitionService.addGenericRuleDefinition(onGenericRuleDefinitionAdded);
            };
            $scope.onDevProjectSelectorReady = function (api) {
                devProjectDirectiveApi = api;
                devProjectPromiseReadyDeferred.resolve();
            };
        }

        function load() {
            $scope.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadDevProjectSelector])
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

        function setGridQuery() {
            gridQuery = {
                Name: $scope.name,
                DevProjectIds: devProjectDirectiveApi.getSelectedIds()
            };
        }
    }

    appControllers.controller('VR_GenericData_GenericRuleDefinitionManagementController', GenericRuleDefinitionManagementController);

})(appControllers);