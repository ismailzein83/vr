(function (appControllers) {

    "use strict";

    VRNamespaceManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRCommon_VRNamespaceService', 'VRCommon_VRNamespaceAPIService'];

    function VRNamespaceManagementController($scope, UtilsService, VRUIUtilsService, VRCommon_VRNamespaceService, VRCommon_VRNamespaceAPIService) {

        var gridAPI;

        defineScope();
        load();


        function defineScope() {

            $scope.search = function () {
                var query = buildGridQuery();
                return gridAPI.load(query);
            };
            $scope.add = function () {
                var onVRNamespaceAdded = function (addedVRNamespace) {
                    gridAPI.onVRNamespaceAdded(addedVRNamespace);
                };
                VRCommon_VRNamespaceService.addVRNamespace(onVRNamespaceAdded);
            };

            $scope.hasAddVRNamespacePermission = function () {
                return VRCommon_VRNamespaceAPIService.HasAddVRNamespacePermission();
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

    appControllers.controller('VRCommon_VRNamespaceManagementController', VRNamespaceManagementController);

})(appControllers);