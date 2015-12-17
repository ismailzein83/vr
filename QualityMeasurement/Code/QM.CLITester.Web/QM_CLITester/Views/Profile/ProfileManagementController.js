(function (appControllers) {

    "use strict";

    profileManagementController.$inject = ['$scope'];

    function profileManagementController($scope) {
        var gridAPI;

        defineScope();
        load();

        function defineScope() {
            $scope.onGridReady = function (api) {
                gridAPI = api;
                api.loadGrid({});
            }

            $scope.searchClicked = function () {
                if (gridAPI != undefined)
                    return gridAPI.loadGrid(getFilterObject());
            };


            function getFilterObject() {
                var query = {
                    Name: $scope.name
                };
                return query;
            }
        }

        function load() {
        }

    }

    appControllers.controller('QM_CLITester_ProfileManagementController', profileManagementController);
})(appControllers);