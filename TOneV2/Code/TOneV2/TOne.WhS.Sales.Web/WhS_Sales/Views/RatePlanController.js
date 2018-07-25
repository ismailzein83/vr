(function (appControllers) {

    "use strict";

    RatePlanController.$inject = ["$window", "$scope", "WhS_Sales_RatePlanService", "WhS_Sales_RatePlanAPIService", 'WhS_Sales_RatePlanConfigAPIService', "WhS_BE_SalePriceListOwnerTypeEnum", "WhS_Sales_RatePlanStatusEnum", 'BusinessProcess_BPInstanceAPIService', 'BusinessProcess_BPInstanceService', 'WhS_BP_CreateProcessResultEnum', 'VRCommon_CurrencyAPIService', 'WhS_BE_CarrierAccountAPIService', 'BPInstanceStatusEnum', "UtilsService", "VRUIUtilsService", "VRNotificationService", "WhS_BP_RatePlanDefinitionEnum", "WhS_BE_SellingProductAPIService", "VRDateTimeService", "VRCommon_VRExclusiveSessionTypeService", "VRCommon_VRExclusiveSessionTypeAPIService", "WhS_BE_ExclusiveSessionTypeIdEnum", "WhS_BE_ExclusiveSessionTargetIdPrefixEnum"];

    function RatePlanController($window, $scope, WhS_Sales_RatePlanService, WhS_Sales_RatePlanAPIService, WhS_Sales_RatePlanConfigAPIService, WhS_BE_SalePriceListOwnerTypeEnum, WhS_Sales_RatePlanStatusEnum, BusinessProcess_BPInstanceAPIService, BusinessProcess_BPInstanceService, WhS_BP_CreateProcessResultEnum, VRCommon_CurrencyAPIService, WhS_BE_CarrierAccountAPIService, BPInstanceStatusEnum, UtilsService, VRUIUtilsService, VRNotificationService, WhS_BP_RatePlanDefinitionEnum, WhS_BE_SellingProductAPIService, VRDateTimeService, VRCommon_VRExclusiveSessionTypeService, VRCommon_VRExclusiveSessionTypeAPIService, WhS_BE_ExclusiveSessionTypeIdEnum, WhS_BE_ExclusiveSessionTargetIdPrefixEnum) {

        var ownerTypeSelectorAPI;
        var ownerTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var ownerPricingSettings;

        var sellingProductSelectorAPI;
        var sellingProductSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var carrierAccountSelectorAPI;
        var carrierAccountSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var databaseSelectorAPI;
        var databaseSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var policySelectorAPI;

        var customerSellingProductId;

        var currencySelectorAPI;
        var currencySelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var sellingProductCurrencyId;
        var draftCurrencyId;
        var defaultCustomerCurrencyId;
        var longPrecision;

        var countrySelectorAPI;
        var countrySelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var textFilterAPI;
        var textFilterReadyDeferred = UtilsService.createPromiseDeferred();

        var defaultItem;
        var countryChanges;

        var gridAPI;
        var gridReadyDeferred = UtilsService.createPromiseDeferred();
        var getSellingProductCurrencyIdPromise;
        var settings = {};
        var pricingSettings;
        var ratePlanSettingsData;
        var saleAreaSettingsData;
        var isRatePlanloadedFromDrafts = false;

        var subscriberOwnerEntities;
        var subscriberOwnerIds;

        var exclusiveSessionObject = null;
        defineScope();
        load();

        function defineScope() {
            /* These vars are reversed with every onOwnerTypeChanged. Therefore, the customer selector will show when the event first occurs */
            $scope.showSellingProductSelector = true;
            $scope.showCarrierAccountSelector = false;
            //  $scope.showBulkActionButton = false;
            /* ***** */

            $scope.onOwnerTypeSelectorReady = function (api) {
                ownerTypeSelectorAPI = api;
                ownerTypeSelectorReadyDeferred.resolve();
            };
            $scope.onOwnerTypeChanged = function (selectedOwnerType) {

                releaseSession();
                clearOwnerInfo();
                resetRatePlan();
                showActionButtons(false);

                draftCurrencyId = undefined;
                ownerPricingSettings = undefined;

                var selectedId = ownerTypeSelectorAPI.getSelectedIds();

                if (selectedId == undefined)
                    return;

                $scope.showSellingProductSelector = !$scope.showSellingProductSelector;
                $scope.showCarrierAccountSelector = !$scope.showCarrierAccountSelector;

                if (selectedId == WhS_BE_SalePriceListOwnerTypeEnum.SellingProduct.value) {
                    $scope.selectedCustomer = undefined;
                    $scope.showSaveButtonForCustomer = false;
                    $scope.showSaveButtonForSellingProduct = true;
                }
                else if (selectedId == WhS_BE_SalePriceListOwnerTypeEnum.Customer.value) {
                    $scope.selectedSellingProduct = undefined;
                    $scope.showSaveButtonForCustomer = true;
                    $scope.showSaveButtonForSellingProduct = false;
                }
            };

            $scope.onSellingProductSelectorReady = function (api) {
                sellingProductSelectorAPI = api;
                sellingProductSelectorReadyDeferred.resolve();
            };
            $scope.onSellingProductChanged = function (selectedSellingProduct) {
                releaseSession();
                resetRatePlan();
                showActionButtons(false);
                if (selectedSellingProduct == undefined)
                    return;

                $scope.isLoadingFilterSection = true;

                var promises = [];

                var getEntityIdsPromise = getEntityIds(WhS_BE_SalePriceListOwnerTypeEnum.SellingProduct.value, selectedSellingProduct.SellingProductId);
                promises.push(getEntityIdsPromise);

                var doRunningProcessesExistDeferred = UtilsService.createPromiseDeferred();
                promises.push(doRunningProcessesExistDeferred.promise);

                var getSellingProductCurrencyIdDeferred = UtilsService.createPromiseDeferred();
                promises.push(getSellingProductCurrencyIdDeferred.promise);

                var loadOwnerInfoDeferred = UtilsService.createPromiseDeferred();
                promises.push(loadOwnerInfoDeferred.promise);

                var loadOwnerPricingSettingsDeferred = UtilsService.createPromiseDeferred();
                promises.push(loadOwnerPricingSettingsDeferred.promise);

                getEntityIdsPromise.then(function (entityIds) {
                    hasRunningProcessesForCustomerOrSellingProduct(entityIds, selectedSellingProduct.SellingProductId, WhS_BE_SalePriceListOwnerTypeEnum.SellingProduct.value).then(function (response) {
                        doRunningProcessesExistDeferred.resolve();
                        var targetId = WhS_BE_ExclusiveSessionTargetIdPrefixEnum.SellingProduct.value + selectedSellingProduct.SellingProductId;
                        tryTakeSession(targetId).then(function (response) {
                            if (response.IsSucceeded) {
                                getSellingProductCurrencyId(selectedSellingProduct.SellingProductId).then(function () { getSellingProductCurrencyIdDeferred.resolve(); }).catch(function (error) { getSellingProductCurrencyIdDeferred.reject(error); });
                                loadOwnerInfo().then(function () { loadOwnerInfoDeferred.resolve(); }).catch(function (error) { loadOwnerInfoDeferred.reject(error); });
                                getOwnerPricingSettings().then(function () { loadOwnerPricingSettingsDeferred.resolve(); }).catch(function (error) { loadOwnerPricingSettingsDeferred.reject(error); });
                            }
                            else {
                                getSellingProductCurrencyIdDeferred.resolve();
                                loadOwnerInfoDeferred.resolve();
                                loadOwnerPricingSettingsDeferred.resolve();
                                onTryTakeFailure(response).then(function () { $scope.isLoadingFilterSection = false; });
                            }
                        });
                    }).catch(function (error) {
                        doRunningProcessesExistDeferred.reject(error);
                    });
                });

                UtilsService.waitMultiplePromises(promises).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                }).finally(function () {
                    $scope.isLoadingFilterSection = false;
                });
            };

            $scope.onCarrierAccountSelectorReady = function (api) {
                carrierAccountSelectorAPI = api;
                carrierAccountSelectorReadyDeferred.resolve();
            };
            $scope.onCarrierAccountChanged = function (selectedCustomer) {
                releaseSession();
                resetRatePlan();
                showActionButtons(false);

                draftCurrencyId = undefined;

                var selectedCustomerId = carrierAccountSelectorAPI.getSelectedIds();

                if (selectedCustomerId == undefined)
                    return;

                $scope.isLoadingFilterSection = true;

                var promises = [];

                var getEntityIdsPromise = getEntityIds(WhS_BE_SalePriceListOwnerTypeEnum.Customer.value, selectedCustomerId);
                promises.push(getEntityIdsPromise);

                var doRunningProcessesExistDeferred = UtilsService.createPromiseDeferred();
                promises.push(doRunningProcessesExistDeferred.promise);

                var onCustomerChangedDeferred = UtilsService.createPromiseDeferred();
                promises.push(onCustomerChangedDeferred.promise);

                getEntityIdsPromise.then(function (entityIds) {
                    hasRunningProcessesForCustomerOrSellingProduct(entityIds, selectedCustomerId, WhS_BE_SalePriceListOwnerTypeEnum.Customer.value).then(function (response) {
                        var targetId = WhS_BE_ExclusiveSessionTargetIdPrefixEnum.Customer.value + selectedCustomerId;
                        tryTakeSession(targetId).then(function (response) {
                            if (response.IsSucceeded) {
                                doRunningProcessesExistDeferred.resolve();
                                onCustomerChanged(selectedCustomerId).then(function () { onCustomerChangedDeferred.resolve(); }).catch(function (error) { onCustomerChangedDeferred.reject(error); });
                            }
                            else
                                onTryTakeFailure(response).then(function () { $scope.isLoadingFilterSection = false; });
                        });
                    }).catch(function (error) {
                        doRunningProcessesExistDeferred.reject(error);
                    });
                });

                UtilsService.waitMultiplePromises(promises).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                }).finally(function () {
                    $scope.isLoadingFilterSection = false;
                });
            };

            $scope.numberOfOptions = 3;

            $scope.onDatabaseSelectorReady = function (api) {
                databaseSelectorAPI = api;
                databaseSelectorReadyDeferred.resolve();
            };

            $scope.onRoutingDatabaseChanged = function () {
                draftCurrencyId = undefined;

                var selectedId = databaseSelectorAPI.getSelectedIds();

                if (selectedId == undefined)
                    return;

                var policySelectorPayload = {
                    filter: {
                        RoutingDatabaseId: selectedId
                    },
                    selectDefaultPolicy: true
                };

                var setLoader = function (value) {
                    $scope.isLoadingFilterSection = value;
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, policySelectorAPI, policySelectorPayload, setLoader, undefined);
            };

            $scope.onPolicySelectorReady = function (api) {
                policySelectorAPI = api;
            };

            $scope.onCurrencySelectorReady = function (api) {
                currencySelectorAPI = api;
                currencySelectorReadyDeferred.resolve();
            };

            $scope.onCurrencyChanged = function (selectedCurrency) {

                if (draftCurrencyId == undefined)
                    return;

                if (selectedCurrency == undefined)
                    return;

                if (selectedCurrency.CurrencyId == draftCurrencyId)
                    return;

                VRNotificationService.showConfirmation('Changing the currency will reset all new rates. Are you sure that you want to proceed?').then(function (isConfirmed) {
                    if (isConfirmed) {
                        var promises = [];

                        var saveChangesPromise = saveDraft(false);
                        promises.push(saveChangesPromise);

                        var defineNewRatesConvertedToCurrencyDeferred = UtilsService.createPromiseDeferred();
                        promises.push(defineNewRatesConvertedToCurrencyDeferred.promise);

                        var doesOwnerDraftExistDeferred = UtilsService.createPromiseDeferred();
                        promises.push(doesOwnerDraftExistDeferred.promise);

                        saveChangesPromise.then(function () {
                            var defineNewRatesInput = {
                                CustomerId: getOwnerId(),
                                NewCurrencyId: selectedCurrency.CurrencyId,
                                EffectiveOn: UtilsService.getDateFromDateTime(VRDateTimeService.getNowDateTime()),
                                NewCountryIds: null
                            };
                            WhS_Sales_RatePlanAPIService.DefineNewRatesConvertedToCurrency(defineNewRatesInput).then(function () {
                                draftCurrencyId = selectedCurrency.CurrencyId;
                                defineNewRatesConvertedToCurrencyDeferred.resolve();
                            }).catch(function (error) {
                                defineNewRatesConvertedToCurrencyDeferred.reject(error);
                            });
                        });

                        defineNewRatesConvertedToCurrencyDeferred.promise.then(function () {
                            doesOwnerDraftExist().then(function () {
                                doesOwnerDraftExistDeferred.resolve();
                            }).catch(function (error) {
                                doesOwnerDraftExistDeferred.reject(error);
                            });
                        });

                        UtilsService.waitMultiplePromises(promises).then(function () {
                            loadGrid();
                        });
                    }
                    else { currencySelectorAPI.selectedCurrency(draftCurrencyId); }
                });
            };

            $scope.onCountrySelectorReady = function (api) {
                countrySelectorAPI = api;
                countrySelectorReadyDeferred.resolve();
            };

            $scope.onTextFilterReady = function (api) {
                textFilterAPI = api;
                textFilterReadyDeferred.resolve();
            };

            $scope.defaultItemTabs = [{
                title: "Default Routing Product",
                directive: "vr-whs-sales-routingproduct-default",
                loadDirective: function (api) {
                    var defaultPayload = {};
                    defaultPayload.defaultItem = defaultItem;
                    defaultPayload.context = {};
                    defaultPayload.context.saveDraft = saveDraft;
                    return api.load(defaultPayload);
                }
            }];

            $scope.zoneLetters = [];
            $scope.connector = {};
            $scope.connector.selectedZoneLetterIndex = 0;

            $scope.onZoneLetterSelectionChanged = function () {
                return saveDraft(true);
            };

            $scope.onGridReady = function (api) {
                gridAPI = api;
                gridReadyDeferred.resolve();
            };

            $scope.load = function () {
                return loadRatePlan();
            };
            $scope.manageCountries = manageCountries;
            $scope.editSettings = function () {
                var onSettingsUpdated = function (updatedSettings) {
                    if (updatedSettings == undefined)
                        settings = undefined;
                    else {
                        settings = {};
                        if (updatedSettings.costCalculationMethods != undefined) {
                            settings.costCalculationMethods = [];
                            for (var i = 0; i < updatedSettings.costCalculationMethods.length; i++) {
                                settings.costCalculationMethods.push(updatedSettings.costCalculationMethods[i]);
                            }
                        }
                    }

                    pricingSettings = null;
                    $scope.showApplyButton = false;

                    VRNotificationService.showSuccess("Settings saved");

                    if (isRoutingInfoDefined()) {
                        var promises = [];

                        var saveChangesPromise = saveDraft(false);
                        promises.push(saveChangesPromise);

                        var loadGridDeferred = UtilsService.createPromiseDeferred();
                        promises.push(loadGridDeferred.promise);

                        saveChangesPromise.then(function () {
                            loadGrid().then(function () {
                                loadGridDeferred.resolve();
                            }).catch(function (error) {
                                loadGridDeferred.reject(error);
                            });
                        });

                        UtilsService.waitMultiplePromises(promises);
                    }
                };
                WhS_Sales_RatePlanService.editSettings(settings, onSettingsUpdated);
            };
            $scope.editPricingSettings = function () {
                var onPricingSettingsUpdated = function (updatedPricingSettings) {
                    pricingSettings = updatedPricingSettings;

                    $scope.showApplyButton = true;
                    VRNotificationService.showSuccess("Pricing settings saved");

                    var promises = [];

                    var saveChangesPromise = saveDraft(false);
                    promises.push(saveChangesPromise);

                    var loadGridDeferred = UtilsService.createPromiseDeferred();
                    promises.push(loadGridDeferred.promise);

                    saveChangesPromise.then(function () {
                        loadGrid().then(function () {
                            loadGridDeferred.resolve();
                        }).catch(function (error) {
                            loadGridDeferred.reject(error);
                        });
                    });

                    UtilsService.waitMultiplePromises(promises);
                };
                WhS_Sales_RatePlanService.editPricingSettings(settings, pricingSettings, onPricingSettingsUpdated);
            };
            $scope.applyCalculatedRates = function () {
                var promises = [];

                var confirmPromise = VRNotificationService.showConfirmation("Are you sure you want to apply the calculated rates?");
                promises.push(confirmPromise);

                var saveChangesDeferred = UtilsService.createPromiseDeferred();
                promises.push(saveChangesDeferred.promise);

                var applyDeferred = UtilsService.createPromiseDeferred();
                promises.push(applyDeferred.promise);

                var calculatedRates;

                confirmPromise.then(function (confirmed) {
                    if (confirmed) {
                        saveDraft(false).then(function () {
                            saveChangesDeferred.resolve();
                            var tryApplyInput = getTryApplyCalculatedRatesInput();
                            WhS_Sales_RatePlanAPIService.TryApplyCalculatedRates(tryApplyInput).then(function (response) {
                                calculatedRates = response;
                                applyDeferred.resolve();
                            }).catch(function (error) {
                                applyDeferred.reject(error);
                            });
                        }).catch(function (error) {
                            saveChangesDeferred.reject(error);
                        });

                        UtilsService.waitMultiplePromises([saveChangesDeferred.promise, applyDeferred.promise]).then(function () {
                            if (calculatedRates == undefined || calculatedRates == null)
                                onRatesApplied();
                            else {
                                var onSaved = function (validCalculatedRates) {
                                    var applyInput = getApplyCalculatedRatesInput(validCalculatedRates);
                                    WhS_Sales_RatePlanAPIService.ApplyCalculatedRates(applyInput).then(function () {
                                        onRatesApplied();
                                    }).catch(function (error) {
                                        VRNotificationService.notifyException(error, $scope);
                                    });
                                };
                                WhS_Sales_RatePlanService.viewInvalidRates(calculatedRates, onSaved);
                            }
                        });
                    }
                    else {
                        saveChangesDeferred.resolve();
                        applyDeferred.resolve();
                    }
                });

                function getTryApplyCalculatedRatesInput() {
                    var input = {
                        OwnerType: ownerTypeSelectorAPI.getSelectedIds(),
                        OwnerId: getOwnerId(),
                        EffectiveOn: VRDateTimeService.getNowDateTime(),
                        RoutingDatabaseId: databaseSelectorAPI ? databaseSelectorAPI.getSelectedIds() : null,
                        PolicyConfigId: policySelectorAPI ? policySelectorAPI.getSelectedIds() : null,
                        NumberOfOptions: $scope.numberOfOptions,
                        CostCalculationMethods: settings ? settings.costCalculationMethods : null,
                        CurrencyId: getCurrencyId(),
                        CountryIds: countrySelectorAPI.getSelectedIds()
                    };

                    if (pricingSettings != undefined) {
                        input.RateCalculationMethod = pricingSettings.selectedRateCalculationMethodData;
                        if (pricingSettings.selectedCostColumn != undefined)
                            input.SelectedCostCalculationMethodConfigId = pricingSettings.selectedCostColumn.ConfigId;
                    }

                    var textFilterData = textFilterAPI.getData();
                    if (textFilterData != undefined) {
                        input.ZoneNameFilterType = textFilterData.TextFilterType;
                        input.ZoneNameFilter = textFilterData.Text;
                    }

                    return input;
                }
                function getApplyCalculatedRatesInput(validCalculatedRates) {
                    return {
                        OwnerType: ownerTypeSelectorAPI.getSelectedIds(),
                        OwnerId: getOwnerId(),
                        CalculatedRates: validCalculatedRates,
                        EffectiveOn: VRDateTimeService.getNowDateTime(),
                        CurrencyId: getCurrencyId()
                    };
                }
                function onRatesApplied() {
                    VRNotificationService.showSuccess("Rates applied");
                    pricingSettings = null;
                    $scope.showApplyButton = false;
                    $scope.showCancelButton = true;

                    loadGrid().catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
                }

                return UtilsService.waitMultiplePromises(promises);
            };
            $scope.deleteDraft = function () {
                var promises = [];

                var confirmationPromise = VRNotificationService.showConfirmation("Are you sure that you want to delete the entire draft?");
                promises.push(confirmationPromise);

                var deletionDeferred = UtilsService.createPromiseDeferred();
                promises.push(deletionDeferred.promise);

                confirmationPromise.then(function (isConfirmed) {
                    if (!isConfirmed)
                        deletionDeferred.resolve();
                    else {
                        var deletionPromises = [];

                        $scope.isLoading = true;
                        $scope.isLoadingFilterSection = true;

                        var deleteDraftPromise = WhS_Sales_RatePlanAPIService.DeleteDraft(ownerTypeSelectorAPI.getSelectedIds(), getOwnerId());
                        deletionPromises.push(deleteDraftPromise);

                        var loadOwnerInfoDeferred = UtilsService.createPromiseDeferred();
                        deletionPromises.push(loadOwnerInfoDeferred.promise);

                        deleteDraftPromise.then(function () {
                            VRNotificationService.showSuccess("Draft deleted");

                            subscriberOwnerEntities = undefined;
                            subscriberOwnerIds = undefined;
                            countryChanges = undefined;
                            draftCurrencyId = undefined;
                            if (gridAPI != undefined)
                                gridAPI.clearDataSource();
                            $scope.showCancelButton = false;
                            currencySelectorAPI.selectedCurrency(defaultCustomerCurrencyId);

                            if (isRoutingInfoDefined()) {
                                isRatePlanloadedFromDrafts = true;
                                loadRatePlan();
                            }

                            loadOwnerInfo().then(function () {
                                loadOwnerInfoDeferred.resolve();
                            }).catch(function (error) {
                                loadOwnerInfoDeferred.reject(error);
                            });
                        });

                        UtilsService.waitMultiplePromises(deletionPromises).then(function () {
                            deletionDeferred.resolve();
                        }).catch(function (error) {
                            deletionDeferred.reject(error);
                        }).finally(function () {
                            $scope.isLoading = false;
                            $scope.isLoadingFilterSection = false;
                        });
                    }
                });

                return UtilsService.waitMultiplePromises(promises);
            };
            $scope.openBulkActionWizard = function () {
                var onBulkActionAppliedToDraft = function () {
                    $scope.showCancelButton = true;

                    var promises = [];

                    $scope.isLoading = true;
                    $scope.isLoadingFilterSection = true;

                    var loadOwnerInfoPromise = loadOwnerInfo();
                    promises.push(loadOwnerInfoPromise);


                    var ownerType = ownerTypeSelectorAPI.getSelectedIds();
                    var ownerId = getOwnerId();
                    if (ownerType == WhS_BE_SalePriceListOwnerTypeEnum.Customer.value) {
                        promises.push(getCountryChanges(ownerId));
                    }

                    if (isRoutingInfoDefined()) {
                        var loadRatePlanPromise = loadRatePlan();
                        promises.push(loadRatePlanPromise);
                    }

                    UtilsService.waitMultiplePromises(promises).catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    }).finally(function () {
                        $scope.isLoading = false;
                        $scope.isLoadingFilterSection = false;
                    });
                };

                var openBulkActionWizardInput = {
                    ownerType: ownerTypeSelectorAPI.getSelectedIds(),
                    ownerId: getOwnerId(),
                    ownerSellingNumberPlanId: getOwnerSellingNumberPlanId(),
                    gridQuery: getGridQuery(false),
                    routingDatabaseId: databaseSelectorAPI.getSelectedIds(),
                    policyConfigId: policySelectorAPI.getSelectedIds(),
                    numberOfOptions: $scope.numberOfOptions,
                    costCalculationMethods: getCostCalculationMethods(),
                    currencyId: getCurrencyId(),
                    longPrecision: longPrecision,
                    onBulkActionAppliedToDraft: onBulkActionAppliedToDraft,
                    pricingSettings: ownerPricingSettings
                };

                saveDraft().catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                }).finally(function () {
                    WhS_Sales_RatePlanService.openBulkActionWizard(openBulkActionWizardInput);
                });
            };
            $scope.importRatePlan = function () {
                var ownerType = ownerTypeSelectorAPI.getSelectedIds();
                var ownerId = getOwnerId();
                var onRatePlanImported = function () {
                    loadRatePlan();
                };
                WhS_Sales_RatePlanService.importRatePlan(ownerType, ownerId, onRatePlanImported);
            };

            $window.onbeforeunload = function () {
                releaseSession();
            };
            defineCustomerApplyButtonMenuActions();
            defineSellingProductApplyButtonMenuActions();
        }
        function load() {
            $scope.isLoadingFilterSection = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadOwnerFilterSection, loadRouteOptionsFilterSection, loadCurrencySelector, loadRatePlanSettingsData, loadSaleAreaSettingsData, getLongPrecisionValue, loadCountrySelector, loadTextFilter]).then(function () {
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isLoadingFilterSection = false;
            });
        }
        function loadOwnerFilterSection() {
            var promises = [];

            var ownerTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            promises.push(ownerTypeSelectorLoadDeferred.promise);

            ownerTypeSelectorReadyDeferred.promise.then(function () {
                var ownerTypeSelectorPayload = { selectedIds: WhS_BE_SalePriceListOwnerTypeEnum.Customer.value };
                VRUIUtilsService.callDirectiveLoad(ownerTypeSelectorAPI, ownerTypeSelectorPayload, ownerTypeSelectorLoadDeferred);
            });

            var sellingProductSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            promises.push(sellingProductSelectorLoadDeferred.promise);

            sellingProductSelectorReadyDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(sellingProductSelectorAPI, undefined, sellingProductSelectorLoadDeferred);
            });

            var carrierAccountSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            promises.push(carrierAccountSelectorLoadDeferred.promise);

            carrierAccountSelectorReadyDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(carrierAccountSelectorAPI, undefined, carrierAccountSelectorLoadDeferred);
            });

            return UtilsService.waitMultiplePromises(promises);
        }
        function loadRouteOptionsFilterSection() {
            var promises = [];

            var databaseSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            promises.push(databaseSelectorLoadDeferred.promise);

            databaseSelectorReadyDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(databaseSelectorAPI, undefined, databaseSelectorLoadDeferred);
            });

            return UtilsService.waitMultiplePromises(promises);
        }
        function loadCurrencySelector() {
            var currencySelectorLoadDeferred = UtilsService.createPromiseDeferred();

            currencySelectorReadyDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(currencySelectorAPI, undefined, currencySelectorLoadDeferred);
            });

            return currencySelectorLoadDeferred.promise;
        }
        function loadRatePlanSettingsData() {
            return WhS_Sales_RatePlanAPIService.GetRatePlanSettingsData().then(function (response) {
                ratePlanSettingsData = response;
                if (response != undefined && response.CostCalculationsMethods != null) {
                    settings.costCalculationMethods = [];
                    for (var i = 0; i < response.CostCalculationsMethods.length; i++) {
                        settings.costCalculationMethods.push(response.CostCalculationsMethods[i]);
                    }
                }
            });
        }
        function loadSaleAreaSettingsData() {
            return WhS_Sales_RatePlanAPIService.GetSaleAreaSettingsData().then(function (response) {
                saleAreaSettingsData = response;
            });
        }

        function getLongPrecisionValue() {
            return WhS_Sales_RatePlanConfigAPIService.GetGeneralSettingsLongPrecisionValue().then(function (response) {
                longPrecision = response;
            });
        }
        function loadCountrySelector() {
            var countrySelectorLoadDeferred = UtilsService.createPromiseDeferred();

            countrySelectorReadyDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(countrySelectorAPI, undefined, countrySelectorLoadDeferred);
            });

            return countrySelectorLoadDeferred.promise;
        }
        function loadTextFilter() {
            var textFilterLoadDeferred = UtilsService.createPromiseDeferred();

            textFilterReadyDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(textFilterAPI, undefined, textFilterLoadDeferred);
            });

            return textFilterLoadDeferred.promise;
        }
        function getOwnerPricingSettings() {
            var ownerType = ownerTypeSelectorAPI.getSelectedIds();
            var ownerId = getOwnerId();
            var pricingSettingsLoadPromise = (ownerType == WhS_BE_SalePriceListOwnerTypeEnum.SellingProduct.value) ?
				WhS_BE_SellingProductAPIService.GetSellingProductPricingSettings(ownerId) : WhS_BE_CarrierAccountAPIService.GetCustomerPricingSettings(ownerId);
            pricingSettingsLoadPromise.then(function (response) {
                ownerPricingSettings = response;
            });
            return pricingSettingsLoadPromise;
        }

        function loadRatePlan() {


            var promises = [];

            var ownerTypeValue = ownerTypeSelectorAPI.getSelectedIds();
            var ownerId = getOwnerId();

            var doesOwnerDraftExistPromise = doesOwnerDraftExist();
            promises.push(doesOwnerDraftExistPromise);

            if (ownerTypeValue == WhS_BE_SalePriceListOwnerTypeEnum.Customer.value) {
                var getDraftCurrencyIdPromise = WhS_Sales_RatePlanAPIService.GetDraftCurrencyId(WhS_BE_SalePriceListOwnerTypeEnum.Customer.value, ownerId);
                promises.push(getDraftCurrencyIdPromise);
                getDraftCurrencyIdPromise.then(function (response) {
                    if (response != undefined) {
                        draftCurrencyId = response;
                        currencySelectorAPI.selectedCurrency(response);
                    }
                    if (draftCurrencyId == undefined)
                        draftCurrencyId = defaultCustomerCurrencyId;
                });
            }

            var loadGridDeferred = UtilsService.createPromiseDeferred();
            promises.push(loadGridDeferred.promise);

            UtilsService.convertToPromiseIfUndefined(getDraftCurrencyIdPromise).then(function () {
                var gridQuery = getGridQuery(true);
                gridAPI.load(gridQuery).then(function () {
                    loadGridDeferred.resolve();
                    var gridHasCustomerData = gridAPI.doesGridHasCustomerData();
                    if (!gridHasCustomerData && ownerTypeValue == WhS_BE_SalePriceListOwnerTypeEnum.Customer.value) {
                        if (!isRatePlanloadedFromDrafts)
                            manageCountries();
                        showActionButtons(gridHasCustomerData);
                        $scope.showBulkActionButton = true;
                    }
                    isRatePlanloadedFromDrafts = false;
                }).catch(function (error) {
                    loadGridDeferred.reject(error);
                });
            });

            return UtilsService.waitMultiplePromises(promises);
        }
        function loadGrid() {
            var gridQuery = getGridQuery();
            return gridAPI.load(gridQuery);
        }
        function getGridQuery(shouldSetFilter) {

            var query = {
                OwnerType: ownerTypeSelectorAPI.getSelectedIds(),
                OwnerId: getOwnerId(),
                ownerSellingNumberPlanId: getOwnerSellingNumberPlanId(),
                CurrencyId: getCurrencyId(),
                RoutingDatabaseId: databaseSelectorAPI.getSelectedIds(),
                PolicyConfigId: policySelectorAPI.getSelectedIds(),
                NumberOfOptions: $scope.numberOfOptions,
                CostCalculationMethods: getCostCalculationMethods(),
                BulkAction: null,
                EffectiveOn: UtilsService.getDateFromDateTime(VRDateTimeService.getNowDateTime()),
                OwnerName: getOwnerName(),
                Settings: ratePlanSettingsData,
                SaleAreaSettings: saleAreaSettingsData,
                longPrecision: longPrecision,
                countryChangesEED: (countryChanges != undefined && countryChanges.ChangedCountries != undefined) ? countryChanges.ChangedCountries.EED : undefined
            };

            if (shouldSetFilter === true) {

                query.Filter = {};
                query.Filter.CountryIds = countrySelectorAPI.getSelectedIds();
                query.Filter.BulkActionFilter = null;

                var textFilterData = textFilterAPI.getData();
                if (textFilterData != undefined) {
                    query.Filter.ZoneNameFilter = textFilterData.Text;
                    query.Filter.ZoneNameFilterType = textFilterData.TextFilterType;
                }
            }

            setGridQueryContext();

            function setGridQueryContext() {
                query.context = {};
                query.context.saveDraft = saveDraft;
                query.context.onZoneLettersLoaded = function () {
                    $scope.showSaveButton = true;
                    $scope.showSettingsButton = true;
                    $scope.showPricingButton = true;
                    $scope.showBulkActionButton = true;
                    showRatePlan(true);
                };
                query.context.showRatePlan = showRatePlan;
                query.context.isFilterApplied = isFilterApplied;
            }
            return query;
        }

        function getCostCalculationMethods() {
            var cosCalculationMethods = [];

            if (settings != undefined && settings.costCalculationMethods != undefined) {
                for (var i = 0; i < settings.costCalculationMethods.length; i++)
                    cosCalculationMethods.push(settings.costCalculationMethods[i]);
            }

            return cosCalculationMethods;
        }

        function saveDraft(shouldLoadGrid) {
            var promises = [];

            var saveDraftDeferred = UtilsService.createPromiseDeferred();
            promises.push(saveDraftDeferred.promise);

            var loadGridDeferred = UtilsService.createPromiseDeferred();
            promises.push(loadGridDeferred.promise);

            var newDraft = getNewDraft();

            if (newDraft == undefined)
                saveDraftDeferred.resolve();
            else {
                var parameters = {
                    OwnerType: ownerTypeSelectorAPI.getSelectedIds(),
                    OwnerId: getOwnerId(),
                    NewChanges: newDraft
                };
                WhS_Sales_RatePlanAPIService.SaveChanges(parameters).then(function () {
                    $scope.showCancelButton = true;
                    saveDraftDeferred.resolve();
                }).catch(function (error) {
                    saveDraftDeferred.reject(error);
                });
            }

            saveDraftDeferred.promise.then(function () {
                if (!shouldLoadGrid)
                    loadGridDeferred.resolve();
                else {
                    loadGrid().then(function () {
                        loadGridDeferred.resolve();
                    }).catch(function (error) {
                        loadGridDeferred.reject(error);
                    });
                }
            });

            return UtilsService.waitMultiplePromises(promises);
        }
        function manageCountries() {
            if ($scope.selectedCustomer != undefined) {
                var customerId = $scope.selectedCustomer.CarrierAccountId;
                var onCountryChangesUpdated = function (updatedCountryChanges) {
                    countryChanges = updatedCountryChanges;
                    var promises = [];

                    var saveDraftPromise = saveDraft(false);
                    promises.push(saveDraftPromise);

                    var defineNewRatesDeferred = UtilsService.createPromiseDeferred();
                    promises.push(defineNewRatesDeferred.promise);

                    var customerCurrencyId = currencySelectorAPI.getSelectedIds();

                    if (customerCurrencyId !== sellingProductCurrencyId && countryChanges != null && countryChanges.NewCountries != undefined && countryChanges.NewCountries.length > 0) {
                        saveDraftPromise.then(function () {

                            var defineNewRatesInput = {
                                CustomerId: getOwnerId(),
                                NewCurrencyId: customerCurrencyId,
                                EffectiveOn: UtilsService.getDateFromDateTime(VRDateTimeService.getNowDateTime())
                            };

                            defineNewRatesInput.NewCountryIds = [];

                            for (var i = 0; i < countryChanges.NewCountries.length; i++)
                                defineNewRatesInput.NewCountryIds.push(countryChanges.NewCountries[i].CountryId);

                            WhS_Sales_RatePlanAPIService.DefineNewRatesConvertedToCurrency(defineNewRatesInput).then(function () {
                                defineNewRatesDeferred.resolve();
                            }).catch(function (error) {
                                defineNewRatesDeferred.reject($scope, error);
                            });
                        });
                    }
                    else {
                        defineNewRatesDeferred.resolve();
                    }

                    UtilsService.waitMultiplePromises(promises).then(function () {
                        if (isRoutingInfoDefined())
                            loadRatePlan();
                    });
                };
                var manageCountriesInput = {
                    customerId: customerId,
                    countryChanges: countryChanges,
                    customerPricingSettings: ownerPricingSettings,
                    onCountryChangesUpdated: onCountryChangesUpdated
                };
                WhS_Sales_RatePlanService.manageCountries(manageCountriesInput);
            }
        }
        function getNewDraft() {
            var newDraft;

            var defaultDraft = getDefaultDraft();
            var zoneDrafts = gridAPI.getZoneDrafts();

            if (defaultDraft != undefined || zoneDrafts != undefined || countryChanges != undefined) {
                newDraft = {
                    CurrencyId: getCurrencyId(),
                    DefaultChanges: defaultDraft,
                    ZoneChanges: zoneDrafts,
                    CountryChanges: countryChanges,
                    SubscriberOwnerEntities: subscriberOwnerEntities
                };
            }
            return newDraft;
        }
        function getDefaultDraft() {
            if (defaultItem == undefined || !defaultItem.IsDirty)
                return null;

            var defaultDraft = {};
            for (var i = 0; i < $scope.defaultItemTabs.length; i++) {
                var defaultTab = $scope.defaultItemTabs[i];
                if (defaultTab.directiveAPI != undefined)
                    defaultTab.directiveAPI.applyChanges(defaultDraft);
            }
            defaultDraft.NewService = defaultItem.NewService;
            defaultDraft.ClosedService = defaultItem.ClosedService;
            defaultDraft.ResetService = defaultItem.ResetService;
            return defaultDraft;
        }

        function onCustomerChanged(customerId) {
            var promises = [];

            var isCustomerValid;
            var customerCurrencyId;

            var validateCustomerPromise = validateCustomer();
            promises.push(validateCustomerPromise);

            var getCustomerCurrencyIdDeferred = UtilsService.createPromiseDeferred();
            promises.push(getCustomerCurrencyIdDeferred.promise);

            var getCountryChangesDeferred = UtilsService.createPromiseDeferred();
            promises.push(getCountryChangesDeferred.promise);

            var loadOwnerInfoPromise = loadOwnerInfo();
            promises.push(loadOwnerInfoPromise);

            var loadOwnerPricingSettingsDeferred = UtilsService.createPromiseDeferred();
            promises.push(loadOwnerPricingSettingsDeferred.promise);

            validateCustomerPromise.then(function () {
                if (isCustomerValid) {
                    getCustomerCurrencyId().then(function () {
                        getCustomerCurrencyIdDeferred.resolve();
                    }).catch(function (error) {
                        getCustomerCurrencyIdDeferred.reject(error);
                    });
                    getCountryChanges(customerId).then(function () {
                        getCountryChangesDeferred.resolve();
                    }).catch(function (error) {
                        getCountryChangesDeferred.reject(error, $scope);
                    });
                    getOwnerPricingSettings().then(function () {
                        loadOwnerPricingSettingsDeferred.resolve();
                    }).catch(function (error) {
                        loadOwnerPricingSettingsDeferred.reject(error);
                    });
                }
                else {
                    getCustomerCurrencyIdDeferred.resolve();
                    getCountryChangesDeferred.resolve();
                    loadOwnerPricingSettingsDeferred.resolve();
                    VRNotificationService.showInformation($scope.selectedCustomer.Name + " is not assigned to a selling product");
                    $scope.selectedCustomer = undefined;
                }
            });

            function validateCustomer() {
                return WhS_Sales_RatePlanAPIService.ValidateCustomer(customerId, VRDateTimeService.getNowDateTime()).then(function (response) {
                    isCustomerValid = response;
                });
            }
            function getCustomerCurrencyId() {
                return WhS_BE_CarrierAccountAPIService.GetCarrierAccountCurrencyId(customerId).then(function (response) {
                    defaultCustomerCurrencyId = response;
                    currencySelectorAPI.selectedCurrency(response);
                });
            }

            return UtilsService.waitMultiplePromises(promises);
        }
        function getCountryChanges(customerId) {
            return WhS_Sales_RatePlanAPIService.GetCountryChanges(customerId).then(function (response) {
                countryChanges = response;
            });
        }

        function resetRatePlan() {
            resetZoneLetters();
            if (gridAPI != undefined)
                gridAPI.clearDataSource();
            showRatePlan(false);
            showActionBarButtons(false);
            countryChanges = undefined;
            subscriberOwnerEntities = undefined;
            subscriberOwnerIds = undefined;
        }
        function resetZoneLetters() {
            $scope.zoneLetters.length = 0;
            $scope.connector.selectedZoneLetterIndex = 0;
        }
        function showRatePlan(show) {
            $scope.showZoneLetters = show;
            $scope.showDefaultItem = show;
            $scope.showGrid = show;
        }
        function showActionBarButtons(show) {
            $scope.showSaveButton = show;
            $scope.showSettingsButton = show;
            $scope.showPricingButton = show;
            $scope.showCancelButton = show;
            $scope.showApplyButton = show;
        }

        function defineSellingProductApplyButtonMenuActions() {
            $scope.sellingProductApplyButtonMenuActions = [{
                name: "Draft",
                clicked: function () {
                    return saveDraft(false).then(function () {
                        VRNotificationService.showSuccess("Draft saved");
                    });
                }
            },
			{
			    name: "Offer",
			    clicked: applyDraft
			}];
        }

        function defineCustomerApplyButtonMenuActions() {
            $scope.customerApplyButtonMenuActions = [{
                name: "Draft",
                clicked: function () {
                    return saveDraft(false).then(function () {
                        VRNotificationService.showSuccess("Draft saved");
                    });
                }
            },
			{
			    name: "Offer",
			    clicked: applyDraft
			},
			{
			    name: "Offer for subscribers",
			    clicked: applyDraftOnMultipleCustomers
			}];
        }

        function applyDraft() {
            var promises = [];

            var ownerId = getOwnerId();
            var ownerTypeValue = ownerTypeSelectorAPI.getSelectedIds();

            var getEntityIdsPromise = getEntityIds(ownerTypeSelectorAPI.getSelectedIds(), ownerId);
            promises.push(getEntityIdsPromise);

            var hasRunningProcessesDeferred = UtilsService.createPromiseDeferred();
            promises.push(hasRunningProcessesDeferred.promise);

            var applyOfferDeferred = UtilsService.createPromiseDeferred();
            promises.push(applyOfferDeferred.promise);

            getEntityIdsPromise.then(function (entityIds) {

                hasRunningProcessesForCustomerOrSellingProduct(entityIds, ownerId, ownerTypeValue).then(function (response) {

                    hasRunningProcessesDeferred.resolve();

                    if (response.hasRunningProcesses) {
                        applyOfferDeferred.resolve();
                        VRNotificationService.showWarning("Cannot start process because another instance is still running");
                    }
                    else {
                        var applyOfferPromises = [];

                        var saveChangesPromise = saveDraft(false);
                        applyOfferPromises.push(saveChangesPromise);

                        var createProcessDeferred = UtilsService.createPromiseDeferred();
                        applyOfferPromises.push(createProcessDeferred.promise);

                        saveChangesPromise.then(function () {

                            var ownerTypeValue = ownerTypeSelectorAPI.getSelectedIds();
                            var ownerId = getOwnerId();

                            var inputArguments = {
                                $type: 'TOne.WhS.Sales.BP.Arguments.RatePlanInput, TOne.WhS.Sales.BP.Arguments',
                                OwnerType: ownerTypeValue,
                                OwnerId: ownerId,
                                CurrencyId: getCurrencyId(),
                                EffectiveDate: UtilsService.getDateFromDateTime(VRDateTimeService.getNowDateTime())
                            };

                            var input = {
                                InputArguments: inputArguments
                            };

                            BusinessProcess_BPInstanceAPIService.CreateNewProcess(input).then(function (response) {
                                createProcessDeferred.resolve();
                                if (response.Result == WhS_BP_CreateProcessResultEnum.Succeeded.value) {

                                    var processTrackingContext = {
                                        onClose: function (bpInstanceClosureContext) {
                                            if (bpInstanceClosureContext != undefined && bpInstanceClosureContext.bpInstanceStatusValue === BPInstanceStatusEnum.Completed.value) {

                                                resetRatePlan();

                                                $scope.isLoading = true;
                                                var promises = [];

                                                var loadOwnerInfoPromise = loadOwnerInfo();
                                                promises.push(loadOwnerInfoPromise);

                                                if (ownerTypeValue == WhS_BE_SalePriceListOwnerTypeEnum.Customer.value) {

                                                    var doesOwnerDraftExistDeferred = UtilsService.createPromiseDeferred();
                                                    promises.push(doesOwnerDraftExistDeferred.promise);

                                                    var getCountryChangesPromise = getCountryChanges(ownerId);
                                                    promises.push(getCountryChangesPromise);

                                                    getCountryChangesPromise.then(function () {
                                                        doesOwnerDraftExist().then(function () {
                                                            doesOwnerDraftExistDeferred.resolve();
                                                        }).catch(function (error) {
                                                            doesOwnerDraftExistDeferred.reject(error);
                                                        });
                                                    });
                                                }

                                                UtilsService.waitMultiplePromises(promises).finally(function () {
                                                    $scope.isLoading = false;
                                                });
                                            }
                                        }
                                    };

                                    BusinessProcess_BPInstanceService.openProcessTracking(response.ProcessInstanceId, processTrackingContext);
                                }
                            }).catch(function (error) {
                                createProcessDeferred.reject(error);
                            });
                        });

                        UtilsService.waitMultiplePromises(applyOfferPromises).then(function () {
                            applyOfferDeferred.resolve();
                        }).catch(function (error) {
                            applyOfferDeferred.reject(error);
                        });
                    }
                });
            });

            return UtilsService.waitMultiplePromises(promises);
        }

        function applyDraftOnMultipleCustomers() {
            var promises = [];
            var sellingNumberPlanId = getOwnerSellingNumberPlanId();
            var ownerId = getOwnerId();
            var ownerTypeValue = ownerTypeSelectorAPI.getSelectedIds();

            var getEntityIdsPromise = getEntityIds(ownerTypeSelectorAPI.getSelectedIds(), ownerId);
            promises.push(getEntityIdsPromise);

            var hasRunningProcessesDeferred = UtilsService.createPromiseDeferred();
            promises.push(hasRunningProcessesDeferred.promise);

            var applyOfferDeferred = UtilsService.createPromiseDeferred();
            promises.push(applyOfferDeferred.promise);

            getEntityIdsPromise.then(function (entityIds) {

                hasRunningProcessesForCustomerOrSellingProduct(entityIds, ownerId, ownerTypeValue).then(function (response) {

                    hasRunningProcessesDeferred.resolve();

                    if (response.hasRunningProcesses) {
                        applyOfferDeferred.resolve();
                        VRNotificationService.showWarning("Cannot start process because another instance is still running");
                    }
                    else {
                        var ownerId = getOwnerId();
                        WhS_Sales_RatePlanService.applyDraftOnMultipleCustomers(executeApplyDraftOnMultipleCustomersProcess, ownerId, sellingNumberPlanId, customerSellingProductId);
                        applyOfferDeferred.resolve();
                    }
                });
            });

            return UtilsService.waitMultiplePromises(promises);
        }
        function executeApplyDraftOnMultipleCustomersProcess(subscriberOwners, followPublisherRatesBED, followPublisherRountingProduct) {
            subscriberOwnerEntities = [];
            subscriberOwnerIds = [];
            if (subscriberOwners != null) {
                for (var i = 0; i < subscriberOwners.length; i++) {
                    subscriberOwnerEntities.push(subscriberOwners[i].Entity);
                    subscriberOwnerIds.push(subscriberOwners[i].Entity.EntityId);
                }
            }
            var applyOfferDeferred = UtilsService.createPromiseDeferred();
            var applyOfferPromises = [];

            var saveChangesPromise = saveDraft(false);
            applyOfferPromises.push(saveChangesPromise);

            var createProcessDeferred = UtilsService.createPromiseDeferred();
            applyOfferPromises.push(createProcessDeferred.promise);

            saveChangesPromise.then(function () {

                var ownerTypeValue = ownerTypeSelectorAPI.getSelectedIds();
                var ownerId = getOwnerId();

                var inputArguments = {
                    $type: 'TOne.WhS.Sales.BP.Arguments.RatePlanInput, TOne.WhS.Sales.BP.Arguments',
                    OwnerType: ownerTypeValue,
                    OwnerId: ownerId,
                    CurrencyId: getCurrencyId(),
                    EffectiveDate: UtilsService.getDateFromDateTime(VRDateTimeService.getNowDateTime()),
                    SubscriberOwnerIds: subscriberOwnerIds,
                    FollowPublisherRatesBED: followPublisherRatesBED,
                    FollowPublisherRountingProduct: followPublisherRountingProduct
                };

                var input = {
                    InputArguments: inputArguments
                };

                BusinessProcess_BPInstanceAPIService.CreateNewProcess(input).then(function (response) {
                    createProcessDeferred.resolve();
                    if (response.Result == WhS_BP_CreateProcessResultEnum.Succeeded.value) {

                        var processTrackingContext = {
                            onClose: function (bpInstanceClosureContext) {
                                if (bpInstanceClosureContext != undefined && bpInstanceClosureContext.bpInstanceStatusValue === BPInstanceStatusEnum.Completed.value) {

                                    resetRatePlan();

                                    $scope.isLoading = true;
                                    var promises = [];

                                    var loadOwnerInfoPromise = loadOwnerInfo();
                                    promises.push(loadOwnerInfoPromise);

                                    if (ownerTypeValue == WhS_BE_SalePriceListOwnerTypeEnum.Customer.value) {

                                        var doesOwnerDraftExistDeferred = UtilsService.createPromiseDeferred();
                                        promises.push(doesOwnerDraftExistDeferred.promise);

                                        var getCountryChangesPromise = getCountryChanges(ownerId);
                                        promises.push(getCountryChangesPromise);

                                        getCountryChangesPromise.then(function () {
                                            doesOwnerDraftExist().then(function () {
                                                doesOwnerDraftExistDeferred.resolve();
                                            }).catch(function (error) {
                                                doesOwnerDraftExistDeferred.reject(error);
                                            });
                                        });
                                    }

                                    UtilsService.waitMultiplePromises(promises).finally(function () {
                                        $scope.isLoading = false;
                                    });
                                }
                            }
                        };

                        BusinessProcess_BPInstanceService.openProcessTracking(response.ProcessInstanceId, processTrackingContext);
                    }
                }).catch(function (error) {
                    createProcessDeferred.reject(error);
                });
            });

            UtilsService.waitMultiplePromises(applyOfferPromises).then(function () {
                applyOfferDeferred.resolve();
            }).catch(function (error) {
                applyOfferDeferred.reject(error);
            });
            return applyOfferDeferred.promise;
        }

        function getOwnerId() {
            var ownerTypeValue = ownerTypeSelectorAPI.getSelectedIds();
            return (ownerTypeValue == WhS_BE_SalePriceListOwnerTypeEnum.SellingProduct.value) ? sellingProductSelectorAPI.getSelectedIds() : carrierAccountSelectorAPI.getSelectedIds();
        }
        function getOwnerName() {
            var ownerName = null;

            if ($scope.showSellingProductSelector)
                ownerName = $scope.selectedSellingProduct.Name;
            else if ($scope.showCarrierAccountSelector)
                ownerName = $scope.selectedCustomer.Name;

            return ownerName;
        }

        function getOwnerSellingNumberPlanId() {
            var ownerType = ownerTypeSelectorAPI.getSelectedIds();
            if (ownerType == WhS_BE_SalePriceListOwnerTypeEnum.SellingProduct.value) {
                if ($scope.selectedSellingProduct != undefined)
                    return $scope.selectedSellingProduct.SellingNumberPlanId;
            }
            else {
                if ($scope.selectedCustomer != undefined)
                    return $scope.selectedCustomer.SellingNumberPlanId;
            }
        }
        function getCurrencyId() {
            return (ownerTypeSelectorAPI.getSelectedIds() == WhS_BE_SalePriceListOwnerTypeEnum.SellingProduct.value) ? sellingProductCurrencyId : currencySelectorAPI.getSelectedIds();
        }
        function isFilterApplied() {
            var selectedCountryIds = countrySelectorAPI.getSelectedIds();
            if (selectedCountryIds != undefined)
                return true;
            var textFilterData = textFilterAPI.getData();
            if (textFilterData != undefined)
                return true;
            return false;
        }

        function doesOwnerDraftExist() {
            var ownerTypeValue = ownerTypeSelectorAPI.getSelectedIds();
            var ownerId = getOwnerId();

            return WhS_Sales_RatePlanAPIService.CheckIfDraftExists(ownerTypeValue, ownerId).then(function (response) {
                $scope.showCancelButton = response === true;
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }
        function isRoutingInfoDefined() {
            return (databaseSelectorAPI.getSelectedIds() != undefined && policySelectorAPI.getSelectedIds() != undefined && $scope.numberOfOptions != undefined);
        }

        function loadOwnerInfo() {
            var ownerType = ownerTypeSelectorAPI.getSelectedIds();
            var ownerId = getOwnerId();
            var effectiveOn = UtilsService.getDateFromDateTime(VRDateTimeService.getNowDateTime());

            return WhS_Sales_RatePlanAPIService.GetOwnerInfo(ownerType, ownerId, effectiveOn).then(function (response) {
                if (response != undefined) {
                    $scope.assignedToSellingProductName = response.AssignedToSellingProductName;
                    $scope.assignedToSellingProductCurrencySymbol = response.AssignedToSellingProductCurrencySymbol;
                    $scope.currentDefaultRoutingProductName = response.CurrentDefaultRoutingProductName;
                    if (response.IsCurrentDefaultRoutingProductInherited)
                        $scope.currentDefaultRoutingProductName += ' (Inherited)';

                    $scope.newDefaultRoutingProductName = response.NewDefaultRoutingProductName;
                    $scope.resetToDefaultRoutingProductName = response.ResetToDefaultRoutingProductName;
                }
            });
        }
        function clearOwnerInfo() {
            $scope.assignedToSellingProductName = undefined;
            $scope.currentDefaultRoutingProductName = undefined;
            $scope.newDefaultRoutingProductName = undefined;
            $scope.resetToDefaultRoutingProductName = undefined;
        }

        function showActionButtons(showValue) {
            $scope.showSettingsButton = showValue;
            $scope.showBulkActionButton = showValue;
            $scope.showSaveButton = showValue;
            $scope.showCancelButton = showValue;
            ////  setTimeout(function () {
            //      UtilsService.safeApply($scope);
            // // }, 1);
        }
        function getSellingProductCurrencyId(sellingProductId) {
            return WhS_BE_SellingProductAPIService.GetSellingProductCurrencyId(sellingProductId).then(function (response) {
                sellingProductCurrencyId = response;
                currencySelectorAPI.selectedCurrency(response);
            });
        }
        function getEntityIds(ownerType, ownerId) {
            var entityIdPromiseDeferred = UtilsService.createPromiseDeferred();

            var getEntityIdsPromise;
            var entityIds = [];

            switch (ownerType) {
                case WhS_BE_SalePriceListOwnerTypeEnum.SellingProduct.value:
                    WhS_BE_CarrierAccountAPIService.GetCarrierAccountIdsAssignedToSellingProduct(ownerId).then(function (response) {
                        if (response != undefined) {
                            var i;
                            for (i = 0; i < response.length; i++) {
                                var entityId = WhS_BE_SalePriceListOwnerTypeEnum.Customer.value + "_" + response[i];
                                entityIds.push(entityId);
                            }
                        }
                        entityIds.push(WhS_BE_SalePriceListOwnerTypeEnum.SellingProduct.value + "_" + ownerId);
                        entityIdPromiseDeferred.resolve(entityIds);
                    }).catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    }).finally(function () {
                    });
                    break;
                case WhS_BE_SalePriceListOwnerTypeEnum.Customer.value:

                    WhS_BE_CarrierAccountAPIService.GetSellingProductId(ownerId).then(function (response) {
                        if (response != undefined) {
                            customerSellingProductId = response;
                            var entityId = WhS_BE_SalePriceListOwnerTypeEnum.SellingProduct.value + "_" + response;
                            entityIds.push(entityId);
                        }
                        entityIds.push(WhS_BE_SalePriceListOwnerTypeEnum.Customer.value + "_" + ownerId);
                        entityIdPromiseDeferred.resolve(entityIds);
                    });
                    break;
            }
            return entityIdPromiseDeferred.promise;
        }
        function hasRunningProcessesForCustomerOrSellingProduct(entityIds, ownerId, ownerType) {

            var editorMessage;

            if (ownerType == WhS_BE_SalePriceListOwnerTypeEnum.SellingProduct.value) {
                var sellingProductName = ($scope.selectedSellingProduct != undefined) ? $scope.selectedSellingProduct.Name : undefined;
                editorMessage = "Other rate plan processes are still pending for selling product '" + sellingProductName + "' or some of its customers";
            }
            else {
                var customerName = ($scope.selectedCustomer != undefined) ? $scope.selectedCustomer.Name : undefined;
                editorMessage = "Other rate plan processes are still pending for customer '" + customerName + "' or its selling product";
            }

            var runningInstanceEditorSettings = { message: editorMessage };
            return BusinessProcess_BPInstanceService.displayRunningInstancesIfExist(WhS_BP_RatePlanDefinitionEnum.BPDefinitionId.value, entityIds, runningInstanceEditorSettings);
        }

        function tryTakeSession(targetId) {
            var exclusiveSessionInput = {
                SessionTypeId: WhS_BE_ExclusiveSessionTypeIdEnum.SaleArea.value,
                TargetId: targetId,
            };
            var promiseDeffered = UtilsService.createPromiseDeferred();
            VRCommon_VRExclusiveSessionTypeService.tryTakeSession($scope, exclusiveSessionInput).then(function (response) {
                promiseDeffered.resolve(response);
                if (response.IsSucceeded) {
                    response.onTryTakeFailure = onTryTakeFailure;
                    $scope.$on("$destroy", function () {
                        response.Release();
                    });
                    exclusiveSessionObject = response;
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
            return promiseDeffered.promise;
        }
        function onTryTakeFailure(failureObject) {
            return VRNotificationService.showPromptWarning(failureObject.FailureMessage).then(function () {
                $scope.selectedSellingProduct = null;
                $scope.selectedCustomer = null;
            });
        }

        function releaseSession() {
            if (exclusiveSessionObject != null) {
                exclusiveSessionObject.Release();
                exclusiveSessionObject = null;
            }
        }
    }

    appControllers.controller("WhS_Sales_RatePlanController", RatePlanController);

})(appControllers);