(function (appControllers) {

    "use strict";

    customerPricingProductManagementController.$inject = ['$scope', 'WhS_BE_CustomerPricingProductAPIService', 'WhS_BE_MainService', 'UtilsService', 'VRModalService', 'VRNotificationService'];

    function customerPricingProductManagementController($scope, WhS_BE_CustomerPricingProductAPIService, WhS_BE_MainService, UtilsService, VRModalService, VRNotificationService) {
        var gridAPI;
        var carrierAccountDirectiveAPI;
        var pricingProductsDirectiveAPI;
        defineScope();
        load();

        function defineScope() {
            $scope.searchClicked = function () {
                if (carrierAccountDirectiveAPI != undefined && pricingProductsDirectiveAPI != undefined && gridAPI != undefined)
                    gridAPI.loadGrid(getFilterObject());
            };

            $scope.AddNewCustomerPricingProduct = AddNewCustomerPricingProduct;
            $scope.onGridReady = function (api) {
                gridAPI = api;
                var filter = {
                }
                api.loadGrid(filter);
            }
            $scope.selectedSupplier;
            $scope.effectiveDate;
            $scope.selectedPricingProduct;
            $scope.onCarrierAccountDirectiveLoaded = function (api) {
                carrierAccountDirectiveAPI = api;
            }
            $scope.onPricingProductsDirectiveLoaded = function (api) {
                pricingProductsDirectiveAPI = api;
            }
            $scope.onCarrierAccountSelectionChanged = function () {
                if (carrierAccountDirectiveAPI != undefined)
                $scope.selectedSupplier = carrierAccountDirectiveAPI.getData();
            }
            $scope.onPricingProductsSelectionChanged = function () {
                if (pricingProductsDirectiveAPI!=undefined)
                $scope.selectedPricingProduct = pricingProductsDirectiveAPI.getData();
            }
        }

        function load() {
        }

        function getFilterObject() {
            var selectedSupplier;
            var selectedPricingProduct;
            if ($scope.selectedSupplier != undefined)
                selectedSupplier = $scope.selectedSupplier.CarrierAccountId;
            if ($scope.selectedPricingProduct != undefined)
                selectedPricingProduct=$scope.selectedPricingProduct.PricingProductId;
            var data = {
                CustomerId: selectedSupplier,
                PricingProductId: selectedPricingProduct,
                EffectiveDate: $scope.effectiveDate
            };
            return data;
        }

        function AddNewCustomerPricingProduct() {
            var onPricingProductAdded = function (pricingProductObj) {
                if (gridAPI != undefined)
                    gridAPI.onCustomerPricingProductAdded(pricingProductObj);
            };

            WhS_BE_MainService.addCustomerPricingProduct(onPricingProductAdded);
        }
    }

    appControllers.controller('WhS_BE_CustomerPricingProductManagementController', customerPricingProductManagementController);
})(appControllers);