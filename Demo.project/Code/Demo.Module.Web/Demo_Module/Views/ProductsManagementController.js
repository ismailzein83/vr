(function (appControllers) {
    "use strict";

    parentManagementController.$inject = ['$scope', 'VRNotificationService', 'Demo_Module_ProductService', 'UtilsService', 'VRUIUtilsService', 'Demo_Module_CompanyService'];

    function parentManagementController($scope, VRNotificationService, Demo_Module_ProductService, UtilsService, VRUIUtilsService, Demo_Module_CompanyService) {

        var gridApi;
        defineScope();
        load();

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onGridReady = function (api) {
                gridApi = api;
                api.load(getFilter());
            };

            $scope.scopeModel.search = function () {
                return gridApi.load(getFilter());
            };


            $scope.scopeModel.addProduct = function () {
                var onProductAdded = function (product) {
                    if (gridApi != undefined) {
                        gridApi.onProductAdded(product);
                    }
                };
                Demo_Module_ProductService.addProduct(onProductAdded);
            };
        };

        function load() {

        }


        function getFilter() {
            return {
                Name: $scope.scopeModel.name
            };
        };

    };

    appControllers.controller('Demo_Module_ProductManagementController', parentManagementController);
})(appControllers);