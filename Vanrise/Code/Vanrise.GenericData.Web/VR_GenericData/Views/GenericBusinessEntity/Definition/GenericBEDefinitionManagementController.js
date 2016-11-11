(function (appControllers) {
    'use strict';

    GenericBEDefinitionManagementController.$inject = ['$scope', 'VR_GenericData_GenericBEService', 'VR_GenericData_BusinessEntityDefinitionAPIService'];

    function GenericBEDefinitionManagementController($scope, VR_GenericData_GenericBEService, businessEntityDefinitionAPIService) {

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
            $scope.hasAddGenericBEPermission = function () {
                return businessEntityDefinitionAPIService.HasAddBusinessEntityDefinition();
            };
            $scope.addGenericBE = function () {
                var onGenericBEAdded = function (onGenericBusinessEntityDefinitionObj) {
                    gridAPI.onGenericBusinessEntityDefinitionAdded(onGenericBusinessEntityDefinitionObj);
                };

                VR_GenericData_GenericBEService.addGenericBE(onGenericBEAdded);
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

    appControllers.controller('VR_GenericData_GenericBEDefinitionManagementController', GenericBEDefinitionManagementController);

})(appControllers);
