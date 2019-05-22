(function (appControllers) {
    'use strict';

    DataTransformationDefinitionManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VR_GenericData_DataTransformationDefinitionService', 'VR_GenericData_DataTransformationDefinitionAPIService'];

    function DataTransformationDefinitionManagementController($scope, UtilsService, VRUIUtilsService, VR_GenericData_DataTransformationDefinitionService, dataTransformationDefinitionAPIService) {

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

            $scope.addDataTransformationDefinition = function () {
                var onDataTransformationDefinitionAdded = function (onDataTransformationDefinitionObj) {
                    gridAPI.onDataTransformationDefinitionAdded(onDataTransformationDefinitionObj);
                };

                VR_GenericData_DataTransformationDefinitionService.addDataTransformationDefinition(onDataTransformationDefinitionAdded);
            };
            $scope.hasAddDataTransformationDefinition = function () {
                return dataTransformationDefinitionAPIService.HasAddDataTransformationDefinition();
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

    appControllers.controller('VR_GenericData_DataTransformationDefinitionManagementController', DataTransformationDefinitionManagementController);

})(appControllers);
