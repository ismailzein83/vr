(function (appControllers) {

    'use strict';

    GenericRuleDefinitionManagementController.$inject = ['$scope'];

    function GenericRuleDefinitionManagementController($scope) {

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
                var onUserAdded = function (userObj) {
                    gridAPI.onUserAdded(userObj);
                };

                VR_Sec_UserService.addUser(onUserAdded);
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