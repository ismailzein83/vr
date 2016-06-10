(function (appControllers) {

    'use strict';

    ServiceTypeManagementController.$inject = ['$scope',  'UtilsService'];

    function ServiceTypeManagementController($scope,  UtilsService) {
        var gridAPI;

        defineScope();
        load();

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.load({});
            };

            $scope.scopeModel.search = function () {
                var query = buildGridQuery();
                return gridAPI.load(query);
            };

        }

        function load() {

        }

        function buildGridQuery() {
            return {
                Name: $scope.scopeModel.name,
            };
        }
    }

    appControllers.controller('Retail_BE_ServiceTypeManagementController', ServiceTypeManagementController);

})(appControllers);