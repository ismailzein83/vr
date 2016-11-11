(function (appControllers) {
    'use strict';

    DataStoreManagementController.$inject = ['$scope', 'VR_GenericData_DataStoreService', 'VR_GenericData_DataStoreAPIService'];

    function DataStoreManagementController($scope, VR_GenericData_DataStoreService, VR_GenericData_DataStoreAPIService) {

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
            $scope.hasAddDataStore = function () {
                return VR_GenericData_DataStoreAPIService.HasAddDataStore();
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
