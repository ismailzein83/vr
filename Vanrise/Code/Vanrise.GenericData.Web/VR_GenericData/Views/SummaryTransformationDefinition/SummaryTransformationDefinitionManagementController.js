(function (appControllers) {
    'use strict';

    SummaryTransformationDefinitionManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VR_GenericData_SummaryTransformationDefinitionService', 'VR_GenericData_SummaryTransformationDefinitionAPIService','VRNotificationService'];

    function SummaryTransformationDefinitionManagementController($scope, UtilsService, VRUIUtilsService, VR_GenericData_SummaryTransformationDefinitionService, summaryTransformationDefinitionAPIService, VRNotificationService) {

        var gridAPI;
        var filter = {};
        var devProjectDirectiveApi;
        var devProjectPromiseReadyDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();

        function defineScope() {
            $scope.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.loadGrid(filter);
            };

            $scope.search = function () {
                getFilterObject();
                return gridAPI.loadGrid(filter);
            };
            $scope.hasAddSummaryTransformationDefinition = function () {
                return summaryTransformationDefinitionAPIService.HasAddSummaryTransformationDefinition();
            };
            $scope.addSummaryTransformationDefinition = function () {
                var onSummaryTransformationDefinitionAdded = function (onSummaryTransformationDefinitionObj) {
                    gridAPI.onSummaryTransformationDefinitionAdded(onSummaryTransformationDefinitionObj);
                };

                VR_GenericData_SummaryTransformationDefinitionService.addSummaryTransformationDefinition(onSummaryTransformationDefinitionAdded);
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

        function getFilterObject() {
            filter = {
                Name: $scope.name,
                DevProjectIds: devProjectDirectiveApi.getSelectedIds()
            };
        }
    }

    appControllers.controller('VR_GenericData_SummaryTransformationDefinitionManagementController', SummaryTransformationDefinitionManagementController);

})(appControllers);
