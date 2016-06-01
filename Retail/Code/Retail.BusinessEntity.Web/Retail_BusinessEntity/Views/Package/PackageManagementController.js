(function (appControllers) {

    "use strict";

    packageManagementController.$inject = ['$scope', 'UtilsService', 'Retail_BE_PackageService', 'Retail_BE_PackageAPIService'];

    function packageManagementController($scope, UtilsService, Retail_BE_PackageService, Retail_BE_PackageAPIService) {
        var gridAPI;
        var packageDirectiveAPI;
        defineScope();
        load();

        function defineScope() {
            $scope.hadAddPackagePermission = function () {
                return Retail_BE_PackageService.HasAddPackagePermission();
            };

            $scope.onGridReady = function (api) {
                gridAPI = api;
                api.loadGrid({});
            }

            $scope.searchClicked = function () {
                return gridAPI.loadGrid(getFilterObject());
            };

            $scope.AddPackage = AddPackage;

            function getFilterObject() {
                var data = {
                    Name: $scope.name,
                };
                return data;
            }
        }

        function load() {
            loadAllControls();
        }

        function loadAllControls() {
        }

        function AddPackage() {
            var onPackageAdded = function (packageObj) {
                gridAPI.onPackageAdded(packageObj);
            };
            WhS_BE_PackageService.addPackage(onPackageAdded);
        }

    }

    appControllers.controller('Retail_BE_PackageManagementController', packageManagementController);
})(appControllers);