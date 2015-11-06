(function (appControllers) {

    "use strict";

    zoneServiceConfigManagementController.$inject = ['$scope', 'WhS_BE_MainService'];

    function zoneServiceConfigManagementController($scope, WhS_BE_MainService) {
        var gridAPI;
        defineScope();
        load();
        var filter = {};

        function defineScope() {
            $scope.searchClicked = function () {
                setFilterObject()
                return gridAPI.loadGrid(filter);
            };

            $scope.onGridReady = function (api) {
               
                gridAPI = api;
                api.loadGrid(filter);
            }
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
            WhS_BE_MainService.addZoneServiceConfig(onZoneServiceConfigAdded);
        }

    }

    appControllers.controller('WhS_BE_ZoneServiceConfigManagementController', zoneServiceConfigManagementController);
})(appControllers);