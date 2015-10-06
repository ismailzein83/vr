﻿(function (appControllers) {

    "use strict";

    customerPricingProductEditorController.$inject = ['$scope', 'WhS_BE_CustomerPricingProductAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService'];

    function customerPricingProductEditorController($scope, WhS_BE_CustomerPricingProductAPIService, UtilsService, VRNotificationService, VRNavigationService) {

        var pricingProductDirectiveAPI;
        var carrierAccountDirectiveAPI;
        defineScope();
        load();
        function defineScope() {

            $scope.SavePricingProduct = function () {
                    return insertPricingProduct();
            };

            $scope.close = function () {
                $scope.modalContext.closeModal()
            };
            $scope.onPricingProductsDirectiveLoaded = function (api) {
                pricingProductDirectiveAPI = api;
               
            }
            $scope.onCarrierAccountDirectiveLoaded = function (api) {
                carrierAccountDirectiveAPI = api;
            }
            $scope.onPricingProductsSelectionChanged = function () {
             
                  if (pricingProductDirectiveAPI!=undefined)
                      $scope.selectedPricingProduct = pricingProductDirectiveAPI.getData();
            }
            $scope.selectedSupplier;
            $scope.onCarrierAccountSelectionChanged = function () {
                if (carrierAccountDirectiveAPI != undefined)
                    $scope.selectedSupplier = carrierAccountDirectiveAPI.getData();
            }
            $scope.beginEffectiveDate = new Date();
            $scope.endEffectiveDate;

           
        }

        function load() {
 
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
                CustomerId: $scope.selectedSupplier.CarrierAccountId,
                CustomerName: $scope.selectedSupplier.Name,
                PricingProductId: $scope.selectedPricingProduct.PricingProductId,
                PricingProductName:$scope.selectedPricingProduct.Name,
                BED: $scope.beginEffectiveDate,
                EED: $scope.endEffectiveDate

            };
            return obj;
        }
    }
  
    appControllers.controller('WhS_BE_CustomerPricingProductEditorController', customerPricingProductEditorController);
})(appControllers);
