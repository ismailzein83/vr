(function (appControllers) {
    'use strict';

    DataStoreManagementController.$inject = ['$scope', 'VR_GenericData_DataStoreService'];

    function DataStoreManagementController($scope, VR_GenericData_DataStoreService) {

        var DatStoreGridAPI;
        var filter = {};

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

            $scope.addDataStore = function () {
                var onDataStoreAdded = function (dataStoreObj) {
                    DatStoreGridAPI.onDataStoreAdded(dataStoreObj);
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
