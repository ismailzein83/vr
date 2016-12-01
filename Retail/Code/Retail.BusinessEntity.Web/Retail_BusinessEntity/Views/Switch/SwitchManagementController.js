(function (appControllers) {

    "use strict";

    SwitchManagementController.$inject = ['$scope', 'Retail_BE_SwitchService', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

    function SwitchManagementController($scope, Retail_BE_SwitchService, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

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

            $scope.scopeModel.add = function () {
                var onSwitchAdded = function (addedSwitch) {
                    gridAPI.onSwitchAdded(addedSwitch);
                };
                Retail_BE_SwitchService.addSwitch(onSwitchAdded);
            };

            $scope.onSwitchUpdated = function (updatedSwitch) {
                gridAPI.onSwitchUpdated(updatedSwitch);
            };

        }

        function load() {

        }

        function buildGridQuery() {
            return {
                Name: $scope.scopeModel.name,
                Description: $scope.scopeModel.description,
                Location: $scope.scopeModel.location
            };
        }
    }

    appControllers.controller('Retail_BE_SwitchManagementController', SwitchManagementController);

})(appControllers);