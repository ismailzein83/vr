(function (appControllers) {
    'use strict';

    DataRecordTypeManagementController.$inject = ['$scope', 'VR_GenericData_DataRecordTypeService'];

    function DataRecordTypeManagementController($scope, VR_GenericData_DataRecordTypeService) {

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

            $scope.addDataRecordType = function () {
                var onDataRecordTypeAdded = function (onDataRecordTypeObj) {
                    gridAPI.onDataRecordTypeAdded(onDataRecordTypeObj);
                };

                VR_GenericData_DataRecordTypeService.addDataRecordType(onDataRecordTypeAdded);
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

    appControllers.controller('VR_GenericData_DataRecordTypeManagementController', DataRecordTypeManagementController);

})(appControllers);
