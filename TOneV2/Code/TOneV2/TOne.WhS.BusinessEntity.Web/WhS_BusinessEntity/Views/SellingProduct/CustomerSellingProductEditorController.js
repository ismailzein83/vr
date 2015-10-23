(function (appControllers) {

    "use strict";

    customerSellingProductEditorController.$inject = ['$scope', 'WhS_BE_CustomerSellingProductAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService'];

    function customerSellingProductEditorController($scope, WhS_BE_CustomerSellingProductAPIService, UtilsService, VRNotificationService, VRNavigationService) {

        var sellingProductDirectiveAPI;
        var carrierAccountDirectiveAPI;
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
            $scope.allDestinations = false;
            $scope.SaveSellingProduct = function () {
                    return insertSellingProduct();
            };

            $scope.close = function () {
                $scope.modalContext.closeModal()
            };
            $scope.onSellingProductsDirectiveReady = function (api) {
                sellingProductDirectiveAPI = api;
                load();
                if ($scope.disableSellingProduct)
                    sellingProductDirectiveAPI.setData(sellingProductId);
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
            if (carrierAccountDirectiveAPI == undefined || sellingProductDirectiveAPI == undefined)
                return;

            UtilsService.waitMultipleAsyncOperations([loadSellingProducts, loadCarrierAccounts]).then(function () {
                if ($scope.disableSellingProduct && sellingProductDirectiveAPI != undefined)
                    sellingProductDirectiveAPI.setData(sellingProductId);
                if ($scope.disableCarrierAccount && carrierAccountDirectiveAPI != undefined)
                    carrierAccountDirectiveAPI.setData([carrierAccountId]);
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.isGettingData = false;
            }).finally(function () {
                $scope.isGettingData = false;
            });
           
           
        }
        function loadSellingProducts() {
            return sellingProductDirectiveAPI.load();
        }
        function loadCarrierAccounts() {
            return carrierAccountDirectiveAPI.load();
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
            var selectedCustomers = carrierAccountDirectiveAPI.getData();
            var selectedSellingProduct = sellingProductDirectiveAPI.getData();
            for (var i = 0; i < selectedCustomers.length; i++) {
                obj.push({
                    CustomerId: selectedCustomers[i].CarrierAccountId,
                    CustomerName: selectedCustomers[i].Name,
                    SellingProductId: selectedSellingProduct.SellingProductId,
                    SellingProductName: selectedSellingProduct.Name,
                    BED: $scope.beginEffectiveDate,
                    EED: $scope.endEffectiveDate,
                    AllDestinations: $scope.allDestinations
                });
            }
            return obj;
        }
        }
        
  
    appControllers.controller('WhS_BE_CustomerSellingProductEditorController', customerSellingProductEditorController);
})(appControllers);
