(function (appControllers) {

    "use strict";

    customerSellingProductEditorController.$inject = ['$scope', 'WhS_BE_CustomerSellingProductAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService','VRUIUtilsService'];

    function customerSellingProductEditorController($scope, WhS_BE_CustomerSellingProductAPIService, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

        var sellingProductDirectiveAPI;
        var sellingProductReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var carrierAccountDirectiveAPI;
        var carrierAccountReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var sellingProductId;
        var customerSellingProductId;
        var isEditMode;
        var customerSellingProductEntity;
        var assignableCustomerToSellingProduct;
        defineScope();
        loadParameters();
        load();
        function loadParameters(){
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                customerSellingProductId = parameters.CustomerSellingProductId;
               sellingProductId = parameters.SellingProductId;
                $scope.carrierAccountId = parameters.CarrierAccountId;
            }
            isEditMode = (customerSellingProductId!=undefined)
            $scope.disableSellingProduct = (sellingProductId != undefined);
            $scope.disableCarrierAccount = ($scope.carrierAccountId != undefined || isEditMode);

        }
        function defineScope() {
            $scope.selectedSellingProduct;
            $scope.SaveSellingProduct = function () {
                if (isEditMode) {
                    return updateCustomerSellingProduct();
                }
                else {
                    return insertCustomerSellingProduct();
                }
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
            $scope.onSelectionChanged = function () {
                if (sellingProductDirectiveAPI != undefined && !isEditMode) {
                    var setLoader = function (value) { $scope.isLoadingCustomers = value };
                    var payload = {
                        filter: { AssignableToSellingProductId: sellingProductDirectiveAPI.getSelectedIds() },
                    }
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, carrierAccountDirectiveAPI, payload, setLoader);
                }
              
            }

            $scope.beginEffectiveDate = new Date();
           
        }

        function load() {
            $scope.isLoading = true;
            if (isEditMode) {
                getCustomerSellingProduct().then(function () {
                    loadAllControls()
                        .finally(function () {
                            customerSellingProductEntity = undefined;
                        });
                }).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.isLoading = false;
                });
            }
            else {
   
                loadAllControls();
            }
           
                 
        }
        function loadAllControls() {
  
            return UtilsService.waitMultipleAsyncOperations([isAssignableCustomerToSellingProduct,loadFilterBySection,loadSellingProducts, loadCarrierAccounts])
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
               .finally(function () {
                   $scope.isLoading = false;
               });
        }
        function loadFilterBySection() {

            if (customerSellingProductEntity != undefined) {
                $scope.beginEffectiveDate = customerSellingProductEntity.BED
            }
           
        }
        function getCustomerSellingProduct() {
            return WhS_BE_CustomerSellingProductAPIService.GetCustomerSellingProduct(customerSellingProductId).then(function (customerSellingProduct) {
                customerSellingProductEntity = customerSellingProduct;
            });
        }

        function loadSellingProducts() {
            var sellingProductLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            sellingProductReadyPromiseDeferred.promise
                .then(function () {
                    var directivePayload = sellingProductId != undefined ? sellingProductId : customerSellingProductEntity != undefined ? customerSellingProductEntity.SellingProductId : undefined

                    VRUIUtilsService.callDirectiveLoad(sellingProductDirectiveAPI, directivePayload, sellingProductLoadPromiseDeferred);
                });
            return sellingProductLoadPromiseDeferred.promise;
        }

        function loadCarrierAccounts() {
            var carrierAccountLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            carrierAccountReadyPromiseDeferred.promise
                .then(function () {
                    var directivePayload = {
                      
                        selectedIds: $scope.carrierAccountId != undefined ? [$scope.carrierAccountId] : customerSellingProductEntity != undefined ? [customerSellingProductEntity.CustomerId] : undefined
                    }
                    VRUIUtilsService.callDirectiveLoad(carrierAccountDirectiveAPI, directivePayload, carrierAccountLoadPromiseDeferred);
                });
            return carrierAccountLoadPromiseDeferred.promise;
        }

        function insertCustomerSellingProduct() {
            var customerSellingProductObject = buildSellingProductObjFromScope();
            return WhS_BE_CustomerSellingProductAPIService.AddCustomerSellingProduct(customerSellingProductObject)
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
            var obj;
            var selectedCustomers = carrierAccountDirectiveAPI.getSelectedValues();
            var selectedSellingProduct = $scope.selectedSellingProduct;
            if (isEditMode) {
                obj = {
                    CustomerId: selectedCustomers[0].CarrierAccountId,
                    SellingProductId: selectedSellingProduct.SellingProductId,
                    BED: $scope.beginEffectiveDate,
                };
            } else {
               
                obj = [];
                for (var i = 0; i < selectedCustomers.length; i++) {
                    obj.push({
                        CustomerId: selectedCustomers[i].CarrierAccountId,
                        SellingProductId: selectedSellingProduct.SellingProductId,
                        BED: $scope.beginEffectiveDate,
                    });
                }
            }
           
            return obj;
        }

        function isAssignableCustomerToSellingProduct() {  
            if ($scope.carrierAccountId != undefined)
            {
                return WhS_BE_CustomerSellingProductAPIService.IsAssignableCustomerToSellingProduct($scope.carrierAccountId).then(function (response) {
                    if (response == true) {
                        VRNotificationService.showError("This Customer Is Assigned to Selling Product");
                        $scope.modalContext.closeModal();
                    }
                       
                });
            }
          
        }
        function updateCustomerSellingProduct() {
            var customerSellingProductObject = buildSellingProductObjFromScope();
            customerSellingProductObject.CustomerSellingProductId=customerSellingProductId;
            WhS_BE_CustomerSellingProductAPIService.UpdateCustomerSellingProduct(customerSellingProductObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Customer Selling Product", response)) {
                    if ($scope.onCustomerSellingProductUpdated != undefined)
                        $scope.onCustomerSellingProductUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }
        }
        
  
    appControllers.controller('WhS_BE_CustomerSellingProductEditorController', customerSellingProductEditorController);
})(appControllers);
