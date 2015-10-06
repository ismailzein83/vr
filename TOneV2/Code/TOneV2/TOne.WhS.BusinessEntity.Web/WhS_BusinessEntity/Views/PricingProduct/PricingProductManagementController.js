(function (appControllers) {

    "use strict";

    pricingProductManagementController.$inject = ['$scope', 'WhS_BE_PricingProductAPIService', 'WhS_BE_MainService', 'UtilsService', 'VRModalService', 'VRNotificationService'];

    function pricingProductManagementController($scope, WhS_BE_PricingProductAPIService, WhS_BE_MainService, UtilsService, VRModalService, VRNotificationService) {
        var gridReady;
        defineScope();
    
        load();

        function defineScope() {
            $scope.searchClicked = function () {
                return load();
            };
            $scope.onGridReady = function (api) {
                gridReady = api;
                api.loadGrid(getFilterObject());
            }
            $scope.AddNewPricingProduct = AddNewPricingProduct;
        }

        function load() {
        }

        function getFilterObject() {

            var data = {
                name: $scope.name,
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