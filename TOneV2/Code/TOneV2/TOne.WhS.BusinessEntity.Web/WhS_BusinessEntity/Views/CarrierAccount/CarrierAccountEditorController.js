(function (appControllers) {

    "use strict";

    carrierAccountEditorController.$inject = ['$scope', 'WhS_BE_CarrierAccountAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'WhS_Be_CarrierAccountTypeEnum', 'VRUIUtilsService'];

    function carrierAccountEditorController($scope, WhS_BE_CarrierAccountAPIService, UtilsService, VRNotificationService, VRNavigationService, WhS_Be_CarrierAccountTypeEnum, VRUIUtilsService) {

        var carrierProfileDirectiveAPI;
        var carrierProfileReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var sellingNumberPlanDirectiveAPI;
        var sellingNumberPlanReadyPromiseDeferred;

        var carrierAccountId;
        var carrierProfileId;
        var carrierAccountEntity;
        $scope.scopeModal = {};

        defineScope();
        loadParameters();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                carrierAccountId = parameters.CarrierAccountId;
                carrierProfileId = parameters.CarrierProfileId
            }
            $scope.scopeModal.isEditMode = (carrierAccountId != undefined);
            $scope.scopeModal.disableCarrierProfile = ((carrierProfileId != undefined) && !$scope.scopeModal.isEditMode);

        }

        function defineScope() {
            
            $scope.scopeModal.SaveCarrierAccount = function () {
                if ($scope.scopeModal.isEditMode) {
                    return updateCarrierAccount();
                } else {
                    return insertCarrierAccount();
                }
            };

            $scope.scopeModal.onSellingNumberPlanDirectiveReady = function (api) {
                sellingNumberPlanDirectiveAPI = api;
            }

            $scope.scopeModal.onCarrierProfileDirectiveReady = function (api) {
                carrierProfileDirectiveAPI = api;
                carrierProfileReadyPromiseDeferred.resolve();

            }

            $scope.scopeModal.onCarrierTypeSelectionChanged = function () {
                if ($scope.scopeModal.selectedCarrierAccountType != undefined)
                {
                    if ($scope.scopeModal.selectedCarrierAccountType.value == WhS_Be_CarrierAccountTypeEnum.Customer.value || $scope.scopeModal.selectedCarrierAccountType.value == WhS_Be_CarrierAccountTypeEnum.Exchange.value) {
                        if (sellingNumberPlanDirectiveAPI != undefined) {
                            $scope.scopeModal.showSellingNumberPlan = true;
                            var setLoader = function (value) { $scope.scopeModal.isLoadingSellingNumberPlan = value };
                            var payload = {
                                selectedIds: (carrierAccountEntity != undefined && carrierAccountEntity.SellingNumberPlanId != null) ? carrierAccountEntity.SellingNumberPlanId : undefined
                            }
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, sellingNumberPlanDirectiveAPI, payload, setLoader);
                        }
                    }
                    else
                        $scope.scopeModal.showSellingNumberPlan = false;
                }
          
            }
            

            $scope.scopeModal.close = function () {
                $scope.modalContext.closeModal()
            };

            //$scope.scopeModal.customerTabShow = function () {
            //    if ($scope.selectedCarrierAccountType != undefined && $scope.selectedCarrierAccountType.value != WhS_Be_CarrierAccountTypeEnum.Supplier.value)
            //        return true;
            //    return false;
            //}

            //$scope.scopeModal.SupplierTabShow = function () {
            //    if ($scope.selectedCarrierAccountType != undefined && $scope.selectedCarrierAccountType.value != WhS_Be_CarrierAccountTypeEnum.Customer.value)
            //        return true;
            //    return false;
            //}

        }

        function load() {

            $scope.scopeModal.isLoading = true;

            defineCarrierAccountTypes();

            if ($scope.scopeModal.isEditMode) {
                getCarrierAccount()
                    .then(function () {
                        loadAllControls()
                            .finally(function () {
                                carrierAccountEntity = undefined;
                            });
                    })
                    .catch(function () {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                        $scope.scopeModal.isLoading = false;
                    });
            } else {
                loadAllControls();
            }

        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadFilterBySection, loadCarrierProfileDirective])
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
                .finally(function () {
                    $scope.scopeModal.isLoading = false;
                });
        }

        function loadCarrierProfileDirective() {

            var loadCarrierProfilePromiseDeferred = UtilsService.createPromiseDeferred();

            carrierProfileReadyPromiseDeferred.promise
                .then(function () {
                    var directivePayload = {
                        selectedIds: carrierProfileId != undefined ? carrierProfileId : undefined
                    }
                    VRUIUtilsService.callDirectiveLoad(carrierProfileDirectiveAPI, directivePayload, loadCarrierProfilePromiseDeferred);
                });

            return loadCarrierProfilePromiseDeferred.promise;
        }

        function getCarrierAccount() {
            return WhS_BE_CarrierAccountAPIService.GetCarrierAccount(carrierAccountId)
                .then(function (carrierAccount) {
                    carrierAccountEntity = carrierAccount;
                });
        }

        function buildCarrierAccountObjFromScope() {
            var obj = {
                CarrierAccountId: (carrierAccountId != null) ? carrierAccountId : 0,
                Name: $scope.scopeModal.name,
                AccountType: $scope.scopeModal.selectedCarrierAccountType.value,
                CarrierProfileId: carrierProfileDirectiveAPI.getSelectedIds(),
                SellingNumberPlanId:sellingNumberPlanDirectiveAPI.getSelectedIds(),
                SupplierSettings: {},
                CustomerSettings: {},
            };
            return obj;
        }

        function loadFilterBySection() {
            if (carrierAccountEntity != undefined) {
                $scope.scopeModal.name = carrierAccountEntity.Name;
                for (var i = 0; i < $scope.scopeModal.carrierAccountTypes.length; i++)
                    if (carrierAccountEntity.AccountType == $scope.scopeModal.carrierAccountTypes[i].value)
                        $scope.scopeModal.selectedCarrierAccountType = $scope.scopeModal.carrierAccountTypes[i];
            }
        }

        function insertCarrierAccount() {
            var carrierAccountObject = buildCarrierAccountObjFromScope();
            return WhS_BE_CarrierAccountAPIService.AddCarrierAccount(carrierAccountObject)
                .then(function (response) {
                    if (VRNotificationService.notifyOnItemAdded("Carrier Account", response)) {
                        if ($scope.onCarrierAccountAdded != undefined)
                            $scope.onCarrierAccountAdded(response.InsertedObject);
                        $scope.modalContext.closeModal();
                    }
                })
                .catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });

        }

        function defineCarrierAccountTypes() {
            $scope.scopeModal.carrierAccountTypes = [];
            for (var p in WhS_Be_CarrierAccountTypeEnum)
                $scope.scopeModal.carrierAccountTypes.push(WhS_Be_CarrierAccountTypeEnum[p]);

        }

        function updateCarrierAccount() {
            var carrierAccountObject = buildCarrierAccountObjFromScope();
            WhS_BE_CarrierAccountAPIService.UpdateCarrierAccount(carrierAccountObject)
                .then(function (response) {
                    if (VRNotificationService.notifyOnItemUpdated("Carrier Account", response)) {
                        if ($scope.onCarrierAccountUpdated != undefined)
                            $scope.onCarrierAccountUpdated(response.UpdatedObject);
                        $scope.modalContext.closeModal();
                    }
                })
                .catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
        }
    }

    appControllers.controller('WhS_BE_CarrierAccountEditorController', carrierAccountEditorController);
})(appControllers);