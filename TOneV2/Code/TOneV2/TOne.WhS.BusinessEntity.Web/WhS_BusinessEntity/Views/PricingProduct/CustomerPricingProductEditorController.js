(function (appControllers) {

    "use strict";

    customerPricingProductEditorController.$inject = ['$scope', 'WhS_BE_CustomerPricingProductAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService'];

    function customerPricingProductEditorController($scope, WhS_BE_CustomerPricingProductAPIService, UtilsService, VRNotificationService, VRNavigationService) {

        var pricingProductDirectiveAPI;
        var carrierAccountDirectiveAPI;
        var pricingProductId;
       
        defineScope();
        loadParameters();
        load();
        function loadParameters(){
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                pricingProductId = parameters.PricingProductId;
            }
            $scope.disablePricingProduct = (pricingProductId != undefined);
        }
        function defineScope() {
            $scope.allDistinations = false;
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
            var obj = {
                CustomerId: $scope.selectedCustomers.CarrierAccountId,
                CustomerName: $scope.selectedCustomers.Name,
                PricingProductId: $scope.selectedPricingProduct.PricingProductId,
                PricingProductName:$scope.selectedPricingProduct.Name,
                BED: $scope.beginEffectiveDate,
                EED: $scope.endEffectiveDate,
                AllDestinations:$scope.allDistinations

            };
            return obj;
        }
    }
  
    appControllers.controller('WhS_BE_CustomerPricingProductEditorController', customerPricingProductEditorController);
})(appControllers);
