(function (appControllers) {

    "use strict";

    pricingProductManagementController.$inject = ['$scope', 'WhS_BE_MainService', 'UtilsService', 'VRNotificationService'];

    function pricingProductManagementController($scope, WhS_BE_MainService, UtilsService, VRNotificationService) {
        var gridReady;
        var saleZonePackageDirectiveAPI;
        var routingProductDirectiveAPI;
        defineScope();
        load();

        function defineScope() {
            $scope.searchClicked = function () {
                if (gridReady != undefined)
                 return gridReady.loadGrid(getFilterObject());
            };
            $scope.onGridReady = function (api) {
                gridReady = api;
                var filter = {}
                api.loadGrid(filter);
            }
            $scope.onSaleZonePackageDirectiveReady = function (api) {
                saleZonePackageDirectiveAPI = api;
                load();
            }
            $scope.onRoutingProductDirectiveReady = function (api) {
                routingProductDirectiveAPI = api;
                console.log(api);
                load();
            }
            $scope.AddNewPricingProduct = AddNewPricingProduct;
        }

        function load() {
            $scope.isGettingData = true;
            if (saleZonePackageDirectiveAPI == undefined || routingProductDirectiveAPI == undefined)
                return;
            UtilsService.waitMultipleAsyncOperations([loadSaleZonePackages, loadRoutingProducts]).then(function () {
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.isGettingData = false;
            }).finally(function () {
                $scope.isGettingData = false;
            });
        }

        function loadSaleZonePackages() {
           return saleZonePackageDirectiveAPI.load();
        }
        function loadRoutingProducts() {
            return routingProductDirectiveAPI.load();
        }

        function getFilterObject() {

            var data = {
                name: $scope.name,
                SaleZonePackagesIds: UtilsService.getPropValuesFromArray(saleZonePackageDirectiveAPI.getData(), "SaleZonePackageId"),
                RoutingProductsIds: UtilsService.getPropValuesFromArray(routingProductDirectiveAPI.getData(), "RoutingProductId"),
            };
            return data;
        }

        function AddNewPricingProduct() {
            var onPricingProductAdded = function (pricingProductObj) {
                if (gridReady != undefined)
                    gridReady.onPricingProductAdded(pricingProductObj);
            };
            WhS_BE_MainService.addPricingProduct(onPricingProductAdded);
        }
    }

    appControllers.controller('WhS_BE_PricingProductManagementController', pricingProductManagementController);
})(appControllers);