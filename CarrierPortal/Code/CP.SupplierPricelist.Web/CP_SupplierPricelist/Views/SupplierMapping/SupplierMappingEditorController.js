(function (appControllers) {

    "use strict";

    supplierMappingEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'CP_SupplierPricelist_SupplierMappingAPIService'];

    function supplierMappingEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, supplierMappingAPIService) {

        var isEditMode;
        var supplierEntity;
        var userDirectiveApi;
        var userReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var carrierAccountDirectiveApi;
        var carrierAccountReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            
        }
        function defineScope() {
            $scope.carriers = [];
            $scope.selectedCarriers = [];
            $scope.close = function () {
                $scope.modalContext.closeModal();
            }

            $scope.onUserDirectiveReady = function (api) {
                userDirectiveApi = api;
                userReadyPromiseDeferred.resolve();
            }
            $scope.onCarrierAccountDirectiveReady = function (api) {
                carrierAccountDirectiveApi = api;
                carrierAccountReadyPromiseDeferred.resolve();
            }
            $scope.saveCustomerSupplierMapping = function () {
                if (isEditMode) {
                    return updateCustomerSupplierMapping();
                }
                else {
                    return insertCustomerSupplierMapping();
                }
            };
        }
        function load() {
            loadAllControls();
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, LoadUser, LoadCarrierAccount])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }

        function setTitle() {
            //if (isEditMode && customerEntity != undefined)
            //    $scope.title = UtilsService.buildTitleForUpdateEditor(customerEntity.Name, "Supplier");
            //else
                $scope.title = UtilsService.buildTitleForAddEditor("Supplier");
        }


        function LoadUser() {        
            var userLoadPromiseDeferred = UtilsService.createPromiseDeferred();
            userReadyPromiseDeferred.promise.then(function () {
             
                VRUIUtilsService.callDirectiveLoad(userDirectiveApi, undefined, userLoadPromiseDeferred);
            });
            return userLoadPromiseDeferred.promise;
        }

        function LoadCarrierAccount() {
            var carrierAccountLoadPromiseDeferred = UtilsService.createPromiseDeferred();
            carrierAccountReadyPromiseDeferred.promise.then(function () {

                VRUIUtilsService.callDirectiveLoad(carrierAccountDirectiveApi, undefined, carrierAccountLoadPromiseDeferred);
            });
            return carrierAccountLoadPromiseDeferred.promise;
        }

        function buildCustomerSupplierMappingFromScope() {
            var object = {                
                UserId: userDirectiveApi.getSelectedIds(),
                Settings: {
                    MappedSuppliers: $scope.selectedCarriers
                }
            };

            return object;
        }

        function insertCustomerSupplierMapping() {
            var object = buildCustomerSupplierMappingFromScope();
            return supplierMappingAPIService.AddCustomerSupplierMapping(object)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Customer Supplier Mapping", response)) {
                    if ($scope.onSupplierMappingAdded != undefined)
                        $scope.onSupplierMappingAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });

        }

        function updateCustomerSupplierMapping() {
            var object = buildCustomerSupplierMappingFromScope();
            return supplierMappingAPIService.AddCustomerSupplierMapping(object)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Customer Supplier Mapping", response)) {
                    if ($scope.onCustomerAdded != undefined)
                        $scope.onCustomerAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });

        }

       
    }
    appControllers.controller('CP_SupplierPricelist_SupplierMappingEditorController', supplierMappingEditorController);
})(appControllers);