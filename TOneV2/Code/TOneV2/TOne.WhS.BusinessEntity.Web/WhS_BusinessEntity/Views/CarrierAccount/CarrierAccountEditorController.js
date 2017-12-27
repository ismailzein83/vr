(function (appControllers) {
    "use strict";

    carrierAccountEditorController.$inject = ['$scope', 'WhS_BE_CarrierAccountAPIService', 'WhS_BE_CarrierProfileAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'WhS_BE_CarrierAccountTypeEnum', 'VRUIUtilsService', 'WhS_BE_CarrierAccountActivationStatusEnum', 'WhS_BE_CustomerCountryAPIService', 'WhS_BE_SellingProductAPIService', 'WhS_BE_RoutingStatusEnum', 'WhS_BE_SaleAreaSettingsContextEnum', 'VRDateTimeService'];

    function carrierAccountEditorController($scope, WhS_BE_CarrierAccountAPIService, WhS_BE_CarrierProfileAPIService, UtilsService, VRNotificationService, VRNavigationService, WhS_BE_CarrierAccountTypeEnum, VRUIUtilsService, WhS_BE_CarrierAccountActivationStatusEnum, WhS_BE_CustomerCountryAPIService, WhS_BE_SellingProductAPIService, WhS_BE_RoutingStatusEnum, WhS_BE_SaleAreaSettingsContextEnum, VRDateTimeService) {

        // Definition
        var carrierProfileSelectorAPI;
        var carrierProfileSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var activationStatusSelectorAPI;
        var activationStatusSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var carrierAccountTypeSelectorAPI;
        var carrierAccountTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var currencySelectorAPI;
        var currencySelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var companySettingsSelectorAPI;
        var companySettingSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        // Pricing Settings
        var bpBusinessRuleSetDirectiveAPI;
        var bpBusinessRuleSetSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        // Customer Settings
        var customerTimeZoneSelectorAPI;
        var customerTimeZoneSelectorReadyDeferred;

        var sellingNumberPlanSelectorAPI;
        var sellingNumberPlanSelectorReadyDeferred;
        var sellingNumberPlanSelectedDeferred;

        var sellingProductSelectorAPI;
        var sellingProductSelectorReadyDeferred;

        var customerRoutingStatusSelectorAPI;
        var customerRoutingStatusSelectorReadyDeferred;

        var priceListSettingsEditorAPI;
        var priceListSettingsEditorReadyDeferred;

        var pricingSettingsEditorAPI;
        var pricingSettingsEditorReadyDeferred;

        // Supplier Settings
        var supplierTimeZoneSelectorAPI;
        var supplierTimeZoneSelectorReadyDeferred;

        var zoneServiceConfigSelectorAPI;
        var zoneServiceConfigSelectorReadyDeferred;

        var supplierRoutingStatusSelectorAPI;
        var supplierRoutingStatusSelectorReadyDeferred;

        var isEditMode;
        var context;
        var isViewHistoryMode;
        var carrierAccountId;
        var carrierProfileId;
        var carrierProfileId;
        var carrierAccountEntity;
        var carrierProfileEntity;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                carrierAccountId = parameters.CarrierAccountId;
                carrierProfileId = parameters.CarrierProfileId;
                context = parameters.context;
            }
            isEditMode = (carrierAccountId != undefined);
            isViewHistoryMode = (context != undefined && context.historyId != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.carrierAccountTypes = UtilsService.getArrayEnum(WhS_BE_CarrierAccountTypeEnum);

            $scope.scopeModel.disableCarrierProfile = (carrierProfileId != undefined) || isEditMode;
            $scope.scopeModel.disableCarrierAccountType = isEditMode;
            $scope.scopeModel.disableSellingNumberPlan = isEditMode;
            $scope.scopeModel.showZoneServiceConfig = false;

            // Definition
            $scope.scopeModel.onCarrierProfileSelectorReady = function (api) {
                carrierProfileSelectorAPI = api;
                carrierProfileSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onActivationStatusSelectorReady = function (api) {
                activationStatusSelectorAPI = api;
                activationStatusSelectorReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.onCarrierAccountTypeSelectorReady = function (api) {
                carrierAccountTypeSelectorAPI = api;
                carrierAccountTypeSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onCarrierAccountTypeChanged = function () {

                if ($scope.scopeModel.selectedCarrierAccountType != undefined) {
                    if ($scope.scopeModel.selectedCarrierAccountType.value == WhS_BE_CarrierAccountTypeEnum.Customer.value || $scope.scopeModel.selectedCarrierAccountType.value == WhS_BE_CarrierAccountTypeEnum.Exchange.value) {
                        if (sellingNumberPlanSelectorAPI != undefined) {
                            var setLoader = function (value) { $scope.scopeModel.isLoadingSellingNumberPlan = value; };
                            var payload = {
                                selectedIds: (carrierAccountEntity != undefined && carrierAccountEntity.SellingNumberPlanId != null) ? carrierAccountEntity.SellingNumberPlanId : undefined,
                                selectifsingleitem: (!isEditMode) ? true : false
                            };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, sellingNumberPlanSelectorAPI, payload, setLoader);
                        }

                        if (customerTimeZoneSelectorAPI != undefined) {
                            var setLoader = function (value) { $scope.scopeModel.isLoadingCustomer = value; };
                            var payload;
                            if (carrierAccountEntity != undefined && carrierAccountEntity.CustomerSettings != null && carrierAccountEntity.CustomerSettings.TimeZoneId != 0)
                                payload = {
                                    selectedIds: carrierAccountEntity.CustomerSettings.TimeZoneId
                                };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, customerTimeZoneSelectorAPI, payload, setLoader);
                        }
                        $scope.scopeModel.showSellingNumberPlan = true;

                    }
                    else {
                        $scope.scopeModel.sellingNumberPlan = undefined;
                        $scope.scopeModel.showSellingNumberPlan = false;
                    }

                    if ($scope.scopeModel.selectedCarrierAccountType.value == WhS_BE_CarrierAccountTypeEnum.Supplier.value || $scope.scopeModel.selectedCarrierAccountType.value == WhS_BE_CarrierAccountTypeEnum.Exchange.value) {
                        if (zoneServiceConfigSelectorAPI != undefined) {
                            var setLoader = function (value) { $scope.scopeModel.isLoadingZoneServiceConfigSelector = value; };
                            var payload = {
                                selectedIds: getDefaultServices(),
                                selectminweight: (!isEditMode) ? true : false
                            };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, zoneServiceConfigSelectorAPI, payload, setLoader);
                        }

                        if (supplierTimeZoneSelectorAPI != undefined) {
                            var setLoader = function (value) { $scope.scopeModel.isLoadingSupplier = value; };
                            var payload;
                            if (carrierAccountEntity != undefined && carrierAccountEntity.SupplierSettings != null && carrierAccountEntity.SupplierSettings.TimeZoneId != 0)
                                payload = {
                                    selectedIds: carrierAccountEntity.SupplierSettings.TimeZoneId
                                };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, supplierTimeZoneSelectorAPI, payload, setLoader);
                        }
                        $scope.scopeModel.showZoneServiceConfig = true;
                    }
                    else {
                        $scope.scopeModel.showZoneServiceConfig = false;
                    }
                }
            };
            $scope.scopeModel.onCurrencySelectorReady = function (api) {
                currencySelectorAPI = api;
                currencySelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onCompanySettingSelectorReady = function (api) {
                companySettingsSelectorAPI = api;
                companySettingSelectorReadyDeferred.resolve();
            };

            // Pricing Settings
            $scope.scopeModel.onBPBusinessRuleSetSelectorReady = function (api) {
                bpBusinessRuleSetDirectiveAPI = api;
                bpBusinessRuleSetSelectorReadyDeferred.resolve();
            };

            // Customer Settings
            $scope.scopeModel.onCustomerTimeSelectorReady = function (api) {
                customerTimeZoneSelectorAPI = api;
                var setLoader = function (value) { $scope.scopeModel.isLoadingCustomerTimeZoneSelector = value; };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, customerTimeZoneSelectorAPI, undefined, setLoader, customerTimeZoneSelectorReadyDeferred);
            };
            $scope.scopeModel.onSellingNumberPlanSelectorReady = function (api) {
                sellingNumberPlanSelectorAPI = api;
                var setLoader = function (value) { $scope.scopeModel.isLoadingSellingNumberPlanSelector = value; };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, sellingNumberPlanSelectorAPI, { selectifsingleitem: (!isEditMode) ? true : false }, setLoader, sellingNumberPlanSelectorReadyDeferred);
            };
            $scope.scopeModel.onSellingNumberPlanChanged = function (selectedSellingNumberPlan) {
                if (selectedSellingNumberPlan == undefined)
                    sellingProductSelectorAPI.clearDataSource();
                else if (sellingNumberPlanSelectedDeferred != undefined)
                    sellingNumberPlanSelectedDeferred.resolve();
                else {
                    var sellingProductSelectorPayload = {
                        filter: {
                            SellingNumberPlanId: selectedSellingNumberPlan.SellingNumberPlanId
                        },
                        selectifsingleitem: (!isEditMode) ? true : false
                    };
                    var setLoader = function (value) { $scope.scopeModel.isLoadingSellingProductSelector = value; };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, sellingProductSelectorAPI, sellingProductSelectorPayload, setLoader, sellingProductSelectorReadyDeferred);
                }
            };
            $scope.scopeModel.onSellingProductSelectorReady = function (api) {
                sellingProductSelectorAPI = api;
                if (sellingProductSelectorReadyDeferred != undefined)
                    sellingProductSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onCustomerRoutingStatusSelectorReady = function (api) {
                customerRoutingStatusSelectorAPI = api;
                var setLoader = function (value) { $scope.scopeModel.isLoadingCustomerRoutingStatusSelector = value; };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, customerRoutingStatusSelectorAPI, { selectedIds: WhS_BE_RoutingStatusEnum.Enabled.value }, setLoader, customerRoutingStatusSelectorReadyDeferred);
            };

            $scope.scopeModel.onPricingSettingsEditorReady = function (api) {
                pricingSettingsEditorAPI = api;
                var pricingSettingsPayload = {
                    directiveContext: WhS_BE_SaleAreaSettingsContextEnum.Customer.value
                };
                var setLoader = function (value) { $scope.scopeModel.isLoadingPricingSettingsEditor = value; };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, pricingSettingsEditorAPI, pricingSettingsPayload, setLoader, pricingSettingsEditorReadyDeferred);
            };

            $scope.scopeModel.onPriceListSettingsEditorReady = function (api) {
                priceListSettingsEditorAPI = api;
                var priceListSettingsPayload = {
                    directiveContext: WhS_BE_SaleAreaSettingsContextEnum.Customer.value
                };
                var setLoader = function (value) { $scope.scopeModel.isLoadingPriceListSettingsEditor = value; };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, priceListSettingsEditorAPI, priceListSettingsPayload, setLoader, priceListSettingsEditorReadyDeferred);
            };

            // Supplier Settings
            $scope.scopeModel.onSupplierTimeSelectorReady = function (api) {
                supplierTimeZoneSelectorAPI = api;
                var setLoader = function (value) { $scope.scopeModel.isLoadingSupplierTimeZoneSelector = value; };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, supplierTimeZoneSelectorAPI, undefined, setLoader, supplierTimeZoneSelectorReadyDeferred);

            };
            $scope.scopeModel.onZoneServiceConfigSelectorReady = function (api) {
                zoneServiceConfigSelectorAPI = api;
                var setLoader = function (value) { $scope.scopeModel.isLoadingZoneServiceConfigSelector = value; };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, zoneServiceConfigSelectorAPI, { selectminweight: (!isEditMode) ? true : false }, setLoader, zoneServiceConfigSelectorReadyDeferred);
            };
            $scope.scopeModel.onSupplierRoutingStatusSelectorReady = function (api) {
                supplierRoutingStatusSelectorAPI = api;
                var setLoader = function (value) { $scope.scopeModel.isLoadingSupplierRoutingStatusSelector = value; };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, supplierRoutingStatusSelectorAPI, { selectedIds: WhS_BE_RoutingStatusEnum.Enabled.value }, setLoader, supplierRoutingStatusSelectorReadyDeferred);
            };

            $scope.hasSaveCarrierAccountPermission = function () {
                if (isEditMode)
                    return WhS_BE_CarrierAccountAPIService.HasUpdateCarrierAccountPermission();
                else
                    return WhS_BE_CarrierAccountAPIService.HasAddCarrierAccountPermission();
            };

            $scope.scopeModel.saveCarrierAccount = function () {
                validateCarrierAccountCurrency().then(function (response) {
                    if (response) {
                        if (isEditMode)
                            return updateCarrierAccount();
                        else
                            return insertCarrierAccount();
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

            function validateCarrierAccountCurrency() {
                var continueSaveCarrierAccountPromiseDeferred = UtilsService.createPromiseDeferred();


                if (isCustomer()) {
                    var sellingProductId = sellingProductSelectorAPI.getSelectedIds();
                    var carrierAccountCurrencyId = currencySelectorAPI.getSelectedIds();
                    var sellingProductCurrencyId;

                    WhS_BE_SellingProductAPIService.GetSellingProductCurrencyId(sellingProductId).then(function (response) {
                        sellingProductCurrencyId = response;
                        if (carrierAccountCurrencyId == sellingProductCurrencyId)
                            continueSaveCarrierAccountPromiseDeferred.resolve(true);
                        else {
                            VRNotificationService.showConfirmation("Carrier account and selling product have different currency. Are you sure you want to continue ?").then(function (result) {
                                continueSaveCarrierAccountPromiseDeferred.resolve(result);
                            });
                        }
                    }).catch(function (error) {
                        continueSaveCarrierAccountPromiseDeferred.reject(error);
                    });

                    return continueSaveCarrierAccountPromiseDeferred.promise;
                }
                else {
                    continueSaveCarrierAccountPromiseDeferred.resolve(true);
                    return continueSaveCarrierAccountPromiseDeferred.promise;
                }
            }

        }

        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                getCarrierAccount().then(function () {
                    loadAllControls().finally(function () {
                        carrierAccountEntity = undefined;
                    });
                })
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
            else if (isViewHistoryMode) {
                getCarrierAccountHistory().then(function () {
                    loadAllControls().finally(function () {
                        carrierAccountEntity = undefined;
                    });
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.isLoading = false;
                });
            }
            else
                loadAllControls();
        }

        function getCarrierAccount() {
            return WhS_BE_CarrierAccountAPIService.GetCarrierAccount(carrierAccountId)
                .then(function (carrierAccount) {
                    carrierAccountEntity = carrierAccount;
                });
        }
        function getCarrierProfile() {
            return WhS_BE_CarrierProfileAPIService.GetCarrierProfile(carrierProfileId).then(function (carrierAccount) {
                carrierProfileEntity = carrierAccount;
            });
        }
        function getCarrierAccountHistory() {
            return WhS_BE_CarrierAccountAPIService.GetCarrierAccountHistoryDetailbyHistoryId(context.historyId).then(function (response) {
                carrierAccountEntity = response;

            });
        }

        function loadAllControls() {

            var operations = [
                setTitle,
                loadDefinitionTab,
            ];

            setGlobalVarsAndAddOperations();
            function setGlobalVarsAndAddOperations() {
                if (carrierAccountEntity != undefined) {
                    switch (carrierAccountEntity.AccountType) {
                        case WhS_BE_CarrierAccountTypeEnum.Customer.value:
                            setCustomerGlobalVars();
                            operations.push(loadCustomerSettingsTab);
                            break;
                        case WhS_BE_CarrierAccountTypeEnum.Supplier.value:
                            setSupplierGlobalVars();
                            operations.push(loadSupplierSettingsTab);
                            break;
                        case WhS_BE_CarrierAccountTypeEnum.Exchange.value:
                            setCustomerGlobalVars();
                            setSupplierGlobalVars();
                            operations.push(loadCustomerSettingsTab);
                            operations.push(loadSupplierSettingsTab);
                            break;
                    }
                }
                function setCustomerGlobalVars() {
                    customerTimeZoneSelectorReadyDeferred = UtilsService.createPromiseDeferred();
                    sellingNumberPlanSelectorReadyDeferred = UtilsService.createPromiseDeferred();
                    sellingNumberPlanSelectedDeferred = UtilsService.createPromiseDeferred();
                    sellingProductSelectorReadyDeferred = UtilsService.createPromiseDeferred();
                    customerRoutingStatusSelectorReadyDeferred = UtilsService.createPromiseDeferred();
                    priceListSettingsEditorReadyDeferred = UtilsService.createPromiseDeferred();
                    pricingSettingsEditorReadyDeferred = UtilsService.createPromiseDeferred();
                }
                function setSupplierGlobalVars() {
                    supplierTimeZoneSelectorReadyDeferred = UtilsService.createPromiseDeferred();
                    zoneServiceConfigSelectorReadyDeferred = UtilsService.createPromiseDeferred();
                    supplierRoutingStatusSelectorReadyDeferred = UtilsService.createPromiseDeferred();
                }
            }

            return UtilsService.waitMultipleAsyncOperations(operations).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function setTitle() {
            if (isEditMode)
                WhS_BE_CarrierAccountAPIService.GetCarrierAccountName(carrierAccountId)
                .then(function (response) {
                    $scope.title = UtilsService.buildTitleForUpdateEditor(response, 'Carrier Account', $scope);
                });
            else if (isViewHistoryMode && carrierAccountEntity != undefined)
                $scope.title = "View Carrier Account: " + carrierAccountEntity.NameSuffix;
            else
                $scope.title = UtilsService.buildTitleForAddEditor('Carrier Account');
        }

        function loadDefinitionTab() {
            return UtilsService.waitMultipleAsyncOperations([loadStaticData, loadCarrierProfileSelector, loadCarrierActivationStatusSelector, loadCarrierAccountTypeSelector, loadCurrencySelector, loadCompanySettingSelector]);
        }
        function loadStaticData() {
            if (!isEditMode && !isViewHistoryMode) {
                $scope.scopeModel.nominalCapacity = 1;
                return;
            }
            if (carrierAccountEntity != undefined) {
                $scope.scopeModel.name = carrierAccountEntity.NameSuffix;
                for (var i = 0; i < $scope.scopeModel.carrierAccountTypes.length; i++)
                    if (carrierAccountEntity.AccountType == $scope.scopeModel.carrierAccountTypes[i].value)
                        $scope.scopeModel.selectedCarrierAccountType = $scope.scopeModel.carrierAccountTypes[i];

                if (carrierAccountEntity != undefined) {
                    if (carrierAccountEntity.CarrierAccountSettings != undefined) {
                        $scope.scopeModel.mask = carrierAccountEntity.CarrierAccountSettings.Mask;
                        $scope.scopeModel.nominalCapacity = carrierAccountEntity.CarrierAccountSettings.NominalCapacity;
                        $scope.scopeModel.isInterconnectSwitch = carrierAccountEntity.CarrierAccountSettings.IsInterconnectSwitch;

                        if (carrierAccountEntity.CarrierAccountSettings.PriceListSettings) {
                            $scope.scopeModel.fileMask = carrierAccountEntity.CarrierAccountSettings.PriceListSettings.FileMask;
                            $scope.scopeModel.automaticPriceListEmail = carrierAccountEntity.CarrierAccountSettings.PriceListSettings.Email;
                            $scope.scopeModel.automaticPriceListSubjectCode = carrierAccountEntity.CarrierAccountSettings.PriceListSettings.SubjectCode;
                        }
                    }
                    if (carrierAccountEntity.SupplierSettings) {
                        $scope.scopeModel.supplierEffectiveDateDayOffset = carrierAccountEntity.SupplierSettings.EffectiveDateDayOffset;
                    }

                    if (carrierAccountEntity.CustomerSettings) {
                        //$scope.scopeModel.isAToZ = carrierAccountEntity.CustomerSettings.IsAToZ;
                        $scope.scopeModel.customerInvoiceTimeZone = carrierAccountEntity.CustomerSettings.InvoiceTimeZone;

                    }
                }
            }
        }
        function loadCarrierProfileSelector() {

            var loadCarrierProfilePromiseDeferred = UtilsService.createPromiseDeferred();

            carrierProfileSelectorReadyDeferred.promise
                .then(function () {
                    var directivePayload = {
                        selectedIds: (carrierAccountEntity != undefined ? carrierAccountEntity.CarrierProfileId : (carrierProfileId != undefined ? carrierProfileId : undefined))
                    };
                    VRUIUtilsService.callDirectiveLoad(carrierProfileSelectorAPI, directivePayload, loadCarrierProfilePromiseDeferred);
                });

            return loadCarrierProfilePromiseDeferred.promise;
        }
        function loadCarrierActivationStatusSelector() {
            var loadActivationStatusSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
            activationStatusSelectorReadyPromiseDeferred.promise.then(function () {
                var payload = {
                    selectedIds: (carrierAccountEntity != undefined && carrierAccountEntity.CarrierAccountSettings != undefined ? carrierAccountEntity.CarrierAccountSettings.ActivationStatus : WhS_BE_CarrierAccountActivationStatusEnum.Active.value)
                };

                VRUIUtilsService.callDirectiveLoad(activationStatusSelectorAPI, payload, loadActivationStatusSelectorPromiseDeferred);
            });
            return loadActivationStatusSelectorPromiseDeferred.promise;
        }
        function loadCarrierAccountTypeSelector() {
            return carrierAccountTypeSelectorReadyDeferred.promise;
        }
        function loadCurrencySelector() {
            var loadCurrencySelectorPromiseDeferred = UtilsService.createPromiseDeferred();

            currencySelectorReadyDeferred.promise.then(function () {

                var payload = {
                    selectedIds: (carrierAccountEntity != undefined && carrierAccountEntity.CarrierAccountSettings != undefined ? carrierAccountEntity.CarrierAccountSettings.CurrencyId : undefined)
                };

                VRUIUtilsService.callDirectiveLoad(currencySelectorAPI, payload, loadCurrencySelectorPromiseDeferred);

            });

            return loadCurrencySelectorPromiseDeferred.promise;

        }
        function loadCompanySettingSelector() {
            var loadCompanySettingSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

            companySettingSelectorReadyDeferred.promise.then(function () {

                var payload = {
                    selectedIds: (carrierAccountEntity != undefined && carrierAccountEntity.CarrierAccountSettings != undefined ? carrierAccountEntity.CarrierAccountSettings.CompanySettingId : undefined)
                };

                VRUIUtilsService.callDirectiveLoad(companySettingsSelectorAPI, payload, loadCompanySettingSelectorPromiseDeferred);

            });

            return loadCompanySettingSelectorPromiseDeferred.promise;
        }

        function loadPricingSettingsTab() {
            return UtilsService.waitMultipleAsyncOperations([loadBPBusinessRuleSetSelector]);
        }
        function loadBPBusinessRuleSetSelector() {
            var loadBPBusinessRuleSetSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
            bpBusinessRuleSetSelectorReadyDeferred.promise.then(function () {
                var payload = {
                    filter: { BPDefinitionId: "6EF1A7A7-9B70-4A8F-B94E-F9BB5E347CF2" },
                    selectedIds: (carrierAccountEntity != undefined && carrierAccountEntity.CarrierAccountSettings != undefined && carrierAccountEntity.CarrierAccountSettings.PriceListSettings != undefined ? carrierAccountEntity.CarrierAccountSettings.PriceListSettings.BPBusinessRuleSetIds : undefined)
                };

                VRUIUtilsService.callDirectiveLoad(bpBusinessRuleSetDirectiveAPI, payload, loadBPBusinessRuleSetSelectorPromiseDeferred);
            });
            return loadBPBusinessRuleSetSelectorPromiseDeferred.promise;
        }

        function loadCustomerSettingsTab() {
            var promises = [];

            var loadCustomerTimeZoneSelectorPromise = loadCustomerTimeZoneSelector();
            promises.push(loadCustomerTimeZoneSelectorPromise);

            var loadSellingNumberPlanSelectorPromise = loadSellingNumberPlanSelector();
            promises.push(loadSellingNumberPlanSelectorPromise);

            if (carrierAccountEntity != undefined) {
                var loadSellingProductSelectorPromise = loadSellingProductSelector();
                promises.push(loadSellingProductSelectorPromise);

                var areEffectiveOrFutureCountriesSoldToCustomerPromise = areEffectiveOrFutureCountriesSoldToCustomer();
                promises.push(areEffectiveOrFutureCountriesSoldToCustomerPromise);
            }

            var loadCustomerRoutingStatusSelectorPromise = loadCustomerRoutingStatusSelector();
            promises.push(loadCustomerRoutingStatusSelectorPromise);

            var loadPriceListSettingsPromise = loadPriceListSettings();
            promises.push(loadPriceListSettingsPromise);

            var loadPricingSettingsPromise = loadPricingSettings();
            promises.push(loadPricingSettingsPromise);

            return UtilsService.waitMultiplePromises(promises);
        }
        function loadCustomerTimeZoneSelector() {
            var customerTimeZoneSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            customerTimeZoneSelectorReadyDeferred.promise.then(function () {
                customerTimeZoneSelectorReadyDeferred = undefined;
                var customerTimeZoneSelectorPayload = {
                    selectedIds: (carrierAccountEntity != undefined) ? carrierAccountEntity.CustomerSettings.TimeZoneId : undefined
                };
                VRUIUtilsService.callDirectiveLoad(customerTimeZoneSelectorAPI, customerTimeZoneSelectorPayload, customerTimeZoneSelectorLoadDeferred);
            });

            return customerTimeZoneSelectorLoadDeferred.promise;
        }
        function loadSellingNumberPlanSelector() {
            var sellingNumberPlanSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            sellingNumberPlanSelectorReadyDeferred.promise.then(function () {
                sellingNumberPlanSelectorReadyDeferred = undefined;
                var sellingNumberPlanSelectorPayload = {
                    selectedIds: (carrierAccountEntity != undefined) ? carrierAccountEntity.SellingNumberPlanId : undefined,
                    selectifsingleitem: (!isEditMode) ? true : false
                };
                VRUIUtilsService.callDirectiveLoad(sellingNumberPlanSelectorAPI, sellingNumberPlanSelectorPayload, sellingNumberPlanSelectorLoadDeferred);
            });

            return sellingNumberPlanSelectorLoadDeferred.promise;
        }
        function loadSellingProductSelector() {
            var sellingProductSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            UtilsService.waitMultiplePromises([sellingProductSelectorReadyDeferred.promise, sellingNumberPlanSelectedDeferred.promise]).then(function () {
                sellingProductSelectorReadyDeferred = undefined;
                sellingNumberPlanSelectedDeferred = undefined;
                var sellingProductSelectorPayload = {
                    selectifsingleitem: (!isEditMode) ? true : false
                };
                if (carrierAccountEntity != undefined) {
                    sellingProductSelectorPayload.filter = {
                        SellingNumberPlanId: carrierAccountEntity.SellingNumberPlanId
                    };
                    sellingProductSelectorPayload.selectedIds = carrierAccountEntity.SellingProductId;
                }
                VRUIUtilsService.callDirectiveLoad(sellingProductSelectorAPI, sellingProductSelectorPayload, sellingProductSelectorLoadDeferred);
            });

            return sellingProductSelectorLoadDeferred.promise;
        }
        function areEffectiveOrFutureCountriesSoldToCustomer() {
            return WhS_BE_CustomerCountryAPIService.AreEffectiveOrFutureCountriesSoldToCustomer(carrierAccountEntity.CarrierAccountId, UtilsService.getDateFromDateTime(VRDateTimeService.getNowDateTime())).then(function (response) {
                $scope.scopeModel.isSellingProductSelectorDisabled = (response === true);
            });
        }
        function loadCustomerRoutingStatusSelector() {
            var customerRoutingStatusSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            customerRoutingStatusSelectorReadyDeferred.promise.then(function () {
                customerRoutingStatusSelectorReadyDeferred = undefined;
                var customerRoutingStatusSelectorPayload = {
                    selectedIds: (carrierAccountEntity != undefined) ? carrierAccountEntity.CustomerSettings.RoutingStatus : WhS_BE_RoutingStatusEnum.Enabled.value
                };
                VRUIUtilsService.callDirectiveLoad(customerRoutingStatusSelectorAPI, customerRoutingStatusSelectorPayload, customerRoutingStatusSelectorLoadDeferred);
            });

            return customerRoutingStatusSelectorLoadDeferred.promise;
        }

        function loadPriceListSettings() {
            var priceListSettingsEditorLoadDeferred = UtilsService.createPromiseDeferred();
            priceListSettingsEditorReadyDeferred.promise.then(function () {
                priceListSettingsEditorReadyDeferred = undefined;
                var priceListSettingsPayload = {
                    directiveContext: WhS_BE_SaleAreaSettingsContextEnum.Customer.value
                };
                if (carrierAccountEntity != undefined && carrierAccountEntity.CustomerSettings != undefined) {
                    priceListSettingsPayload.data = carrierAccountEntity.CustomerSettings.PricelistSettings;
                }
                VRUIUtilsService.callDirectiveLoad(priceListSettingsEditorAPI, priceListSettingsPayload, priceListSettingsEditorLoadDeferred);
            });
            return priceListSettingsEditorLoadDeferred.promise;
        }

        function loadPricingSettings() {
            var pricingSettingsEditorLoadDeferred = UtilsService.createPromiseDeferred();
            pricingSettingsEditorReadyDeferred.promise.then(function () {
                pricingSettingsEditorReadyDeferred = undefined;
                var pricingSettingsPayload = {
                    directiveContext: WhS_BE_SaleAreaSettingsContextEnum.Customer.value
                };
                if (carrierAccountEntity != undefined && carrierAccountEntity.CustomerSettings != undefined) {
                    pricingSettingsPayload.data = carrierAccountEntity.CustomerSettings.PricingSettings;
                }
                VRUIUtilsService.callDirectiveLoad(pricingSettingsEditorAPI, pricingSettingsPayload, pricingSettingsEditorLoadDeferred);
            });
            return pricingSettingsEditorLoadDeferred.promise;
        }


        function loadSupplierSettingsTab() {
            return UtilsService.waitMultipleAsyncOperations([loadSupplierTimeZoneSelector, loadZoneServiceConfigSelector, loadSupplierRoutingStatusSelector, loadPricingSettingsTab]);
        }
        function loadSupplierTimeZoneSelector() {
            var supplierTimeZoneSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            supplierTimeZoneSelectorReadyDeferred.promise.then(function () {
                supplierTimeZoneSelectorReadyDeferred = undefined;
                var supplierTimeZoneSelectorPayload = {
                    selectedIds: (carrierAccountEntity != undefined) ? carrierAccountEntity.SupplierSettings.TimeZoneId : undefined
                };
                VRUIUtilsService.callDirectiveLoad(supplierTimeZoneSelectorAPI, supplierTimeZoneSelectorPayload, supplierTimeZoneSelectorLoadDeferred);
            });

            return supplierTimeZoneSelectorLoadDeferred.promise;
        }
        function loadZoneServiceConfigSelector() {
            var zoneServiceConfigSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            zoneServiceConfigSelectorReadyDeferred.promise.then(function () {
                zoneServiceConfigSelectorReadyDeferred = undefined;
                var zoneServiceConfigSelectorPayload = {
                    selectedIds: getDefaultServices(),
                    selectminweight: (!isEditMode) ? true : false
                };
                VRUIUtilsService.callDirectiveLoad(zoneServiceConfigSelectorAPI, zoneServiceConfigSelectorPayload, zoneServiceConfigSelectorLoadDeferred);
            });

            return zoneServiceConfigSelectorLoadDeferred.promise;
        }
        function loadSupplierRoutingStatusSelector() {
            var supplierRoutingStatusSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            supplierRoutingStatusSelectorReadyDeferred.promise.then(function () {
                supplierRoutingStatusSelectorReadyDeferred = undefined;
                var supplierRoutingStatusSelectorPayload = {
                    selectedIds: (carrierAccountEntity != undefined) ? carrierAccountEntity.SupplierSettings.RoutingStatus : WhS_BE_RoutingStatusEnum.Enabled.value
                };
                VRUIUtilsService.callDirectiveLoad(supplierRoutingStatusSelectorAPI, supplierRoutingStatusSelectorPayload, supplierRoutingStatusSelectorLoadDeferred);
            });

            return supplierRoutingStatusSelectorLoadDeferred.promise;
        }

        function getDefaultServices() {
            if (carrierAccountEntity != undefined && carrierAccountEntity.SupplierSettings != null && carrierAccountEntity.SupplierSettings.DefaultServices != null) {
                var defaultServices = [];

                for (var i = 0 ; i < carrierAccountEntity.SupplierSettings.DefaultServices.length ; i++)
                    defaultServices.push(carrierAccountEntity.SupplierSettings.DefaultServices[i].ServiceId);

                return defaultServices;
            }
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

        function buildCarrierAccountObjFromScope() {
            var obj = {
                CarrierAccountId: (carrierAccountId != null) ? carrierAccountId : 0,
                NameSuffix: $scope.scopeModel.name,
                SellingProductId: (isCustomer()) ? sellingProductSelectorAPI.getSelectedIds() : null,
                CarrierAccountSettings: {
                    ActivationStatus: activationStatusSelectorAPI.getSelectedIds(),
                    CurrencyId: currencySelectorAPI.getSelectedIds(),
                    Mask: $scope.scopeModel.mask,
                    CompanySettingId: companySettingsSelectorAPI.getSelectedIds(),
                    NominalCapacity: $scope.scopeModel.nominalCapacity,
                    IsInterconnectSwitch: $scope.scopeModel.isInterconnectSwitch,
                    PriceListSettings: {
                        Email: $scope.scopeModel.automaticPriceListEmail,
                        FileMask: $scope.scopeModel.fileMask,
                        SubjectCode: $scope.scopeModel.automaticPriceListSubjectCode,
                        BPBusinessRuleSetIds: bpBusinessRuleSetDirectiveAPI.getSelectedIds()
                    }
                },
                SupplierSettings: {
                    DefaultServices: zoneServiceConfigSelectorAPI != undefined ? getSelectedDefaultServices() : null,
                    TimeZoneId: supplierTimeZoneSelectorAPI != undefined ? supplierTimeZoneSelectorAPI.getSelectedIds() : undefined,
                    RoutingStatus: supplierRoutingStatusSelectorAPI != undefined ? supplierRoutingStatusSelectorAPI.getSelectedIds() : undefined,
                    EffectiveDateDayOffset: $scope.scopeModel.supplierEffectiveDateDayOffset
                },
                CustomerSettings: {
                    TimeZoneId: customerTimeZoneSelectorAPI != undefined ? customerTimeZoneSelectorAPI.getSelectedIds() : undefined,
                    RoutingStatus: customerRoutingStatusSelectorAPI != undefined ? customerRoutingStatusSelectorAPI.getSelectedIds() : undefined,
                    InvoiceTimeZone: $scope.scopeModel.customerInvoiceTimeZone,
                    PricelistSettings: priceListSettingsEditorAPI != undefined ? priceListSettingsEditorAPI.getData() : undefined,
                    PricingSettings: pricingSettingsEditorAPI != undefined ? pricingSettingsEditorAPI.getData() : undefined,
                }
            };
            if (!isEditMode) {
                obj.CarrierProfileId = carrierProfileSelectorAPI.getSelectedIds();
                obj.SellingNumberPlanId = sellingNumberPlanSelectorAPI != undefined ? sellingNumberPlanSelectorAPI.getSelectedIds() : null;
                obj.AccountType = $scope.scopeModel.selectedCarrierAccountType.value;
            }

            return obj;
        }

        function isCustomer() {
            return ($scope.scopeModel.selectedCarrierAccountType == WhS_BE_CarrierAccountTypeEnum.Customer || $scope.scopeModel.selectedCarrierAccountType == WhS_BE_CarrierAccountTypeEnum.Exchange);
        }

        function isSupplier() {
            return ($scope.scopeModel.selectedCarrierAccountType == WhS_BE_CarrierAccountTypeEnum.Supplier || $scope.scopeModel.selectedCarrierAccountType == WhS_BE_CarrierAccountTypeEnum.Exchange);
        }
    }

    appControllers.controller('WhS_BE_CarrierAccountEditorController', carrierAccountEditorController);
})(appControllers);