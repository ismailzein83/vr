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
            $scope.selectedCustomers;
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
                    $scope.selectedCustomers = carrierAccountDirectiveAPI.getData();
            }
            $scope.onPricingProductsSelectionChanged = function () {
                if (pricingProductsDirectiveAPI!=undefined)
                $scope.selectedPricingProduct = pricingProductsDirectiveAPI.getData();
            }
        }

        function load() {
        }

        function getFilterObject() {
            var selectedCustomers;
            var selectedPricingProduct;
            if ($scope.selectedCustomers != undefined)
                selectedCustomers = $scope.selectedCustomers.CarrierAccountId;
            if ($scope.selectedPricingProduct != undefined)
                selectedPricingProduct=$scope.selectedPricingProduct.PricingProductId;
            var data = {
                CustomerId: selectedCustomers,
                PricingProductId: selectedPricingProduct,
                EffectiveDate: $scope.effectiveDate
            };
            return data;
        }

        function AddNewCustomerPricingProduct() {
            var onCustomerPricingProductAdded = function (customerPricingProductObj) {

                for (var i = 0; i < customerPricingProductObj.length; i++) {
                    if (customerPricingProductObj[i].Status == 0 && gridAPI != undefined)
                        gridAPI.onCustomerPricingProductAdded(customerPricingProductObj[i]);
                    else if (customerPricingProductObj[i].Status == 1 && gridAPI != undefined)
                    {
                        gridAPI.onCustomerPricingProductUpdated(cloneObject(customerPricingProductObj[i], true));
                    }
                      
                    console.log(customerPricingProductObj[i]);
                }
              
                
            };

            WhS_BE_MainService.addCustomerPricingProduct(onCustomerPricingProductAdded);
        }
    }

    appControllers.controller('WhS_BE_CustomerPricingProductManagementController', customerPricingProductManagementController);
})(appControllers);