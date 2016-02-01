(function (appControllers) {
    'use strict';

    DataTransformationDefinitionManagementController.$inject = ['$scope', 'VR_GenericData_DataRecordTypeService'];

    function DataTransformationDefinitionManagementController($scope, VR_GenericData_DataRecordTypeService) {

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

            $scope.addDataTransformationDefinition = function () {
                var onDataTransformationDefinitionAdded = function (onDataTransformationDefinitionObj) {
                    gridAPI.onDataTransformationDefinitionAdded(onDataTransformationDefinitionObj);
                };

                VR_GenericData_DataRecordTypeService.addDataTransformationDefinition(onDataTransformationDefinitionAdded);
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

    appControllers.controller('VR_GenericData_DataTransformationDefinitionManagementController', DataTransformationDefinitionManagementController);

})(appControllers);
