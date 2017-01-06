(function (appControllers) {

    "use strict";

    ProductManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'Retail_BE_ProductAPIService', 'Retail_BE_ProductService'];

    function ProductManagementController($scope, UtilsService, VRUIUtilsService, Retail_BE_ProductAPIService, Retail_BE_ProductService) {

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
                var onProductAdded = function (addedProduct) {
                    gridAPI.onProductAdded(addedProduct);
                };

                Retail_BE_ProductService.addProduct(onProductAdded);
            };

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.load({});
            };

            //$scope.scopeModel.hasAddProductPermission = function () {
            //    return Retail_BE_ProductAPIService.HasAddProductPermission();
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

    appControllers.controller('Retail_BE_ProductManagementController', ProductManagementController);

})(appControllers);