(function (appControllers) {
    "use strict";

    buildingManagementController.$inject = ['$scope', 'VRNotificationService', 'Demo_Module_BuildingService', 'UtilsService', 'VRUIUtilsService', 'Demo_Module_CompanyService'];

    function buildingManagementController($scope, VRNotificationService, Demo_Module_BuildingService, UtilsService, VRUIUtilsService, Demo_Module_CompanyService) {

        var gridApi;
        defineScope();
        load();

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onGridReady = function (api) {
                gridApi = api;
                api.load(getFilter());
            };

            $scope.scopeModel.search = function () {
                return gridApi.load(getFilter());
            };


            $scope.scopeModel.addBuilding = function () {
                var onBuildingAdded = function (building) {
                    if (gridApi != undefined) {
                        gridApi.onBuildingAdded(building);
                    }
                };
                Demo_Module_BuildingService.addBuilding(onBuildingAdded);
            };
        };

        function load() {

        }


        function getFilter() {
            return {
                Name: $scope.scopeModel.name
            };
        };

    };

    appControllers.controller('Demo_Module_BuildingManagementController', buildingManagementController);
})(appControllers);