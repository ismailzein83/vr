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

        var zoneServiceConfigSelectorAPI;

        var bpBusinessRuleSetDirectiveAPI;
        var bpBusinessRuleSetReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var sellingNumberPlanDirectiveAPI;
        var isEditMode;
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
        }

        function defineScope() {
            $scope.scopeModal = {};
            $scope.scopeModal.disableCarrierProfile = (carrierProfileId != undefined) || isEditMode
            $scope.scopeModal.disableCarrierAccountType = isEditMode
            $scope.scopeModal.disableSellingNumberPlan = isEditMode
            $scope.scopeModal.showZoneServiceConfig = false;

            $scope.scopeModal.onBPBusinessRuleSetSelectorReady = function (api) {
                bpBusinessRuleSetDirectiveAPI = api;
                bpBusinessRuleSetReadyPromiseDeferred.resolve();
            }
            $scope.hasSaveCarrierAccountPermission = function () {
                if (isEditMode)
                    return WhS_BE_CarrierAccountAPIService.HasUpdateCarrierAccountPermission();
                else
                    return WhS_BE_CarrierAccountAPIService.HasAddCarrierAccountPermission();
            }

            $scope.scopeModal.SaveCarrierAccount = function () {
                if (isEditMode)
                    return updateCarrierAccount();
                else
                    return insertCarrierAccount();
            };

            $scope.scopeModal.onSellingNumberPlanDirectiveReady = function (api) {
                sellingNumberPlanDirectiveAPI = api;
            }

            $scope.scopeModal.onZoneServiceConfigSelectorReady = function (api) {
                zoneServiceConfigSelectorAPI = api;
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
                    else {
                        $scope.scopeModal.sellingNumberPlan = undefined;
                        $scope.scopeModal.showSellingNumberPlan = false;
                    }

                    if ($scope.scopeModal.selectedCarrierAccountType.value == WhS_BE_CarrierAccountTypeEnum.Supplier.value || $scope.scopeModal.selectedCarrierAccountType.value == WhS_BE_CarrierAccountTypeEnum.Exchange.value) {
                        if (zoneServiceConfigSelectorAPI != undefined) {
                            var setLoader = function (value) { $scope.scopeModal.isLoadingZoneServiceConfig = value };
                            var payload = {
                                selectedIds: getDefaultServices()
                            }
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, zoneServiceConfigSelectorAPI, payload, setLoader);
                        }
                        $scope.scopeModal.showZoneServiceConfig = true;

                    }
                    else
                        $scope.scopeModal.showZoneServiceConfig = false;

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
            } else
                loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadCarrierAccountType, loadCarrierActivationStatusType, loadStaticSection, loadCarrierProfileDirective, loadCurrencySelector, loadBPBusinessRuleSetSelector])
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

        function loadStaticSection() {
            if (carrierAccountEntity != undefined) {
                $scope.scopeModal.name = carrierAccountEntity.NameSuffix;
                for (var i = 0; i < $scope.scopeModal.carrierAccountTypes.length; i++)
                    if (carrierAccountEntity.AccountType == $scope.scopeModal.carrierAccountTypes[i].value)
                        $scope.scopeModal.selectedCarrierAccountType = $scope.scopeModal.carrierAccountTypes[i];

                if (carrierAccountEntity != undefined && carrierAccountEntity.CarrierAccountSettings != undefined) {
                    $scope.scopeModal.mask = carrierAccountEntity.CarrierAccountSettings.Mask;
                    $scope.scopeModal.nominalCapacity = carrierAccountEntity.CarrierAccountSettings.NominalCapacity;
                    if (carrierAccountEntity.CarrierAccountSettings.PriceListSettings) {
                        $scope.scopeModal.fileMask = carrierAccountEntity.CarrierAccountSettings.PriceListSettings.FileMask;
                        $scope.scopeModal.automaticPriceListEmail = carrierAccountEntity.CarrierAccountSettings.PriceListSettings.Email;
                        $scope.scopeModal.automaticPriceListSubjectCode = carrierAccountEntity.CarrierAccountSettings.PriceListSettings.SubjectCode;
                    }
                }
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
        function loadBPBusinessRuleSetSelector() {
            var loadBPBusinessRuleSetSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
            bpBusinessRuleSetReadyPromiseDeferred.promise.then(function () {
                var payload = {
                    filter: { BPDefinitionId: 3 },
                    selectedIds: (carrierAccountEntity != undefined && carrierAccountEntity.CarrierAccountSettings != undefined && carrierAccountEntity.CarrierAccountSettings.PriceListSettings != undefined ? carrierAccountEntity.CarrierAccountSettings.PriceListSettings.BPBusinessRuleSetIds : undefined)
                };

                VRUIUtilsService.callDirectiveLoad(bpBusinessRuleSetDirectiveAPI, payload, loadBPBusinessRuleSetSelectorPromiseDeferred);
            })
            return loadBPBusinessRuleSetSelectorPromiseDeferred.promise;
        }

        function insertCarrierAccount() {
            $scope.scopeModal.isLoading = true;
            return WhS_BE_CarrierAccountAPIService.AddCarrierAccount(buildCarrierAccountObjFromScope())
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Carrier Account", response, "suffix")) {
                    if ($scope.onCarrierAccountAdded != undefined)
                        $scope.onCarrierAccountAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModal.isLoading = false;
            });
        }

        function updateCarrierAccount() {
            $scope.scopeModal.isLoading = true;
            return WhS_BE_CarrierAccountAPIService.UpdateCarrierAccount(buildCarrierAccountObjFromScope())
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Carrier Account", response, "suffix")) {
                    if ($scope.onCarrierAccountUpdated != undefined)
                        $scope.onCarrierAccountUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModal.isLoading = false;
            });
        }

        function getDefaultServices() {
            if (carrierAccountEntity != undefined && carrierAccountEntity.SupplierSettings != null && carrierAccountEntity.SupplierSettings.DefaultServices != null) {
                var defaultServices = [];

                for (var i = 0 ; i < carrierAccountEntity.SupplierSettings.DefaultServices.length ; i++)
                    defaultServices.push(carrierAccountEntity.SupplierSettings.DefaultServices[i].ServiceId);

                return defaultServices;
            }
        }

        function buildCarrierAccountObjFromScope() {
            var obj = {
                CarrierAccountId: (carrierAccountId != null) ? carrierAccountId : 0,
                NameSuffix: $scope.scopeModal.name,
                CarrierAccountSettings: {
                    ActivationStatus: activationStatusSelectorAPI.getSelectedIds(),
                    CurrencyId: currencySelectorAPI.getSelectedIds(),
                    Mask: $scope.scopeModal.mask,
                    NominalCapacity: $scope.scopeModal.nominalCapacity,
                    PriceListSettings: {
                        Email: $scope.scopeModal.automaticPriceListEmail,
                        FileMask: $scope.scopeModal.fileMask,
                        SubjectCode: $scope.scopeModal.automaticPriceListSubjectCode,
                        BPBusinessRuleSetIds: bpBusinessRuleSetDirectiveAPI.getSelectedIds()
                    }
                },
                SupplierSettings: {
                    DefaultServices: getSelectedDefaultServices()
                },
                CustomerSettings: {}
            };

            if (!isEditMode) {
                obj.CarrierProfileId = carrierProfileDirectiveAPI.getSelectedIds();

                obj.SellingNumberPlanId = sellingNumberPlanDirectiveAPI.getSelectedIds();
                obj.AccountType = $scope.scopeModal.selectedCarrierAccountType.value;
            }

            return obj;
        }


        function getSelectedDefaultServices() {
            var selectedServices = zoneServiceConfigSelectorAPI.getSelectedIds();
            var defaultServices = [];
            if (selectedServices != undefined) {
                for (var i = 0; i < selectedServices.length ; i++) {
                    defaultServices.push({ ServiceId: selectedServices[i] });
                }
                return defaultServices;
            }
        }
    }

    appControllers.controller('WhS_BE_CarrierAccountEditorController', carrierAccountEditorController);
})(appControllers);