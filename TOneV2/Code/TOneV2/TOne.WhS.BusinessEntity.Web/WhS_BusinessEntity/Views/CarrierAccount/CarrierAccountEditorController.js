(function (appControllers) {

    "use strict";

    carrierAccountEditorController.$inject = ['$scope', 'WhS_BE_CarrierAccountAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'WhS_BE_CarrierAccountTypeEnum', 'VRUIUtilsService', 'WhS_BE_CarrierAccountActivationStatusEnum'];

    function carrierAccountEditorController($scope, WhS_BE_CarrierAccountAPIService, UtilsService, VRNotificationService, VRNavigationService, WhS_BE_CarrierAccountTypeEnum, VRUIUtilsService, WhS_BE_CarrierAccountActivationStatusEnum) {
        var carrierProfileDirectiveAPI;
        var carrierProfileReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var currencySelectorAPI;
        var currencySelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var activationStatusSelectorAPI;
        var activationStatusSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var sellingNumberPlanDirectiveAPI;

        var isEditMode;
        $scope.scopeModal = {};

        var carrierAccountId;
        var carrierProfileId;
        var carrierAccountEntity;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                carrierAccountId = parameters.CarrierAccountId;
                carrierProfileId = parameters.CarrierProfileId
            }
            isEditMode = (carrierAccountId != undefined);
            $scope.scopeModal.disableCarrierProfile = ((carrierProfileId != undefined));
        }

        function defineScope() {
            $scope.scopeModal.SaveCarrierAccount = function () {
                if (isEditMode) {
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

            $scope.scopeModal.onActivationStatusDirectiveReady = function (api) {
                activationStatusSelectorAPI = api;
                activationStatusSelectorReadyPromiseDeferred.resolve();
            }

            $scope.scopeModal.onCurrencySelectorReady = function (api) {
                currencySelectorAPI = api;
                currencySelectorReadyPromiseDeferred.resolve();
            }

            $scope.scopeModal.onCarrierTypeSelectionChanged = function () {
                if ($scope.scopeModal.selectedCarrierAccountType != undefined) {
                    if ($scope.scopeModal.selectedCarrierAccountType.value == WhS_BE_CarrierAccountTypeEnum.Customer.value || $scope.scopeModal.selectedCarrierAccountType.value == WhS_BE_CarrierAccountTypeEnum.Exchange.value) {
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
                $scope.modalContext.closeModal();
            };
        }

        function load() {

            $scope.scopeModal.isLoading = true;

            if (isEditMode) {
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
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadCarrierAccountType, loadCarrierActivationStatusType, loadFilterBySection, loadCarrierProfileDirective, loadCurrencySelector])
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
                .finally(function () {
                    $scope.scopeModal.isLoading = false;
                });
        }

        function setTitle() {
            $scope.title = isEditMode ? UtilsService.buildTitleForUpdateEditor(carrierAccountEntity ? carrierAccountEntity.NameSuffix : null, 'Carrier Account') : UtilsService.buildTitleForAddEditor('Carrier Account');
        }

        function loadCarrierProfileDirective() {

            var loadCarrierProfilePromiseDeferred = UtilsService.createPromiseDeferred();

            carrierProfileReadyPromiseDeferred.promise
                .then(function () {
                    var directivePayload = {
                        selectedIds: (carrierAccountEntity != undefined ? carrierAccountEntity.CarrierProfileId : (carrierProfileId != undefined ? carrierProfileId : undefined))
                    }
                    VRUIUtilsService.callDirectiveLoad(carrierProfileDirectiveAPI, directivePayload, loadCarrierProfilePromiseDeferred);
                });

            return loadCarrierProfilePromiseDeferred.promise;
        }

        function loadCurrencySelector() {
            var loadCurrencySelectorPromiseDeferred = UtilsService.createPromiseDeferred();

            currencySelectorReadyPromiseDeferred.promise.then(function () {

                var payload = {
                    selectedIds: (carrierAccountEntity != undefined && carrierAccountEntity.CarrierAccountSettings != undefined ? carrierAccountEntity.CarrierAccountSettings.CurrencyId : undefined)
                };

                VRUIUtilsService.callDirectiveLoad(currencySelectorAPI, payload, loadCurrencySelectorPromiseDeferred);

            })

            return loadCurrencySelectorPromiseDeferred.promise;

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
                NameSuffix: $scope.scopeModal.name,
                AccountType: $scope.scopeModal.selectedCarrierAccountType.value,
                CarrierProfileId: carrierProfileDirectiveAPI.getSelectedIds(),
                SellingNumberPlanId: sellingNumberPlanDirectiveAPI.getSelectedIds(),
                SupplierSettings: {},
                CustomerSettings: {},
                CarrierAccountSettings: {
                    ActivationStatus: activationStatusSelectorAPI.getSelectedIds(),
                    CurrencyId: currencySelectorAPI.getSelectedIds()
                }
            };
            return obj;
        }

        function loadFilterBySection() {
            if (carrierAccountEntity != undefined) {
                $scope.scopeModal.name = carrierAccountEntity.NameSuffix;
                for (var i = 0; i < $scope.scopeModal.carrierAccountTypes.length; i++)
                    if (carrierAccountEntity.AccountType == $scope.scopeModal.carrierAccountTypes[i].value)
                        $scope.scopeModal.selectedCarrierAccountType = $scope.scopeModal.carrierAccountTypes[i];
            }
        }

        function loadCarrierAccountType() {
            $scope.scopeModal.carrierAccountTypes = UtilsService.getArrayEnum(WhS_BE_CarrierAccountTypeEnum);
        }

        function loadCarrierActivationStatusType() {
            var loadActivationStatusSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
            activationStatusSelectorReadyPromiseDeferred.promise.then(function () {
                var payload = {
                    selectedIds: (carrierAccountEntity != undefined && carrierAccountEntity.CarrierAccountSettings != undefined ? carrierAccountEntity.CarrierAccountSettings.ActivationStatus : WhS_BE_CarrierAccountActivationStatusEnum.Inactive.value)
                };

                VRUIUtilsService.callDirectiveLoad(activationStatusSelectorAPI, payload, loadActivationStatusSelectorPromiseDeferred);
            })
            return loadActivationStatusSelectorPromiseDeferred.promise;
        }

        function insertCarrierAccount() {
            $scope.scopeModal.isLoading = true;

            var carrierAccountObject = buildCarrierAccountObjFromScope();
            return WhS_BE_CarrierAccountAPIService.AddCarrierAccount(carrierAccountObject)
                .then(function (response) {
                    if (VRNotificationService.notifyOnItemAdded("Carrier Account", response, "Name")) {
                        if ($scope.onCarrierAccountAdded != undefined)
                            $scope.onCarrierAccountAdded(response.InsertedObject);
                        $scope.modalContext.closeModal();
                    }
                })
                .catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                }).finally(function () {
                    $scope.scopeModal.isLoading = false;
                });

        }

        function updateCarrierAccount() {
            $scope.scopeModal.isLoading = true;

            var carrierAccountObject = buildCarrierAccountObjFromScope();
            WhS_BE_CarrierAccountAPIService.UpdateCarrierAccount(carrierAccountObject)
                .then(function (response) {
                    if (VRNotificationService.notifyOnItemUpdated("Carrier Account", response, "Name")) {
                        if ($scope.onCarrierAccountUpdated != undefined)
                            $scope.onCarrierAccountUpdated(response.UpdatedObject);
                        $scope.modalContext.closeModal();
                    }
                })
                .catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                }).finally(function () {
                    $scope.scopeModal.isLoading = false;
                });
        }
    }

    appControllers.controller('WhS_BE_CarrierAccountEditorController', carrierAccountEditorController);
})(appControllers);