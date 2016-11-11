(function (appControllers) {

    "use strict";

    customerSellingProductManagementController.$inject = ['$scope', 'WhS_BE_CustomerSellingProductAPIService', 'UtilsService', 'VRModalService', 'VRNotificationService', 'VRUIUtilsService', 'WhS_BE_CustomerSellingProductService'];

    function customerSellingProductManagementController($scope, WhS_BE_CustomerSellingProductAPIService, UtilsService, VRModalService, VRNotificationService, VRUIUtilsService, WhS_BE_CustomerSellingProductService) {
        var gridAPI;

        var sellingProductsDirectiveAPI;
        var sellingProductReadyPromiseDeferred;

        var carrierAccountDirectiveAPI;
        var carrierAccountReadyPromiseDeferred;

        defineScope();

        function defineScope() {
            $scope.hasAddCustomerSellingProductPermission = function () {
                return WhS_BE_CustomerSellingProductAPIService.HasAddCustomerSellingProductPermission();
            };

            $scope.searchClicked = function () {
                if (!$scope.isGettingData && gridAPI != undefined)
                    return gridAPI.loadGrid(getFilterObject());
            };

            $scope.AddNewCustomerSellingProduct = AddNewCustomerSellingProduct;
            $scope.onGridReady = function (api) {
                gridAPI = api;
                var filter = {
                }
                api.loadGrid(filter);
            };
            $scope.effectiveDate;
            $scope.onCarrierAccountDirectiveReady = function (api) {
                carrierAccountDirectiveAPI = api;
                var setLoader = function (value) { $scope.isLoadingCustomers = value };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, carrierAccountDirectiveAPI, undefined, setLoader, carrierAccountReadyPromiseDeferred);
            };
            $scope.onSellingProductsDirectiveReady = function (api) {
                sellingProductsDirectiveAPI = api;
                var setLoader = function (value) { $scope.isLoadingSellingProduct = value };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, sellingProductsDirectiveAPI, undefined, setLoader, sellingProductReadyPromiseDeferred);
            };
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

            WhS_BE_CustomerSellingProductService.addCustomerSellingProduct(onCustomerSellingProductAdded);
        }
    }

    appControllers.controller('WhS_BE_CustomerSellingProductManagementController', customerSellingProductManagementController);
})(appControllers);