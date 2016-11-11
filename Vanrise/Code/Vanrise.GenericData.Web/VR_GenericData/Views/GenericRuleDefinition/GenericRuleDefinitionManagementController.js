(function (appControllers) {

    'use strict';

    GenericRuleDefinitionManagementController.$inject = ['$scope', 'VR_GenericData_GenericRuleDefinitionService', 'VR_GenericData_GenericRuleDefinitionAPIService'];

    function GenericRuleDefinitionManagementController($scope, VR_GenericData_GenericRuleDefinitionService, genericRuleDefinitionAPIService) {

        var gridAPI;
        var gridQuery = {};

        defineScope();
        load();

        function defineScope() {
            $scope.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.loadGrid(gridQuery);
            };

            $scope.search = function () {
                setGridQuery();
                return gridAPI.loadGrid(gridQuery);
            };
            $scope.hasAddGenericRuleDefinition = function () {
                return genericRuleDefinitionAPIService.HasAddGenericRuleDefinition();
            };
            $scope.addGenericRuleDefinition = function () {
                var onGenericRuleDefinitionAdded = function (addedGenericRuleDefinition) {
                    gridAPI.onGenericRuleDefinitionAdded(addedGenericRuleDefinition);
                };
                VR_GenericData_GenericRuleDefinitionService.addGenericRuleDefinition(onGenericRuleDefinitionAdded);
            };
        }

        function load() {

        }

        function setGridQuery() {
            gridQuery = {
                Name: $scope.name
            };
        }
    }

    appControllers.controller('VR_GenericData_GenericRuleDefinitionManagementController', GenericRuleDefinitionManagementController);

})(appControllers);