(function (appControllers) {

    'use strict';

    DataRecordStorageManagementController.$inject = ['$scope', 'VR_GenericData_DataRecordStorageService', 'VR_GenericData_DataRecordStorageAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

    function DataRecordStorageManagementController($scope, VR_GenericData_DataRecordStorageService, dataRecordStorageAPIService, UtilsService, VRUIUtilsService, VRNotificationService) {

        var dataRecordTypeSelectorAPI;
        var dataRecordTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var dataStoreSelectorAPI;
        var dataStoreSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var gridAPI;
        var gridQuery = {};

        defineScope();
        load();

        function defineScope() {
            $scope.onDataRecordTypeSelectorReady = function (api) {
                dataRecordTypeSelectorAPI = api;
                dataRecordTypeSelectorReadyDeferred.resolve();
            };
            $scope.onDataStoreSelectorReady = function (api) {
                dataStoreSelectorAPI = api;
                dataStoreSelectorReadyDeferred.resolve();
            };
            $scope.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.loadGrid(gridQuery);
            };

            $scope.search = function () {
                setGridQuery();
                return gridAPI.loadGrid(gridQuery);
            };
            $scope.addDataRecordStorage = function () {
                var onDataRecordStorageAdded = function (addedDataRecordStorage) {
                    gridAPI.onDataRecordStorageAdded(addedDataRecordStorage);
                };
                VR_GenericData_DataRecordStorageService.addDataRecordStorage(onDataRecordStorageAdded);
            };
            $scope.hasAddDataRecordStorage = function () {
                return dataRecordStorageAPIService.HasAddDataRecordStorage();
            };
        }

        function load() {
            $scope.isLoading = true;

            UtilsService.waitMultipleAsyncOperations([loadDataRecordTypeSelector, loadDataStoreSelector]).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });

            function loadDataRecordTypeSelector() {
                var dataRecordTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                dataRecordTypeSelectorReadyDeferred.promise.then(function () {
                    VRUIUtilsService.callDirectiveLoad(dataRecordTypeSelectorAPI, undefined, dataRecordTypeSelectorLoadDeferred);
                });

                return dataRecordTypeSelectorLoadDeferred.promise;
            }
            function loadDataStoreSelector() {
                var dataStoreSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                dataStoreSelectorReadyDeferred.promise.then(function () {
                    VRUIUtilsService.callDirectiveLoad(dataStoreSelectorAPI, undefined, dataStoreSelectorLoadDeferred);
                });

                return dataStoreSelectorLoadDeferred.promise;
            }
        }

        function setGridQuery() {
            gridQuery = {
                Name: $scope.name,
                DataRecordTypeIds: dataRecordTypeSelectorAPI.getSelectedIds(),
                DataStoreIds: dataStoreSelectorAPI.getSelectedIds()
            };
        }
    }

    appControllers.controller('VR_GenericData_DataRecordStorageManagementController', DataRecordStorageManagementController);

})(appControllers);