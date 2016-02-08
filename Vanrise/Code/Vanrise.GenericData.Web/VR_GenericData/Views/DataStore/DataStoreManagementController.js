(function (appControllers) {
    'use strict';

    DataStoreManagementController.$inject = ['$scope', 'VR_GenericData_DataStoreService'];

    function DataStoreManagementController($scope, VR_GenericData_DataStoreService) {

        var gridAPI;
        var filter = {};

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

            $scope.addDataStore = function () {
                var onDataStoreAdded = function (dataStoreObj) {
                   // gridAPI.onDataStoreAdded(dataStoreObj);
                };

                VR_GenericData_DataStoreService.addDataStore(onDataStoreAdded);
            };
        }

        function load() {

        }

        function getFilterObject() {
            filter = {
                Name: $scope.name,
            };
        }
    }

    appControllers.controller('VR_GenericData_DataStoreManagementController', DataStoreManagementController );

})(appControllers);
