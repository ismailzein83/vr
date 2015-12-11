(function (appControllers) {

    "use strict";

    profileManagementController.$inject = ['$scope', 'Qm_CliTester_ProfileService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

    function profileManagementController($scope, Qm_CliTester_ProfileService, UtilsService, VRUIUtilsService, VRNotificationService) {
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

    appControllers.controller('Qm_CliTester_ProfileManagementController', profileManagementController);
})(appControllers);