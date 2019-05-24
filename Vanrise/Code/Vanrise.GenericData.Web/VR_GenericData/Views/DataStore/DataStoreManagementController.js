(function (appControllers) {
    'use strict';

    DataStoreManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VR_GenericData_DataStoreService', 'VR_GenericData_DataStoreAPIService','VRNotificationService'];

    function DataStoreManagementController($scope, UtilsService, VRUIUtilsService, VR_GenericData_DataStoreService, VR_GenericData_DataStoreAPIService, VRNotificationService) {

        var DatStoreGridAPI;
        var filter = {};
        var devProjectDirectiveApi;
        var devProjectPromiseReadyDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();

        function defineScope() {
            $scope.onGridReady = function (api) {
                DatStoreGridAPI = api;
                DatStoreGridAPI.loadGrid(filter);
            };

            $scope.search = function () {
                getFilterObject();
                return DatStoreGridAPI.loadGrid(filter);
            };
            $scope.hasAddDataStore = function () {
                return VR_GenericData_DataStoreAPIService.HasAddDataStore();
            };
            $scope.addDataStore = function () {
                var onDataStoreAdded = function (dataStoreObj) {
                    DatStoreGridAPI.onDataStoreAdded(dataStoreObj);
                };

                VR_GenericData_DataStoreService.addDataStore(onDataStoreAdded);
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

    appControllers.controller('VR_GenericData_DataStoreManagementController', DataStoreManagementController );

})(appControllers);
