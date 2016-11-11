(function (appControllers) {

    "use strict";

    zoneServiceConfigManagementController.$inject = ["$scope", "WhS_BE_ZoneServiceConfigService", "WhS_BE_ZoneServiceConfigAPIService"];

    function zoneServiceConfigManagementController($scope, WhS_BE_ZoneServiceConfigService, WhS_BE_ZoneServiceConfigAPIService) {
        var gridAPI;
        defineScope();
        load();
        var filter = {};

        function defineScope() {

            $scope.hasAddZoneServiceConfigPermission = function () {
                return WhS_BE_ZoneServiceConfigAPIService.HasAddZoneServiceConfigPermission();
            };
            $scope.searchClicked = function () {
                setFilterObject();
                return gridAPI.loadGrid(filter);
            };

            $scope.onGridReady = function (api) {

                gridAPI = api;
                api.loadGrid(filter);
            };
            $scope.addNewZoneServiceConfig = addNewZoneServiceConfig;
        }

        function load() {
           


        }

        function setFilterObject() {
            filter = {
                Name: $scope.name
            };

        }

        function addNewZoneServiceConfig() {
            var onZoneServiceConfigAdded = function (zoneServiceConfigObj) {
                if (gridAPI != undefined) {
                    gridAPI.onZoneServiceConfigAdded(zoneServiceConfigObj);
                }


            };
            WhS_BE_ZoneServiceConfigService.addZoneServiceConfig(onZoneServiceConfigAdded);
        }

    }

    appControllers.controller("WhS_BE_ZoneServiceConfigManagementController", zoneServiceConfigManagementController);
})(appControllers);