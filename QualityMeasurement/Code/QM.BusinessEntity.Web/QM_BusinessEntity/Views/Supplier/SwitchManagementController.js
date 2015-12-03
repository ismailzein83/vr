(function (appControllers) {

    "use strict";

    switchManagementController.$inject = ['$scope', 'WhS_BE_SwitchService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

    function switchManagementController($scope, WhS_BE_SwitchService, UtilsService, VRUIUtilsService, VRNotificationService) {
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

    appControllers.controller('WhS_BE_SwitchManagementController', switchManagementController);
})(appControllers);