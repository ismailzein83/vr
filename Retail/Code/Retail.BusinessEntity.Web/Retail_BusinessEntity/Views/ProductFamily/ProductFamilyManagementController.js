(function (appControllers) {

    "use strict";

    ProductFamilyManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'Retail_BE_ProductFamilyAPIService', 'Retail_BE_ProductFamilyService'];

    function ProductFamilyManagementController($scope, UtilsService, VRUIUtilsService, Retail_BE_ProductFamilyAPIService, Retail_BE_ProductFamilyService) {

        var gridAPI;

        defineScope();
        load();

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.search = function () {
                var query = buildGridQuery();
                return gridAPI.load(query);
            };

            $scope.scopeModel.add = function () {
                var onProductFamilyAdded = function (addedProductFamily) {
                    gridAPI.onProductFamilyAdded(addedProductFamily);
                };

                Retail_BE_ProductFamilyService.addProductFamily(onProductFamilyAdded);
            };

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.load({});
            };

            //$scope.scopeModel.hasAddProductFamilyPermission = function () {
            //    return Retail_BE_ProductFamilyAPIService.HasAddProductFamilyPermission();
            //};
        }
        function load() {

        }

        function buildGridQuery() {
            return {
                Name: $scope.scopeModel.name
            };
        }
    }

    appControllers.controller('Retail_BE_ProductFamilyManagementController', ProductFamilyManagementController);

})(appControllers);