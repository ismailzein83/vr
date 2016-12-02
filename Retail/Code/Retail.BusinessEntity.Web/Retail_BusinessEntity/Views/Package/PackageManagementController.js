(function (appControllers) {

    "use strict";

    PackageManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'Retail_BE_PackageService', 'Retail_BE_PackageAPIService'];

    function PackageManagementController($scope, UtilsService, VRUIUtilsService, Retail_BE_PackageService, Retail_BE_PackageAPIService) {

        var gridAPI;

        defineScope();
        load();

        function defineScope() {

            $scope.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.load({});
            };

            $scope.search = function () {
                var query = buildGridQuery();
                return gridAPI.load(query);
            };
            $scope.add = function () {
                var onPackageAdded = function (addedPackage) {
                    gridAPI.onPackageAdded(addedPackage);
                };

                Retail_BE_PackageService.addPackage(onPackageAdded);
            };

            $scope.hasAddPackagePermission = function () {
                return Retail_BE_PackageAPIService.HasAddPackagePermission();
            };
        }
        function load() {

        }

        function buildGridQuery() {
            return {
                Name: $scope.name
            };
        }
    }

    appControllers.controller('Retail_BE_PackageManagementController', PackageManagementController);

})(appControllers);