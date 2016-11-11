(function (appControllers) {

    "use strict";

    switchManagementController.$inject = ["$scope", "WhS_BE_SwitchService", "UtilsService", "VRUIUtilsService", "VRNotificationService", "WhS_BE_SwitchAPIService"];

    function switchManagementController($scope, WhS_BE_SwitchService, UtilsService, VRUIUtilsService, VRNotificationService, WhS_BE_SwitchAPIService) {
        var gridAPI;

        defineScope();
        load();

        function defineScope() {
            $scope.hasAddSwitchPermission = function () {
                return WhS_BE_SwitchAPIService.HasAddSwitchPermission();
            };

            $scope.onGridReady = function (api) {
                gridAPI = api;
                api.loadGrid({});
            };

            $scope.searchClicked = function () {
                return gridAPI.loadGrid(getFilterObject());
            };

            $scope.AddNewSwitch = AddNewSwitch;

            function getFilterObject() {
                var query = {
                    Name: $scope.name
                };
                return query;
            }
        }

        function load() {
        }


        function AddNewSwitch() {
            var onSwitchAdded = function (addedItem) {
                gridAPI.onSwitchAdded(addedItem);
            };

            WhS_BE_SwitchService.addSwitch(onSwitchAdded);
        }
    }

    appControllers.controller("WhS_BE_SwitchManagementController", switchManagementController);
})(appControllers);