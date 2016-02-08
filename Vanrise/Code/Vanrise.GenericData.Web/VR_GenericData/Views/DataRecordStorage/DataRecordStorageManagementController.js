(function (appControllers) {

    'use strict';

    DataRecordStorageManagementController.$inject = ['$scope', 'VR_GenericData_DataRecordStorageService', 'UtilsService'];

    function DataRecordStorageManagementController($scope, VR_GenericData_DataRecordStorageService, UtilsService) {

        var dataRecordTypeSelectorAPI;
        var dataRecordTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var gridAPI;
        var gridQuery = {};

        defineScope();
        load();

        function defineScope() {
            $scope.onDataRecordTypeSelectorReady = function (api) {
                dataRecordTypeSelectorAPI = api;
                dataRecordTypeSelectorReadyDeferred.resolve();
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
            function loadDataStoreSelector() { }
        }

        function setGridQuery() {
            gridQuery = {
                Name: $scope.name,
                DataRecordTypeIds: null,
                DataStoreIds: null
            };
        }
    }

    appControllers.controller('VR_GenericData_DataRecordStorageManagementController', DataRecordStorageManagementController);

})(appControllers);