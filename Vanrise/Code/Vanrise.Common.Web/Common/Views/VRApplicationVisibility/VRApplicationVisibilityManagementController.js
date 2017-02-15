(function (appControllers) {

    "use strict";

    VRApplicationVisibilityManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRCommon_VRApplicationVisibilityService', 'VRCommon_VRApplicationVisibilityAPIService'];

    function VRApplicationVisibilityManagementController($scope, UtilsService, VRUIUtilsService, VRCommon_VRApplicationVisibilityService, VRCommon_VRApplicationVisibilityAPIService) {

        var gridAPI;

        defineScope();
        load();


        function defineScope() {

            $scope.search = function () {
                var query = buildGridQuery();
                return gridAPI.load(query);
            };
            $scope.add = function () {
                var onVRApplicationVisibilityAdded = function (addedVRApplicationVisibility) {
                    gridAPI.onVRApplicationVisibilityAdded(addedVRApplicationVisibility);
                };

                VRCommon_VRApplicationVisibilityService.addVRApplicationVisibility(onVRApplicationVisibilityAdded);
            };

            $scope.hasAddVRApplicationVisibilityPermission = function () {
                return VRCommon_VRApplicationVisibilityAPIService.HasAddVRApplicationVisibilityPermission()
            };

            $scope.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.load({});
            };
        }

        function load() {

        }

        function buildGridQuery() {
            return {
                Name: $scope.name,
            };
        }
    }

    appControllers.controller('VRCommon_VRApplicationVisibilityManagementController', VRApplicationVisibilityManagementController);

})(appControllers);