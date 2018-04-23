(function (appControllers) {

    "use strict";

    RuntimeNodeConfigurationManagementController.$inject = ['$scope', 'VRRuntime_RuntimeNodeConfigurationService', 'UtilsService', 'VRUIUtilsService'];

    function RuntimeNodeConfigurationManagementController($scope, VRRuntime_RuntimeNodeConfigurationService, UtilsService, VRUIUtilsService) {
        var gridAPI;
        var filter = {};

        defineScope();
        load();

        function defineScope() {
            $scope.searchClicked = function () {
                getFilterObject();
                return gridAPI.loadGrid(filter);
            };

            function getFilterObject() {
                filter = {
                    Name: $scope.name
                };

            }

            $scope.onGridReady = function (api) {
                gridAPI = api;
                api.loadGrid(filter);
            };
            $scope.addNewRuntimeNodeConfiguration = addNewRuntimeNodeConfiguration;
        }

        function load() {
            $scope.isLoadingFilters = true;
            loadAllControls();
        }

        function loadAllControls() {
        }

        function addNewRuntimeNodeConfiguration() {
            var onRuntimeNodeConfigurationAdded = function (Obj) {
                gridAPI.onRuntimeNodeConfigurationAdded(Obj);
            };

            VRRuntime_RuntimeNodeConfigurationService.addRuntimeNodeConfiguration(onRuntimeNodeConfigurationAdded);
        }

    }

    appControllers.controller('VRRuntime_RuntimeNodeConfigurationManagementController', RuntimeNodeConfigurationManagementController);
})(appControllers);