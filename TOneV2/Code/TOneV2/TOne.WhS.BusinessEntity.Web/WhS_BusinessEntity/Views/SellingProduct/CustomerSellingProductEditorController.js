(function (appControllers) {

    "use strict";

    customerSellingProductEditorController.$inject = ['$scope', 'WhS_BE_CustomerSellingProductAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService','VRUIUtilsService'];

    function customerSellingProductEditorController($scope, WhS_BE_CustomerSellingProductAPIService, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

        var sellingProductDirectiveAPI;
        var sellingProductReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var carrierAccountDirectiveAPI;
        var carrierAccountReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var sellingProductId;
        var carrierAccountId;
       
        defineScope();
        loadParameters();
        load();
        function loadParameters(){
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                sellingProductId = parameters.SellingProductId;
                carrierAccountId = parameters.CarrierAccountId;
            }
            $scope.disableSellingProduct = (sellingProductId != undefined);
            $scope.disableCarrierAccount = (carrierAccountId != undefined);

        }
        function defineScope() {
            $scope.selectedSellingProduct;
            $scope.SaveSellingProduct = function () {
                    return insertSellingProduct();
            };

            $scope.close = function () {
                $scope.modalContext.closeModal()
            };
            $scope.onSellingProductsDirectiveReady = function (api) {
                sellingProductDirectiveAPI = api;
                sellingProductReadyPromiseDeferred.resolve();
            }
            $scope.onCarrierAccountDirectiveReady = function (api) {
                carrierAccountDirectiveAPI = api;
                carrierAccountReadyPromiseDeferred.resolve();
            }

            $scope.beginEffectiveDate = new Date();
           
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
                    var directivePayload = sellingProductId

                    VRUIUtilsService.callDirectiveLoad(sellingProductDirectiveAPI, directivePayload, sellingProductLoadPromiseDeferred);
                });
            return sellingProductLoadPromiseDeferred.promise;
        }

        function loadCarrierAccounts() {
            var carrierAccountLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            carrierAccountReadyPromiseDeferred.promise
                .then(function () {
                    var directivePayload = {
                        selectedIds: carrierAccountId != undefined ? [carrierAccountId] : undefined
                    }

                    VRUIUtilsService.callDirectiveLoad(carrierAccountDirectiveAPI, directivePayload, carrierAccountLoadPromiseDeferred);
                });
            return carrierAccountLoadPromiseDeferred.promise;
        }

        function insertSellingProduct() {
            var sellingProductObject = buildSellingProductObjFromScope();
            return WhS_BE_CustomerSellingProductAPIService.AddCustomerSellingProduct(sellingProductObject)
            .then(function (response) {
               
                if (VRNotificationService.notifyOnItemAdded("Customer Selling Product", response)) {
                    {
                        if ($scope.onCustomerSellingProductAdded != undefined)
                            $scope.onCustomerSellingProductAdded(response.InsertedObject);
                        $scope.modalContext.closeModal();
                    }
                }
               
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });

        }
        function buildSellingProductObjFromScope() {
            var obj = [];
            var selectedCustomers = carrierAccountDirectiveAPI.getSelectedValues();
            var selectedSellingProduct = $scope.selectedSellingProduct;
            for (var i = 0; i < selectedCustomers.length; i++) {
                obj.push({
                    CustomerId: selectedCustomers[i].CarrierAccountId,
                    CustomerName: selectedCustomers[i].Name,
                    SellingProductId: selectedSellingProduct.SellingProductId,
                    SellingProductName: selectedSellingProduct.Name,
                    BED: $scope.beginEffectiveDate,
                });
            }
            return obj;
        }
        }
        
  
    appControllers.controller('WhS_BE_CustomerSellingProductEditorController', customerSellingProductEditorController);
})(appControllers);
