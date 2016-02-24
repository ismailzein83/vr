(function (appControllers) {

    "use strict";

    supplierMappingEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'CP_SupplierPricelist_SupplierMappingAPIService'];

    function supplierMappingEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, supplierMappingAPIService) {

        var isEditMode;
        var supplierMappingId;
        var supplierEntity;
        var userDirectiveApi;

        var userReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var carrierAccountDirectiveApi;
        var carrierAccountReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                supplierMappingId = parameters.SupplierMappingId;
            }
            isEditMode = (supplierMappingId != undefined);
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
            $scope.isLoading = true;
            if (isEditMode) {
                getCustomerSupplierMapping().then(function () {
                    loadAllControls()
                        .finally(function () {
                            supplierEntity = undefined;
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
        function getCustomerSupplierMapping() {
            return supplierMappingAPIService.GetCustomerSupplierMapping(supplierMappingId).then(function (supplier) {
                supplierEntity = supplier;
            });
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
            if (isEditMode && supplierEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(supplierEntity.SupplierMappingId, "Supplier Mapping");
            else
                $scope.title = UtilsService.buildTitleForAddEditor("Supplier Mapping");
        }


        function LoadUser() {        
            var userLoadPromiseDeferred = UtilsService.createPromiseDeferred();
            userReadyPromiseDeferred.promise.then(function () {
                var obj;
                if (supplierEntity != undefined && supplierEntity.UserId != undefined) {
                    obj = {
                        selectedIds: supplierEntity.UserId
                    }
                    $scope.disableUser = true;
                }
                   
                VRUIUtilsService.callDirectiveLoad(userDirectiveApi, obj, userLoadPromiseDeferred);
            });
            return userLoadPromiseDeferred.promise;
        }

        function LoadCarrierAccount() {
            var carrierAccountLoadPromiseDeferred = UtilsService.createPromiseDeferred();
            carrierAccountReadyPromiseDeferred.promise.then(function () {
                var obj;
                if (supplierEntity != undefined && supplierEntity.Settings != undefined && supplierEntity.Settings.MappedSuppliers!=undefined)
                    obj = {
                        selectedIds: UtilsService.getPropValuesFromArray(supplierEntity.Settings.MappedSuppliers, "SupplierId")
                    }
                VRUIUtilsService.callDirectiveLoad(carrierAccountDirectiveApi, obj, carrierAccountLoadPromiseDeferred);
            });
            return carrierAccountLoadPromiseDeferred.promise;
        }

        function buildCustomerSupplierMappingFromScope() {
            var object = {
                SupplierMappingId: (supplierMappingId != undefined) ? supplierMappingId : 0,
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
            return supplierMappingAPIService.UpdateCustomerSupplierMapping(object)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Customer Supplier Mapping", response ,  "User")) {
                    if ($scope.onSupplierMappingUpdated != undefined)
                        $scope.onSupplierMappingUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });

        }

       
    }
    appControllers.controller('CP_SupplierPricelist_SupplierMappingEditorController', supplierMappingEditorController);
})(appControllers);