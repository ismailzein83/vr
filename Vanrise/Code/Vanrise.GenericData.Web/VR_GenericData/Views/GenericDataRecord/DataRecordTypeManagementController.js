(function (appControllers) {
    'use strict';

    DataRecordTypeManagementController.$inject = ['$scope', 'UtilsService','VRUIUtilsService', 'VR_GenericData_DataRecordTypeService', 'VR_GenericData_DataRecordTypeAPIService'];

    function DataRecordTypeManagementController($scope, UtilsService, VRUIUtilsService, VR_GenericData_DataRecordTypeService,dataRecordTypeAPIService) {

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
            $scope.hasAddDataRecordType = function () {
                return dataRecordTypeAPIService.HasAddDataRecordType();
            };
            $scope.addDataRecordType = function () {
                var onDataRecordTypeAdded = function (onDataRecordTypeObj) {
                    gridAPI.onDataRecordTypeAdded(onDataRecordTypeObj);
                };

                VR_GenericData_DataRecordTypeService.addDataRecordType(onDataRecordTypeAdded);
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

    appControllers.controller('VR_GenericData_DataRecordTypeManagementController', DataRecordTypeManagementController);

})(appControllers);
