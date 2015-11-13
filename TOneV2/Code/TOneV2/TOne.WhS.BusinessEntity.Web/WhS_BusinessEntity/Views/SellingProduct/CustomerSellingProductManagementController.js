(function (appControllers) {

    "use strict";

    customerSellingProductManagementController.$inject = ['$scope', 'WhS_BE_CustomerSellingProductAPIService', 'WhS_BE_MainService', 'UtilsService', 'VRModalService', 'VRNotificationService','VRUIUtilsService'];

    function customerSellingProductManagementController($scope, WhS_BE_CustomerSellingProductAPIService, WhS_BE_MainService, UtilsService, VRModalService, VRNotificationService, VRUIUtilsService) {
        var gridAPI;

        var sellingProductsDirectiveAPI;
        var sellingProductReadyPromiseDeferred;

        var carrierAccountDirectiveAPI;
        var carrierAccountReadyPromiseDeferred;

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
                var setLoader = function (value) { $scope.isLoadingCustomers = value };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, carrierAccountDirectiveAPI, undefined, setLoader, carrierAccountReadyPromiseDeferred);
            }
            $scope.onSellingProductsDirectiveReady = function (api) {
                sellingProductsDirectiveAPI = api;
                var setLoader = function (value) { $scope.isLoadingSellingProduct = value };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, sellingProductsDirectiveAPI, undefined, setLoader, sellingProductReadyPromiseDeferred);
            }
        }

        function load() {
        }
       

        function getFilterObject() {
            var data = {
                CustomersIds: carrierAccountDirectiveAPI.getSelectedIds(),
                SellingProductsIds: sellingProductsDirectiveAPI.getSelectedIds(), 
                EffectiveDate: $scope.effectiveDate
            };
            return data;
        }

        function AddNewCustomerSellingProduct() {
            var onCustomerSellingProductAdded = function (customerSellingProductObj) {

                for (var i = 0; i < customerSellingProductObj.length; i++) {
                        gridAPI.onCustomerSellingProductAdded(customerSellingProductObj[i]);

                }
              
                
            };

            WhS_BE_MainService.addCustomerSellingProduct(onCustomerSellingProductAdded);
        }
    }

    appControllers.controller('WhS_BE_CustomerSellingProductManagementController', customerSellingProductManagementController);
})(appControllers);