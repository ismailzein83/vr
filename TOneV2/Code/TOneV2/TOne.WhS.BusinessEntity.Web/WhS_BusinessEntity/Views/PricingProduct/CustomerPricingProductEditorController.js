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
            $scope.onPricingProductsDirectiveLoaded = function (api) {
                pricingProductDirectiveAPI = api;
                if ($scope.disablePricingProduct)
                    pricingProductDirectiveAPI.setData(pricingProductId);
            }
            $scope.onCarrierAccountDirectiveLoaded = function (api) {
                carrierAccountDirectiveAPI = api;
                if ($scope.disableCarrierAccount)
                    carrierAccountDirectiveAPI.setData([carrierAccountId]);
            }
            $scope.onPricingProductsSelectionChanged = function () {
             
                  if (pricingProductDirectiveAPI!=undefined)
                      $scope.selectedPricingProduct = pricingProductDirectiveAPI.getData();
            }
            $scope.selectedCustomers;
            $scope.onCarrierAccountSelectionChanged = function () {
                if (carrierAccountDirectiveAPI != undefined)
                    $scope.selectedCustomers = carrierAccountDirectiveAPI.getData();
            }
            $scope.beginEffectiveDate = new Date();
            $scope.endEffectiveDate;

           
        }

        function load() {
            if ($scope.disablePricingProduct && pricingProductDirectiveAPI != undefined)
                pricingProductDirectiveAPI.setData(pricingProductId);
            if ($scope.disableCarrierAccount && carrierAccountDirectiveAPI != undefined)
                carrierAccountDirectiveAPI.setData([carrierAccountId]);
           
        }

        function insertPricingProduct() {
            var pricingProductObject = buildPricingProductObjFromScope();
            return WhS_BE_CustomerPricingProductAPIService.AddCustomerPricingProduct(pricingProductObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Customer Pricing Product", response)) {
                    if ($scope.onCustomerPricingProductAdded != undefined)
                        $scope.onCustomerPricingProductAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });

        }
        function buildPricingProductObjFromScope() {
            var obj = [];
            for (var i = 0; i < $scope.selectedCustomers.length; i++) {
                obj.push({
                    CustomerId: $scope.selectedCustomers[i].CarrierAccountId,
                    CustomerName: $scope.selectedCustomers.Name,
                    PricingProductId: $scope.selectedPricingProduct.PricingProductId,
                    PricingProductName: $scope.selectedPricingProduct.Name,
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
