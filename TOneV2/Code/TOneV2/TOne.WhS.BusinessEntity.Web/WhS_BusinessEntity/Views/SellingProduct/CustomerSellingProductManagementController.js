(function (appControllers) {

    "use strict";

    customerSellingProductManagementController.$inject = ['$scope', 'WhS_BE_CustomerSellingProductAPIService', 'WhS_BE_MainService', 'UtilsService', 'VRModalService', 'VRNotificationService'];

    function customerSellingProductManagementController($scope, WhS_BE_CustomerSellingProductAPIService, WhS_BE_MainService, UtilsService, VRModalService, VRNotificationService) {
        var gridAPI;

        var sellingProductsDirectiveAPI;
        var sellingProductReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var carrierAccountDirectiveAPI;
        var carrierAccountReadyPromiseDeferred = UtilsService.createPromiseDeferred();

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
                carrierAccountReadyPromiseDeferred.resolve();
            }
            $scope.onSellingProductsDirectiveReady = function (api) {
                sellingProductsDirectiveAPI = api;
                sellingProductReadyPromiseDeferred.resolve();
            }
        }

        function load() {
            $scope.isLoading = true;
            loadAllControls();
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadSellingProducts, loadCarrierAccounts])
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.isLoading = false;
                })
               .finally(function () {
                   $scope.isLoading = false;
               });
        }
        function loadSellingProducts() {
            var sellingProductLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            sellingProductReadyPromiseDeferred.promise
                .then(function () {
                    var directivePayload = undefined

                    VRUIUtilsService.callDirectiveLoad(sellingProductsDirectiveAPI, directivePayload, sellingProductLoadPromiseDeferred);
                });
            return sellingProductLoadPromiseDeferred.promise;
        }
        function loadCarrierAccounts() {
            var carrierAccountLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            carrierAccountReadyPromiseDeferred.promise
                .then(function () {
                    var directivePayload = {
                        selectedIds: undefined
                    }

                    VRUIUtilsService.callDirectiveLoad(carrierAccountDirectiveAPI, directivePayload, carrierAccountLoadPromiseDeferred);
                });
            return carrierAccountLoadPromiseDeferred.promise;
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
                    if (customerSellingProductObj[i].Status == 0 && gridAPI != undefined)
                        gridAPI.onCustomerSellingProductAdded(customerSellingProductObj[i]);
                    else if (customerSellingProductObj[i].Status == 2 && gridAPI != undefined)
                    {
                        gridAPI.onCustomerSellingProductDeleted(customerSellingProductObj[i]);
                    }
                }
              
                
            };

            WhS_BE_MainService.addCustomerSellingProduct(onCustomerSellingProductAdded);
        }
    }

    appControllers.controller('WhS_BE_CustomerSellingProductManagementController', customerSellingProductManagementController);
})(appControllers);