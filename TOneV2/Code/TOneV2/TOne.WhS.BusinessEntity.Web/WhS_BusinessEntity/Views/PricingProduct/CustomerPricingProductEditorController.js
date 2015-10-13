(function (appControllers) {

    "use strict";

    customerPricingProductEditorController.$inject = ['$scope', 'WhS_BE_CustomerPricingProductAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService'];

    function customerPricingProductEditorController($scope, WhS_BE_CustomerPricingProductAPIService, UtilsService, VRNotificationService, VRNavigationService) {

        var pricingProductDirectiveAPI;
        var carrierAccountDirectiveAPI;
        var pricingProductId;
        var carrierAccountId;
       
        defineScope();
        loadParameters();
        load();
        function loadParameters(){
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                pricingProductId = parameters.PricingProductId;
                carrierAccountId = parameters.CarrierAccountId;
            }
            $scope.disablePricingProduct = (pricingProductId != undefined);
            $scope.disableCarrierAccount = (carrierAccountId != undefined);

        }
        function defineScope() {
            $scope.allDestinations = false;
            $scope.SavePricingProduct = function () {
                    return insertPricingProduct();
            };

            $scope.close = function () {
                $scope.modalContext.closeModal()
            };
            $scope.onPricingProductsDirectiveReady = function (api) {
                pricingProductDirectiveAPI = api;
                load();
                if ($scope.disablePricingProduct)
                    pricingProductDirectiveAPI.setData(pricingProductId);
            }
            $scope.onCarrierAccountDirectiveReady = function (api) {
                carrierAccountDirectiveAPI = api;
                load();
                if ($scope.disableCarrierAccount)
                    carrierAccountDirectiveAPI.setData([carrierAccountId]);
            }

            $scope.beginEffectiveDate = new Date();
            $scope.endEffectiveDate;

           
        }

        function load() {
            $scope.isGettingData = true;
            if (carrierAccountDirectiveAPI == undefined || pricingProductDirectiveAPI == undefined)
                return;

            UtilsService.waitMultipleAsyncOperations([loadPricingProducts, loadCarrierAccounts]).then(function () {
                if ($scope.disablePricingProduct && pricingProductDirectiveAPI != undefined)
                    pricingProductDirectiveAPI.setData(pricingProductId);
                if ($scope.disableCarrierAccount && carrierAccountDirectiveAPI != undefined)
                    carrierAccountDirectiveAPI.setData([carrierAccountId]);
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.isGettingData = false;
            }).finally(function () {
                $scope.isGettingData = false;
            });
           
           
        }
        function loadPricingProducts() {
            return pricingProductDirectiveAPI.load();
        }
        function loadCarrierAccounts() {
            return carrierAccountDirectiveAPI.load();
        }

        function insertPricingProduct() {
            var pricingProductObject = buildPricingProductObjFromScope();
            return WhS_BE_CustomerPricingProductAPIService.AddCustomerPricingProduct(pricingProductObject)
            .then(function (response) {
               
                if (VRNotificationService.notifyOnItemAdded("Customer Pricing Product", response)) {
                    {
                        if ($scope.onCustomerPricingProductAdded != undefined)
                            $scope.onCustomerPricingProductAdded(response.InsertedObject);
                        $scope.modalContext.closeModal();
                    }
                }
               
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });

        }
        function buildPricingProductObjFromScope() {
            var obj = [];
            var selectedCustomers = carrierAccountDirectiveAPI.getData();
            var selectedPricingProduct = pricingProductDirectiveAPI.getData();
            for (var i = 0; i < selectedCustomers.length; i++) {
                obj.push({
                    CustomerId: selectedCustomers[i].CarrierAccountId,
                    CustomerName: selectedCustomers[i].Name,
                    PricingProductId: selectedPricingProduct.PricingProductId,
                    PricingProductName: selectedPricingProduct.Name,
                    BED: $scope.beginEffectiveDate,
                    EED: $scope.endEffectiveDate,
                    AllDestinations: $scope.allDestinations
                });
            }
            return obj;
        }
        }
        
  
    appControllers.controller('WhS_BE_CustomerPricingProductEditorController', customerPricingProductEditorController);
})(appControllers);
