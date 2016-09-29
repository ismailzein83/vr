﻿(function (appControllers) {

    "use strict";

    carrierAccountEditorController.$inject = ['$scope', 'WhS_BE_CarrierAccountAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'WhS_BE_CarrierAccountTypeEnum', 'VRUIUtilsService', 'WhS_BE_CarrierAccountActivationStatusEnum'];

    function carrierAccountEditorController($scope, WhS_BE_CarrierAccountAPIService, UtilsService, VRNotificationService, VRNavigationService, WhS_BE_CarrierAccountTypeEnum, VRUIUtilsService, WhS_BE_CarrierAccountActivationStatusEnum) {
        var carrierProfileDirectiveAPI;
        var carrierProfileReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var customerTimeDirectiveAPI;
        //var customerTimeReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var supplierTimeDirectiveAPI;
       // var supplierTimeReadyPromiseDeferred = UtilsService.createPromiseDeferred();


        var currencySelectorAPI;
        var currencySelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var activationStatusSelectorAPI;
        var activationStatusSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();


        var cusRoutingStatusSelectorAPI;
        var cusRoutingStatusSelectorReadyPromiseDeferred ;//= UtilsService.createPromiseDeferred();


        var supRoutingStatusSelectorAPI;
        var supRoutingStatusSelectorReadyPromiseDeferred;//= UtilsService.createPromiseDeferred();

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
            $scope.scopeModel = {};
            $scope.scopeModel.disableCarrierProfile = (carrierProfileId != undefined) || isEditMode
            $scope.scopeModel.disableCarrierAccountType = isEditMode
            $scope.scopeModel.disableSellingNumberPlan = isEditMode
            $scope.scopeModel.showZoneServiceConfig = false;

            $scope.scopeModel.onBPBusinessRuleSetSelectorReady = function (api) {
                bpBusinessRuleSetDirectiveAPI = api;
                bpBusinessRuleSetReadyPromiseDeferred.resolve();
            }
            $scope.hasSaveCarrierAccountPermission = function () {
                if (isEditMode)
                    return WhS_BE_CarrierAccountAPIService.HasUpdateCarrierAccountPermission();
                else
                    return WhS_BE_CarrierAccountAPIService.HasAddCarrierAccountPermission();
            }

            $scope.scopeModel.SaveCarrierAccount = function () {
                if (isEditMode)
                    return updateCarrierAccount();
                else
                    return insertCarrierAccount();
            };

            $scope.scopeModel.onSellingNumberPlanDirectiveReady = function (api) {
                sellingNumberPlanDirectiveAPI = api;
                var payload;
                if (carrierAccountEntity != undefined && carrierAccountEntity.SellingNumberPlanId != null)
                    payload = {
                        selectedIds: (carrierAccountEntity != undefined && carrierAccountEntity.SellingNumberPlanId != null) ? carrierAccountEntity.SellingNumberPlanId : undefined
                    }
                var setLoader = function (value) { $scope.scopeModel.isLoadingSellingNumberPlan = value };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, sellingNumberPlanDirectiveAPI, payload, setLoader);
            }

            $scope.scopeModel.onZoneServiceConfigSelectorReady = function (api) {
                zoneServiceConfigSelectorAPI = api;
                var payload = {
                    selectedIds: getDefaultServices()
                }
                var setLoader = function (value) { $scope.scopeModel.isLoadingZoneServiceConfig = value };              
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, zoneServiceConfigSelectorAPI, payload, setLoader);
            }

            $scope.scopeModel.onCarrierProfileDirectiveReady = function (api) {
                carrierProfileDirectiveAPI = api;
                carrierProfileReadyPromiseDeferred.resolve();

            }

            $scope.scopeModel.onCustomerTimeSelectorReady = function (api) {
                customerTimeDirectiveAPI = api;
                var setLoader = function (value) { $scope.scopeModel.isLoadingCustomer = value };
                var payload;
                if (carrierAccountEntity != undefined && carrierAccountEntity.CustomerSettings != null && carrierAccountEntity.CustomerSettings.TimeZoneId != 0)
                    payload = {
                        selectedIds: carrierAccountEntity.CustomerSettings.TimeZoneId
                    }
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, customerTimeDirectiveAPI, payload, setLoader);


            }
            $scope.scopeModel.onSupplierTimeSelectorReady = function (api) {
                supplierTimeDirectiveAPI = api;
                var setLoader = function (value) { $scope.scopeModel.isLoadingSupplier = value };
                var payload;
                if (carrierAccountEntity != undefined && carrierAccountEntity.SupplierSettings != null && carrierAccountEntity.SupplierSettings.TimeZoneId != 0)
                    payload = {
                        selectedIds: carrierAccountEntity.SupplierSettings.TimeZoneId
                    }
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, supplierTimeDirectiveAPI, payload, setLoader);

            }

            $scope.scopeModel.onActivationStatusDirectiveReady = function (api) {
                activationStatusSelectorAPI = api;
                activationStatusSelectorReadyPromiseDeferred.resolve();
            }

            $scope.scopeModel.onCustomerRoutingStatusDirectiveReady = function (api) {
                cusRoutingStatusSelectorAPI = api;
                var setLoader = function (value) { $scope.scopeModel.isLoadingCustomer = value };
                var payload = {
                    selectedIds: (carrierAccountEntity != undefined && carrierAccountEntity.CustomerSettings != undefined ? carrierAccountEntity.CustomerSettings.RoutingStatus : null)
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, cusRoutingStatusSelectorAPI, payload, setLoader);
            }
            $scope.scopeModel.onSupplierRoutingStatusDirectiveReady = function (api) {

                supRoutingStatusSelectorAPI = api;
                var setLoader = function (value) { $scope.scopeModel.isLoadingSupplier = value };
                var payload = {
                    selectedIds: (carrierAccountEntity != undefined && carrierAccountEntity.SupplierSettings != undefined ? carrierAccountEntity.SupplierSettings.RoutingStatus : null)
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, supRoutingStatusSelectorAPI, payload, setLoader);
            }
            $scope.scopeModel.onCurrencySelectorReady = function (api) {
                currencySelectorAPI = api;
                currencySelectorReadyPromiseDeferred.resolve();
            }

            $scope.scopeModel.onCarrierTypeSelectionChanged = function () {
                if ($scope.scopeModel.selectedCarrierAccountType != undefined) {

                    if ($scope.scopeModel.selectedCarrierAccountType.value == WhS_BE_CarrierAccountTypeEnum.Customer.value || $scope.scopeModel.selectedCarrierAccountType.value == WhS_BE_CarrierAccountTypeEnum.Exchange.value) {
                        if (sellingNumberPlanDirectiveAPI != undefined) {                           
                            var setLoader = function (value) { $scope.scopeModel.isLoadingSellingNumberPlan = value };
                            var payload = {
                                selectedIds: (carrierAccountEntity != undefined && carrierAccountEntity.SellingNumberPlanId != null) ? carrierAccountEntity.SellingNumberPlanId : undefined
                            }
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, sellingNumberPlanDirectiveAPI, payload, setLoader);
                        }

                        if (customerTimeDirectiveAPI != undefined) {
                            var setLoader = function (value) { $scope.scopeModel.isLoadingCustomer = value };
                            var payload;
                            if (carrierAccountEntity != undefined && carrierAccountEntity.CustomerSettings != null && carrierAccountEntity.CustomerSettings.TimeZoneId != 0)
                                payload = {
                                    selectedIds: carrierAccountEntity.CustomerSettings.TimeZoneId
                                }
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, customerTimeDirectiveAPI, payload, setLoader);
                        }
                        $scope.scopeModel.showSellingNumberPlan = true;

                    }
                    else {
                        $scope.scopeModel.sellingNumberPlan = undefined;
                        $scope.scopeModel.showSellingNumberPlan = false;
                    }

                    if ($scope.scopeModel.selectedCarrierAccountType.value == WhS_BE_CarrierAccountTypeEnum.Supplier.value || $scope.scopeModel.selectedCarrierAccountType.value == WhS_BE_CarrierAccountTypeEnum.Exchange.value) {
                        if (zoneServiceConfigSelectorAPI != undefined) {
                            var setLoader = function (value) { $scope.scopeModel.isLoadingZoneServiceConfig = value };
                            var payload = {
                                selectedIds: getDefaultServices()
                            }
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, zoneServiceConfigSelectorAPI, payload, setLoader);
                        }

                        if (supplierTimeDirectiveAPI != undefined) {
                            var setLoader = function (value) { $scope.scopeModel.isLoadingSupplier = value };
                            var payload;
                            if (carrierAccountEntity != undefined && carrierAccountEntity.SupplierSettings != null && carrierAccountEntity.SupplierSettings.TimeZoneId !=0)
                                payload = {
                                    selectedIds:  carrierAccountEntity.SupplierSettings.TimeZoneId
                                }
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, supplierTimeDirectiveAPI, payload, setLoader);
                        }
                        $scope.scopeModel.showZoneServiceConfig = true;
                       

                    }
                    else {
                        $scope.scopeModel.showZoneServiceConfig = false;
                    }
                       

                }

            }

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {

            $scope.scopeModel.isLoading = true;

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
                        $scope.scopeModel.isLoading = false;
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
                    $scope.scopeModel.isLoading = false;
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
                $scope.scopeModel.name = carrierAccountEntity.NameSuffix;
                for (var i = 0; i < $scope.scopeModel.carrierAccountTypes.length; i++)
                    if (carrierAccountEntity.AccountType == $scope.scopeModel.carrierAccountTypes[i].value)
                        $scope.scopeModel.selectedCarrierAccountType = $scope.scopeModel.carrierAccountTypes[i];

                if (carrierAccountEntity != undefined && carrierAccountEntity.CarrierAccountSettings != undefined) {
                    $scope.scopeModel.mask = carrierAccountEntity.CarrierAccountSettings.Mask;
                    $scope.scopeModel.nominalCapacity = carrierAccountEntity.CarrierAccountSettings.NominalCapacity;
                    if (carrierAccountEntity.CarrierAccountSettings.PriceListSettings) {
                        $scope.scopeModel.fileMask = carrierAccountEntity.CarrierAccountSettings.PriceListSettings.FileMask;
                        $scope.scopeModel.automaticPriceListEmail = carrierAccountEntity.CarrierAccountSettings.PriceListSettings.Email;
                        $scope.scopeModel.automaticPriceListSubjectCode = carrierAccountEntity.CarrierAccountSettings.PriceListSettings.SubjectCode;
                    }
                }
            }
        }

        function loadCarrierAccountType() {
            $scope.scopeModel.carrierAccountTypes = UtilsService.getArrayEnum(WhS_BE_CarrierAccountTypeEnum);
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
            $scope.scopeModel.isLoading = true;
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
                $scope.scopeModel.isLoading = false;
            });
        }

        function updateCarrierAccount() {
            $scope.scopeModel.isLoading = true;
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
                $scope.scopeModel.isLoading = false;
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
                NameSuffix: $scope.scopeModel.name,
                CarrierAccountSettings: {
                    ActivationStatus: activationStatusSelectorAPI.getSelectedIds(),
                    CurrencyId: currencySelectorAPI.getSelectedIds(),
                    Mask: $scope.scopeModel.mask,
                    NominalCapacity: $scope.scopeModel.nominalCapacity,
                    PriceListSettings: {
                        Email: $scope.scopeModel.automaticPriceListEmail,
                        FileMask: $scope.scopeModel.fileMask,
                        SubjectCode: $scope.scopeModel.automaticPriceListSubjectCode,
                        BPBusinessRuleSetIds: bpBusinessRuleSetDirectiveAPI.getSelectedIds()
                    }
                },
                SupplierSettings: {
                    DefaultServices:($scope.scopeModel.showZoneServiceConfig == true) ? getSelectedDefaultServices() : null,
                    TimeZoneId: ($scope.scopeModel.showZoneServiceConfig == true) ? supplierTimeDirectiveAPI.getSelectedIds() : undefined,
                    RoutingStatus:  customerTimeDirectiveAPI!=undefined ? customerTimeDirectiveAPI.getSelectedIds() : undefined

                },
                CustomerSettings: {
                    TimeZoneId: ($scope.scopeModel.showSellingNumberPlan == true) ? customerTimeDirectiveAPI.getSelectedIds() : undefined,
                    RoutingStatus: supplierTimeDirectiveAPI != undefined ? supplierTimeDirectiveAPI.getSelectedIds() : undefined
                }
            };

            if (!isEditMode) {
                obj.CarrierProfileId = carrierProfileDirectiveAPI.getSelectedIds();

                obj.SellingNumberPlanId = ($scope.scopeModel.showSellingNumberPlan == true) ? sellingNumberPlanDirectiveAPI.getSelectedIds() : null;
                obj.AccountType = $scope.scopeModel.selectedCarrierAccountType.value;
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