(function (appControllers) {

    "use strict";

    profileManagementController.$inject = ['$scope', 'QM_CLITester_ProfileService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

    function profileManagementController($scope, QM_CLITester_ProfileService, UtilsService, VRUIUtilsService, VRNotificationService) {
        var gridAPI;

        defineScope();
        load();

        function defineScope() {
            console.log('defineScope')
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