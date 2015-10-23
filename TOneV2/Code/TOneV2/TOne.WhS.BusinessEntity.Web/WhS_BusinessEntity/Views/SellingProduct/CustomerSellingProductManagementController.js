(function (appControllers) {

    "use strict";

    customerSellingProductManagementController.$inject = ['$scope', 'WhS_BE_CustomerSellingProductAPIService', 'WhS_BE_MainService', 'UtilsService', 'VRModalService', 'VRNotificationService'];

    function customerSellingProductManagementController($scope, WhS_BE_CustomerSellingProductAPIService, WhS_BE_MainService, UtilsService, VRModalService, VRNotificationService) {
        var gridAPI;
        var carrierAccountDirectiveAPI;
        var sellingProductsDirectiveAPI;
        defineScope();

        function defineScope() {
            $scope.searchClicked = function () {
                if (!$scope.isGettingData && gridAPI != undefined)
               return  gridAPI.loadGrid(getFilterObject());
            };

            $scope.AddNewCustomerSellingProduct = AddNewCustomerSellingProduct;
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
            $scope.onSellingProductsDirectiveReady = function (api) {
                sellingProductsDirectiveAPI = api;
                load();
            }
        }

        function load() {
            $scope.isGettingData = true;
            if (carrierAccountDirectiveAPI == undefined || sellingProductsDirectiveAPI == undefined)
                return;

            UtilsService.waitMultipleAsyncOperations([loadSellingProducts,loadCarrierAccounts]).then(function () {
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.isGettingData = false;
            }).finally(function () {
                $scope.isGettingData = false;
            });
        }
        function loadSellingProducts() {
          return  sellingProductsDirectiveAPI.load();
        }
        function loadCarrierAccounts() {
           return carrierAccountDirectiveAPI.load();
        }

        function getFilterObject() {
            var data = {
                CustomersIds: UtilsService.getPropValuesFromArray(carrierAccountDirectiveAPI.getData(), "CarrierAccountId"),
                SellingProductsIds: UtilsService.getPropValuesFromArray(sellingProductsDirectiveAPI.getData(), "SellingProductId"),
                EffectiveDate: $scope.effectiveDate
            };
            return data;
        }

        function AddNewCustomerSellingProduct() {
            var onCustomerSellingProductAdded = function (customerSellingProductObj) {

                for (var i = 0; i < customerSellingProductObj.length; i++) {
                    if (customerSellingProductObj[i].Status == 0 && gridAPI != undefined)
                        gridAPI.onCustomerSellingProductAdded(customerSellingProductObj[i]);
                    else if (customerSellingProductObj[i].Status == 1 && gridAPI != undefined)
                    {
                        gridAPI.onCustomerSellingProductUpdated(customerSellingProductObj[i]);
                    }
                }
              
                
            };

            WhS_BE_MainService.addCustomerSellingProduct(onCustomerSellingProductAdded);
        }
    }

    appControllers.controller('WhS_BE_CustomerSellingProductManagementController', customerSellingProductManagementController);
})(appControllers);