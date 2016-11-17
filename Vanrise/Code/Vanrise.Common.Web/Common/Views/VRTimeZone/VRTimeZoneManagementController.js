(function (appControllers) {

    "use strict";

    vrTimeZoneManagementController.$inject = ['$scope', 'VRCommon_VRTimeZoneService', 'VRCommon_VRTimeZoneAPIService'];

    function vrTimeZoneManagementController($scope, VRCommon_VRTimeZoneService, VRCommon_VRTimeZoneAPIService) {
        var gridAPI;

        defineScope();
        load();

        var filter = {};

        function defineScope() {
            $scope.searchClicked = function () {
                setFilterObject();
                return gridAPI.loadGrid(filter);
            };

            $scope.onGridReady = function (api) {
                gridAPI = api;
                api.loadGrid(filter);
            };

            $scope.addNewVRTimeZone = addNewVRTimeZone;
            $scope.hasAddVRTimeZonePermission = function () {
                return VRCommon_VRTimeZoneAPIService.HasAddVRTimeZonePermission();
            };

            $scope.hasUploadVRTimeZonePermission = function () {
                return VRCommon_VRTimeZoneAPIService.HasUploadVRTimeZonePermission();
            };
        }

        function load() {
        }

        function setFilterObject() {
            filter = {
                Name: $scope.name,
            };
        }

        function addNewVRTimeZone() {
            var onVRTimeZoneAdded = function (vrTimeZoneObj) {
                if (gridAPI != undefined) {
                    gridAPI.onVRTimeZoneAdded(vrTimeZoneObj);
                }
            };

            VRCommon_VRTimeZoneService.addVRTimeZone(onVRTimeZoneAdded);
        }
    }

    appControllers.controller('VRCommon_VRTimeZoneManagementController', vrTimeZoneManagementController); 
})(appControllers);