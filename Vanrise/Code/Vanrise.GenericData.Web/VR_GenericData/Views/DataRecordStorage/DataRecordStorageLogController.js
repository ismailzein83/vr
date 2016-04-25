(function (appControllers) {

    'use strict';

    DataRecordStorageLogController.$inject = ['$scope', 'VRValidationService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'VRModalService', 'VR_GenericData_DataRecordTypeService'];

    function DataRecordStorageLogController($scope, VRValidationService, UtilsService, VRUIUtilsService, VRNotificationService, VRModalService, VR_GenericData_DataRecordTypeService) {

        var dataRecordStorageSelectorAPI;
        var dataRecordStorageSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var gridAPI;
        var gridQuery = {};
        var filterObj;
        $scope.expression;
        $scope.selectedDataRecordStorage = undefined;

        defineScope();
        load();

        function defineScope() {
            $scope.onDataRecordStorageSelectorReady = function (api) {

                dataRecordStorageSelectorAPI = api;
                dataRecordStorageSelectorReadyDeferred.resolve();
            };

            $scope.onGridReady = function (api) {
                gridAPI = api;
            };

            $scope.search = function () {
                setGridQuery();
                return gridAPI.loadGrid(gridQuery);
            };

            $scope.addFilter = function () {
                var onDataRecordFieldTypeFilterAdded = function (filter, expression) {
                    filterObj = filter;
                    $scope.expression = expression;
                }
                VR_GenericData_DataRecordTypeService.addDataRecordTypeFieldFilter($scope.selectedDataRecordStorage.DataRecordTypeId, onDataRecordFieldTypeFilterAdded);
            };

            $scope.validateTimeRange = function () {
                return VRValidationService.validateTimeRange($scope.fromDate, $scope.toDate);
            }
        }

        function load() {
            $scope.isLoading = true;

            UtilsService.waitMultipleAsyncOperations([loadDataRecordStorageSelector]).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });

            function loadDataRecordStorageSelector() {
                var dataRecordStorageSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                dataRecordStorageSelectorReadyDeferred.promise.then(function () {
                    VRUIUtilsService.callDirectiveLoad(dataRecordStorageSelectorAPI, undefined, dataRecordStorageSelectorLoadDeferred);
                });

                return dataRecordStorageSelectorLoadDeferred.promise;
            }
        }

        function setGridQuery() {
            gridQuery = {
                DataRecordStorageId: dataRecordStorageSelectorAPI.getSelectedIds(),
                FromTime: $scope.fromDate,
                ToTime: $scope.toDate,
                FilterGroup: filterObj
            };
        }
    }

    appControllers.controller('VR_GenericData_DataRecordStorageLogController', DataRecordStorageLogController);

})(appControllers);