(function (appControllers) {

    'use strict';

    ServiceTypeManagementController.$inject = ['$scope',  'UtilsService'];

    function ServiceTypeManagementController($scope,  UtilsService) {
        var gridAPI;

        defineScope();
        load();

        function defineScope() {

            $scope.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.loadGrid({});
            };

            $scope.search = function () {
                var query = buildGridQuery();
                return gridAPI.loadGrid(query);
            };

        }

        function load() {

        }

        function buildGridQuery() {
            return {
                Name: $scope.name,
            };
        }
    }

    appControllers.controller('Retail_BE_ServiceTypeManagementController', ServiceTypeManagementController);

})(appControllers);