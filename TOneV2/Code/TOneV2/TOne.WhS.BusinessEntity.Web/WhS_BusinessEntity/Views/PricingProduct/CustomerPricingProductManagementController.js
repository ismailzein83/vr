(function (appControllers) {

    "use strict";

    customerPricingProductManagementController.$inject = ['$scope', 'WhS_BE_CustomerPricingProductAPIService', 'WhS_BE_MainService', 'UtilsService', 'VRModalService', 'VRNotificationService'];

    function customerPricingProductManagementController($scope, WhS_BE_CustomerPricingProductAPIService, WhS_BE_MainService, UtilsService, VRModalService, VRNotificationService) {
        var gridAPI;
        var carrierAccountDirectiveAPI;
        var pricingProductsDirectiveAPI;
        defineScope();

        function defineScope() {
            $scope.searchClicked = function () {
                if (!$scope.isGettingData && gridAPI != undefined)
               return  gridAPI.loadGrid(getFilterObject());
            };

            $scope.AddNewCustomerPricingProduct = AddNewCustomerPricingProduct;
            $scope.onGridReady = function (api) {
                gridAPI = api;
                var filter = {
                }
                api.loadGrid(filter);
            }
            $scope.effectiveDate;
            $scope.onCarrierAccountDirectiveReady = function (api) {
                carrierAccountDirectiveAPI = api;
                load();
            }
            $scope.onPricingProductsDirectiveReady = function (api) {
                pricingProductsDirectiveAPI = api;
                load();
            }
        }

        function load() {
            $scope.isGettingData = true;
            if (carrierAccountDirectiveAPI == undefined || pricingProductsDirectiveAPI == undefined)
                return;

            UtilsService.waitMultipleAsyncOperations([loadPricingProducts,loadCarrierAccounts]).then(function () {
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.isGettingData = false;
            }).finally(function () {
                $scope.isGettingData = false;
            });
        }
        function loadPricingProducts() {
          return  pricingProductsDirectiveAPI.load();
        }
        function loadCarrierAccounts() {
           return carrierAccountDirectiveAPI.load();
        }

        function getFilterObject() {
            var data = {
                CustomersIds: UtilsService.getPropValuesFromArray(carrierAccountDirectiveAPI.getData(), "CarrierAccountId"),
                PricingProductsIds: UtilsService.getPropValuesFromArray(pricingProductsDirectiveAPI.getData(), "PricingProductId"),
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
                        gridAPI.onCustomerPricingProductUpdated(customerPricingProductObj[i]);
                    }
                }
              
                
            };

            WhS_BE_MainService.addCustomerPricingProduct(onCustomerPricingProductAdded);
        }
    }

    appControllers.controller('WhS_BE_CustomerPricingProductManagementController', customerPricingProductManagementController);
})(appControllers);