(function (appControllers) {

    "use strict";

    buildingManagementController.$inject = ['$scope', 'VRNotificationService', 'Demo_Module_BuildingAPIService', 'UtilsService', 'VRUIUtilsService', 'Demo_Module_BuildingService', 'VRNavigationService'];

    function buildingManagementController($scope, VRNotificationService, Demo_Module_BuildingAPIService, UtilsService, VRUIUtilsService, Demo_Module_BuildingService, VRNavigationService) {

        var gridAPI;
        var filter = {};
         
        defineScope();
        load();
        function defineScope() {
            $scope.buildings = [];
            $scope.searchClicked = function () {
                setfilterdobject()
                return gridAPI.loadGrid(filter);
            };

          

            function setfilterdobject() {
                filter = {
                    Name: $scope.name,
                };
            }
            $scope.onGridReady = function (api) {
                gridAPI = api;
                api.loadGrid(filter);
            };
            $scope.addNewBuilding = addNewBuilding;
        }

        function load() {
        }

        function addNewBuilding() {
            var onBuildingAdded = function (building) {
                if (gridAPI != undefined)
                    gridAPI.onBuildingAdded(building);
            };

            Demo_Module_BuildingService.addBuilding(onBuildingAdded);
        }
    }




    appControllers.controller('Demo_Module_BuildingManagementController', buildingManagementController);
})(appControllers);