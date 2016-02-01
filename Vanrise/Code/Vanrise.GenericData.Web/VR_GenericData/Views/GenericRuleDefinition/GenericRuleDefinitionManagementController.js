(function (appControllers) {

    'use strict';

    GenericRuleDefinitionManagementController.$inject = ['$scope', 'VR_GenericData_GenericRuleDefinitionService'];

    function GenericRuleDefinitionManagementController($scope, VR_GenericData_GenericRuleDefinitionService) {

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

            $scope.addGenericRuleDefinition = function () {
                var onGenericRuleDefinitionAdded = function (addedGenericRuleDefinition) {
                    gridAPI.onUserAdded(addedGenericRuleDefinition);
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