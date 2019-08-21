(function (appControllers) {

    productManagementController.$inject = ['$scope', 'UtilsService', 'Demo_Module_ProductService', 'Demo_Module_ManufactoryService'];

    function productManagementController($scope, UtilsService, Demo_Module_ProductService, Demo_Module_ManufactoryService) {

        var manufactorySelectorAPI;
        var manufactorySelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var productGridAPI;
        var productGridReadyDeferred = UtilsService.createPromiseDeferred();

        defineAPI();
        load();

        function defineAPI() {
            $scope.scopeModel = {};
            $scope.scopeModel.products = [];

            $scope.scopeModel.onSearchClicked = function () {
                return productGridAPI.load(getFilter());
            };

            $scope.scopeModel.onAddClicked = function () {
                var onProductAdded = function (product) {
                    productGridAPI.onProductAdded(product);
                };
                Demo_Module_ProductService.addProduct(onProductAdded);
            };

            $scope.scopeModel.onManufactorySelectorReady = function (api) {
                manufactorySelectorAPI = api;
                manufactorySelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onProductGridReady = function (api) {
                productGridAPI = api;
                productGridReadyDeferred.resolve();
            };
        }

        function load() {
            loadManufactorySelector().then(function () {
                loadProductGrid();
            });

            function loadManufactorySelector() {
                var manufactorySelectorLoadDeferred = UtilsService.createPromiseDeferred();

                manufactorySelectorReadyDeferred.promise.then(function () {
                    manufactorySelectorAPI.load().then(function () {
                        manufactorySelectorLoadDeferred.resolve();
                    });
                });

                return manufactorySelectorLoadDeferred.promise;
            }

            function loadProductGrid() {
                var productGridLoadDeferred = UtilsService.createPromiseDeferred();

                productGridReadyDeferred.promise.then(function () {
                    productGridAPI.load(getFilter()).then(function () {
                        productGridLoadDeferred.resolve();
                    });
                });

                return productGridLoadDeferred.promise;
            }
        }

        function getFilter() {
            return {
                query: {
                    Name: $scope.scopeModel.name,
                    ManufactoryIds: manufactorySelectorAPI.getSelectedIds()
                }
            };
        }
    }

    appControllers.controller('Demo_Module_ProductManagementController', productManagementController);

})(appControllers);