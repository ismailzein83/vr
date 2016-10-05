(function (appControllers) {

    "use strict";

    RatePlanController.$inject = ["$scope", "WhS_Sales_RatePlanService", "WhS_Sales_RatePlanAPIService", "WhS_BE_SalePriceListOwnerTypeEnum", "WhS_Sales_RatePlanStatusEnum", 'BusinessProcess_BPInstanceAPIService', 'BusinessProcess_BPInstanceService', 'WhS_BP_CreateProcessResultEnum', 'VRCommon_CurrencyAPIService', 'WhS_BE_CarrierAccountAPIService', "UtilsService", "VRUIUtilsService", "VRNotificationService"];

    function RatePlanController($scope, WhS_Sales_RatePlanService, WhS_Sales_RatePlanAPIService, WhS_BE_SalePriceListOwnerTypeEnum, WhS_Sales_RatePlanStatusEnum, BusinessProcess_BPInstanceAPIService, BusinessProcess_BPInstanceService, WhS_BP_CreateProcessResultEnum, VRCommon_CurrencyAPIService, WhS_BE_CarrierAccountAPIService, UtilsService, VRUIUtilsService, VRNotificationService)
    {
        var ownerTypeSelectorAPI;
        var ownerTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var sellingProductSelectorAPI;
        var sellingProductSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var carrierAccountSelectorAPI;
        var carrierAccountSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var databaseSelectorAPI;
        var databaseSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var policySelectorAPI;

        var currencySelectorAPI;
        var currencySelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var systemCurrencyId;
        var draftCurrencyId;
        var defaultCustomerCurrencyId;

        var countrySelectorAPI;
        var countrySelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var textFilterAPI;
        var textFilterReadyDeferred = UtilsService.createPromiseDeferred();

        var defaultItem;

        var gridAPI;
        var gridReadyDeferred = UtilsService.createPromiseDeferred();

        var settings;
        var pricingSettings;
        var ratePlanSettingsData;
        var saleAreaSettingsData;

        defineScope();
        load();

        function defineScope()
        {
            /* These vars are reversed with every onOwnerTypeChanged. Therefore, the selling product selector will show when the event first occurs */
            $scope.showSellingProductSelector = false;
            $scope.showCarrierAccountSelector = true;
            /* ***** */

            $scope.onOwnerTypeSelectorReady = function (api) {
                ownerTypeSelectorAPI = api;
                ownerTypeSelectorReadyDeferred.resolve();
            };
            $scope.onOwnerTypeChanged = function (item)
            {
                resetRatePlan();
                draftCurrencyId = undefined;

                var selectedId = ownerTypeSelectorAPI.getSelectedIds();

                if (selectedId == undefined)
                    return;

                $scope.showSellingProductSelector = !$scope.showSellingProductSelector;
                $scope.showCarrierAccountSelector = !$scope.showCarrierAccountSelector;

                if (selectedId == WhS_BE_SalePriceListOwnerTypeEnum.SellingProduct.value) {
                    $scope.selectedCustomer = undefined;
                }
                else if (selectedId == WhS_BE_SalePriceListOwnerTypeEnum.Customer.value) {
                    $scope.selectedSellingProduct = undefined;
                }
            };

            $scope.onSellingProductSelectorReady = function (api) {
                sellingProductSelectorAPI = api;
                sellingProductSelectorReadyDeferred.resolve();
            };
            $scope.onSellingProductChanged = function ()
            {
                resetRatePlan();
            };

            $scope.onCarrierAccountSelectorReady = function (api) {
                carrierAccountSelectorAPI = api;
                carrierAccountSelectorReadyDeferred.resolve();
            };
            $scope.onCarrierAccountChanged = function ()
            {
                resetRatePlan();
                draftCurrencyId = undefined;

                var selectedId = carrierAccountSelectorAPI.getSelectedIds();

                if (selectedId != undefined)
                {
                    $scope.isLoadingFilterSection = true;
                    onCustomerChanged(selectedId).finally(function () {
                        $scope.isLoadingFilterSection = false;
                    });
                }
            };

            $scope.numberOfOptions = 3;

            $scope.onDatabaseSelectorReady = function (api) {
                databaseSelectorAPI = api;
                databaseSelectorReadyDeferred.resolve();
            };

            $scope.onRoutingDatabaseChanged = function ()
            {
                resetRatePlan();
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

            $scope.onCurrencyChanged = function ()
            {
                if (draftCurrencyId == undefined)
                    return;

                var selectedId = currencySelectorAPI.getSelectedIds();
                if (selectedId == undefined)
                    return;

                if (selectedId == draftCurrencyId)
                    return;

                VRNotificationService.showConfirmation('Changing the currency will reset all new rates. Are you sure you want to proceed?').then(function (isConfirmed) {
                    if (isConfirmed)
                    {
                        var promises = [];

                        var saveChangesPromise = saveDraft(false);
                        promises.push(saveChangesPromise);

                        var deleteChangedRatesDeferred = UtilsService.createPromiseDeferred();
                        promises.push(deleteChangedRatesDeferred.promise);

                        saveChangesPromise.then(function () {
                            WhS_Sales_RatePlanAPIService.DeleteChangedRates(ownerTypeSelectorAPI.getSelectedIds(), getOwnerId(), selectedId).then(function () {
                                draftCurrencyId = selectedId;
                                deleteChangedRatesDeferred.resolve();
                            }).catch(function (error) {
                                deleteChangedRatesDeferred.reject(error);
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
                title: "Default Services",
                directive: "vr-whs-sales-default-service",
                loadDirective: function (api)
                {
                    defaultItem.context = {};
                    defaultItem.context.getNewDraft = getNewDraft;
                    defaultItem.context.saveDraft = saveDraft;
                    defaultItem.context.loadGrid = loadGrid;

                    var defaultServicePayload = {
                        defaultItem: defaultItem,
                        settings: {
                            newServiceDayOffset: ratePlanSettingsData.NewServiceDayOffset
                        }
                    };

                    return api.load(defaultServicePayload);
                }
            }, {
                title: "Default Routing Product",
                directive: "vr-whs-sales-defaultroutingproduct",
                loadDirective: function (api) {
                    defaultItem.onChange = onDefaultItemChanged;
                    return api.load(defaultItem);
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

            $scope.search = function () {
                return loadRatePlan();
            };
            $scope.sellNewCountries = function () {
                var customerId = $scope.selectedCustomer.CarrierAccountId;
                var onCountriesSold = function (customerZones)
                {
                    if (databaseSelectorAPI.getSelectedIds() != null && policySelectorAPI.getSelectedIds() != null)
                        loadRatePlan();
                };

                WhS_Sales_RatePlanService.sellNewCountries(customerId, onCountriesSold);
            };
            $scope.editSettings = function ()
            {
                var onSettingsUpdated = function (updatedSettings)
                {
                    if (updatedSettings == undefined)
                        settings = undefined;
                    else
                    {
                        settings = {};
                        if (updatedSettings.costCalculationMethods != undefined)
                        {
                            settings.costCalculationMethods = [];
                            for (var i = 0; i < updatedSettings.costCalculationMethods.length; i++) {
                                settings.costCalculationMethods.push(updatedSettings.costCalculationMethods[i]);
                            }
                        }
                    }

                    pricingSettings = null;
                    $scope.showApplyButton = false;

                    VRNotificationService.showSuccess("Settings saved");

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
                WhS_Sales_RatePlanService.editSettings(settings, onSettingsUpdated);
            };
            $scope.editPricingSettings = function ()
            {
                var onPricingSettingsUpdated = function (updatedPricingSettings)
                {
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
            $scope.applyCalculatedRates = function ()
            {
                var promises = [];

                var confirmPromise = VRNotificationService.showConfirmation("Are you sure you want to apply the calculated rates?");
                promises.push(confirmPromise);

                var saveChangesDeferred = UtilsService.createPromiseDeferred();
                promises.push(saveChangesDeferred.promise);

                var applyDeferred = UtilsService.createPromiseDeferred();
                promises.push(applyDeferred.promise);

                var calculatedRates;

                confirmPromise.then(function (confirmed) {
                    if (confirmed)
                    {
                        saveDraft(false).then(function ()
                        {
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

                        UtilsService.waitMultiplePromises([saveChangesDeferred.promise, applyDeferred.promise]).then(function ()
                        {
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
                        EffectiveOn: new Date(),
                        RoutingDatabaseId: databaseSelectorAPI ? databaseSelectorAPI.getSelectedIds() : null,
                        PolicyConfigId: policySelectorAPI ? policySelectorAPI.getSelectedIds() : null,
                        NumberOfOptions: $scope.numberOfOptions,
                        CostCalculationMethods: settings ? settings.costCalculationMethods : null,
                        SelectedCostCalculationMethodConfigId: pricingSettings ? pricingSettings.selectedCostColumn.ConfigId : null,
                        RateCalculationMethod: pricingSettings ? pricingSettings.selectedRateCalculationMethodData : null,
                        CurrencyId: getCurrencyId(),
                        CountryIds: countrySelectorAPI.getSelectedIds()
                    };

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
                        EffectiveOn: new Date(),
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

                var confirmPromise = VRNotificationService.showConfirmation("Are you sure you want to cancel all of your changes?");
                promises.push(confirmPromise);

                var deleteDeferred = UtilsService.createPromiseDeferred();
                promises.push(deleteDeferred.promise);

                confirmPromise.then(function (confirmed) {
                    if (confirmed) {
                        return WhS_Sales_RatePlanAPIService.DeleteDraft(ownerTypeSelectorAPI.getSelectedIds(), getOwnerId()).then(function (response) {
                            if (response) {
                                deleteDeferred.resolve();
                                VRNotificationService.showSuccess("Draft deleted");
                                $scope.showCancelButton = false;
                                loadRatePlan();
                            }
                            else {
                                deleteDeferred.reject();
                            }
                        }).catch(function (error) {
                            deleteDeferred.reject();
                            VRNotificationService.notifyException(error, $scope);
                        });
                    }
                    else {
                        deleteDeferred.resolve();
                    }
                });

                return UtilsService.waitMultiplePromises(promises);
            };

            defineSaveButtonMenuActions();
        }

        function load() {
            $scope.isLoadingFilterSection = true;
            loadAllControls();
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadOwnerFilterSection, loadRouteOptionsFilterSection, loadCurrencySelector, loadRatePlanSettingsData, loadSaleAreaSettingsData, getSystemCurrencyId, loadCountrySelector, loadTextFilter]).catch(function (error) {
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
                var ownerTypeSelectorPayload = { selectedIds: WhS_BE_SalePriceListOwnerTypeEnum.SellingProduct.value };
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
            });
        }
        function loadSaleAreaSettingsData() {
            return WhS_Sales_RatePlanAPIService.GetSaleAreaSettingsData().then(function (response) {
                saleAreaSettingsData = response;
            });
        }
        function getSystemCurrencyId() {
            return VRCommon_CurrencyAPIService.GetSystemCurrencyId().then(function (response) {
                systemCurrencyId = response;
            });
        }
        function loadCountrySelector()
        {
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

        function loadRatePlan()
        {
            $scope.isLoadingRatePlan = true;
            var promises = [];

            $scope.showZoneLetters = false; // This is to delete the choices directive from the dom, via ng-if, to avoid index problems
            $scope.connector.selectedZoneLetterIndex = 0;

            var ownerTypeValue = ownerTypeSelectorAPI.getSelectedIds();

            var isLoadingZoneLetters = true;

            var zoneLettersGetPromise = getZoneLetters();
            promises.push(zoneLettersGetPromise);

            var getDraftCurrencyIdPromise = getDraftCurrencyId();
            promises.push(getDraftCurrencyIdPromise);

            var loadGridDeferred = UtilsService.createPromiseDeferred();
            promises.push(loadGridDeferred.promise);

            var checkIfDraftExistsPromise = checkIfDraftExists();
            promises.push(checkIfDraftExistsPromise);

            UtilsService.waitMultiplePromises([zoneLettersGetPromise, getDraftCurrencyIdPromise]).then(function ()
            {
                if ($scope.zoneLetters.length > 0) {
                    $scope.showSaveButton = true;
                    $scope.showSettingsButton = true;
                    $scope.showPricingButton = true;

                    loadGrid().then(function () {
                        loadGridDeferred.resolve();
                        showRatePlan(true); // At this point, there's no guarantee that the default item has loaded. But that's okay since the tab directive displays a loader for the default item
                    }).catch(function (error) { loadGridDeferred.reject(error); });
                }
                else {
                    loadGridDeferred.resolve();
                    showRatePlan(false);

                    if (isFilterApplied()) {
                        VRNotificationService.showInformation('No effective zones match the filter');
                    }
                    else {
                        if (ownerTypeValue == WhS_BE_SalePriceListOwnerTypeEnum.SellingProduct.value)
                            VRNotificationService.showInformation('No effective zones exist for this selling product');
                        else
                            VRNotificationService.showInformation("No countries are sold to this customer or no effective zones exist for its assigned selling product");
                    }
                }
            });

            zoneLettersGetPromise.finally(function () {
                isLoadingZoneLetters = false;
            });

            var defaultItemGetPromise = getDefaultItem();
            promises.push(defaultItemGetPromise);

            defaultItemGetPromise.then(function (response) {
                if (response) {
                    defaultItem = response;
                    defaultItem.OwnerType = ownerTypeValue;
                    defaultItem.OwnerId = getOwnerId();

                    for (var i = 0; i < $scope.defaultItemTabs.length; i++) {
                        var item = $scope.defaultItemTabs[i];

                        if (item.directiveAPI)
                            item.loadDirective(item.directiveAPI);
                    }
                }
            });

            return UtilsService.waitMultiplePromises(promises).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoadingRatePlan = false;
            });

            function checkIfDraftExists() {
                return WhS_Sales_RatePlanAPIService.CheckIfDraftExists(ownerTypeValue, getOwnerId()).then(function (response) {
                    $scope.showCancelButton = response === true;
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope); // The user can perform other tasks if CheckIfDraftExists fails
                });
            }

            function getZoneLetters() {
                var input = {
                    OwnerType: ownerTypeValue,
                    OwnerId: getOwnerId(),
                    EffectiveOn: new Date(),
                    CountryIds: countrySelectorAPI.getSelectedIds()
                };

                var textFilterData = textFilterAPI.getData();
                if (textFilterData != undefined) {
                    input.ZoneNameFilterType = textFilterData.TextFilterType;
                    input.ZoneNameFilter = textFilterData.Text;
                }

                return WhS_Sales_RatePlanAPIService.GetZoneLetters(input).then(function (response) {
                    if (response) {
                        $scope.zoneLetters.length = 0;

                        for (var i = 0; i < response.length; i++) {
                            $scope.zoneLetters.push(response[i]);
                        }
                    }
                });
            }

            function getDefaultItem() {
                var effectiveOn = UtilsService.getDateFromDateTime(new Date());
                return WhS_Sales_RatePlanAPIService.GetDefaultItem(ownerTypeValue, getOwnerId(), effectiveOn);
            }
        }
        function loadGrid() {
            var gridQuery = getGridQuery();
            return gridAPI.loadGrid(gridQuery);
        }
        function getGridQuery() {
            var gridQuery = {
                OwnerType: ownerTypeSelectorAPI.getSelectedIds(),
                OwnerId: getOwnerId(),
                ZoneLetter: $scope.zoneLetters[$scope.connector.selectedZoneLetterIndex],
                RoutingDatabaseId: databaseSelectorAPI.getSelectedIds(),
                PolicyConfigId: policySelectorAPI.getSelectedIds(),
                NumberOfOptions: $scope.numberOfOptions,
                CostCalculationMethods: settings != undefined ? settings.costCalculationMethods : null,
                Settings: ratePlanSettingsData,
                SaleAreaSettings: saleAreaSettingsData,
                CurrencyId: getCurrencyId(),
                CountryIds: countrySelectorAPI.getSelectedIds()
            };

            gridQuery.context = {};
            gridQuery.context.getNewDraft = getNewDraft;
            gridQuery.context.saveDraft = saveDraft;

            if (pricingSettings != undefined) {
                gridQuery.RateCalculationMethod = pricingSettings.selectedRateCalculationMethodData;
                if (pricingSettings.selectedCostColumn != null)
                    gridQuery.CostCalculationMethodConfigId = pricingSettings.selectedCostColumn.ConfigId;
            }

            var textFilterData = textFilterAPI.getData();
            if (textFilterData != undefined) {
                gridQuery.ZoneNameFilter = textFilterData.Text;
                gridQuery.ZoneNameFilterType = textFilterData.TextFilterType;
            }

            return gridQuery;
        }
        function getDraftCurrencyId()
        {
            if (ownerTypeSelectorAPI.getSelectedIds() == WhS_BE_SalePriceListOwnerTypeEnum.Customer.value)
            {
                return WhS_Sales_RatePlanAPIService.GetDraftCurrencyId(WhS_BE_SalePriceListOwnerTypeEnum.Customer.value, getOwnerId()).then(function (response) {
                    if (response != null) {
                        draftCurrencyId = response;
                        currencySelectorAPI.selectedCurrency(response)
                    }
                    if (draftCurrencyId == undefined)
                        draftCurrencyId = defaultCustomerCurrencyId;
                });
            }
            else {
                var deferred = UtilsService.createPromiseDeferred();
                deferred.resolve();
                return deferred.promise;
            }
        }
        
        function saveDraft(shouldLoadGrid)
        {
            var promises = [];

            var saveDraftDeferred = UtilsService.createPromiseDeferred();
            promises.push(saveDraftDeferred.promise);

            var loadGridDeferred = UtilsService.createPromiseDeferred();
            promises.push(loadGridDeferred.promise);

            var newDraft = getNewDraft();

            if (newDraft == undefined)
                saveDraftDeferred.resolve();
            else
            {
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
                else
                {
                    loadGrid().then(function () {
                        loadGridDeferred.resolve();
                    }).catch(function (error) {
                        loadGridDeferred.reject(error);
                    });
                }
            });

            return UtilsService.waitMultiplePromises(promises);
        }
        function getNewDraft()
        {
            var newDraft;

            var defaultDraft = getDefaultDraft();
            var zoneDrafts = gridAPI.getZoneDrafts();

            if (defaultDraft != undefined || zoneDrafts != undefined)
            {
                newDraft = {
                    CurrencyId: getCurrencyId(),
                    DefaultChanges: defaultDraft,
                    ZoneChanges: zoneDrafts
                };
            }
            return newDraft;
        }
        function getDefaultDraft()
        {
            var defaultDraft;

            if (defaultItem.IsDirty)
            {
                defaultDraft = {};
                for (var i = 0; i < $scope.defaultItemTabs.length; i++) {
                    var defaultTab = $scope.defaultItemTabs[i];
                    if (defaultTab.directiveAPI != undefined)
                        defaultTab.directiveAPI.applyChanges(defaultDraft);
                }
                defaultDraft.NewService = defaultItem.NewService;
                defaultDraft.ClosedService = defaultItem.ClosedService;
                defaultDraft.ResetService = defaultItem.ResetService;
            }
            return defaultDraft;
        }

        // TODO: Remove
        function onDefaultItemChanged()
        {
            return saveDraft(true);
        }
        function onCustomerChanged(selectedCarrierAccountId) {
            var promises = [];

            var isCustomerValid;
            var customerCurrencyId;

            var validateCustomerPromise = validateCustomer();
            promises.push(validateCustomerPromise);

            var getCustomerCurrencyIdDeferred = UtilsService.createPromiseDeferred();
            promises.push(getCustomerCurrencyIdDeferred.promise);

            validateCustomerPromise.then(function () {
                if (isCustomerValid) {
                    getCustomerCurrencyId().then(function () {
                        getCustomerCurrencyIdDeferred.resolve();
                    }).catch(function (error) {
                        getCustomerCurrencyIdDeferred.reject(error);
                    });
                }
                else {
                    getCustomerCurrencyIdDeferred.resolve();
                    VRNotificationService.showInformation($scope.selectedCustomer.Name + " is not assigned to a selling product");
                    $scope.selectedCustomer = undefined;
                }
            });

            function validateCustomer() {
                return WhS_Sales_RatePlanAPIService.ValidateCustomer(selectedCarrierAccountId, new Date()).then(function (response) {
                    isCustomerValid = response;
                });
            }
            function getCustomerCurrencyId() {
                return WhS_BE_CarrierAccountAPIService.GetCarrierAccountCurrencyId(selectedCarrierAccountId).then(function (response) {
                    defaultCustomerCurrencyId = response;
                    currencySelectorAPI.selectedCurrency(response);
                });
            }

            return UtilsService.waitMultiplePromises(promises);
        }

        function resetRatePlan() {
            resetZoneLetters();
            showRatePlan(false);
            showActionBarButtons(false);
        }
        function resetZoneLetters()
        {
            $scope.zoneLetters.length = 0;
            $scope.connector.selectedZoneLetterIndex = 0;
        }
        function showRatePlan(show) {
            $scope.showZoneLetters = show;
            $scope.showDefaultItem = show;
            $scope.showGrid = show;
        }
        function showActionBarButtons(show)
        {
            $scope.showSaveButton = show;
            $scope.showSettingsButton = show;
            $scope.showPricingButton = show;
            $scope.showCancelButton = show;
            $scope.showApplyButton = show;
        }

        function defineSaveButtonMenuActions() {
            $scope.saveButtonMenuActions = [{
                name: "Draft",
                clicked: function () {
                    return saveDraft(false).then(function () {
                        VRNotificationService.showSuccess("Draft saved");
                    });
                }
            }, {
                name: "Apply Draft",
                clicked: applyDraft
            }];
        }
        function applyDraft()
        {
            var promises = [];

            var saveChangesPromise = saveDraft(false);
            promises.push(saveChangesPromise);

            var createProcessDeferred = UtilsService.createPromiseDeferred();
            promises.push(createProcessDeferred.promise);

            saveChangesPromise.then(function ()
            {
                var inputArguments = {
                    $type: 'TOne.WhS.Sales.BP.Arguments.RatePlanInput, TOne.WhS.Sales.BP.Arguments',
                    OwnerType: ownerTypeSelectorAPI.getSelectedIds(),
                    OwnerId: getOwnerId(),
                    CurrencyId: getCurrencyId(),
                    EffectiveDate: new Date()
                };

                var input = {
                    InputArguments: inputArguments
                };

                BusinessProcess_BPInstanceAPIService.CreateNewProcess(input).then(function (response) {
                    createProcessDeferred.resolve();
                    if (response.Result == WhS_BP_CreateProcessResultEnum.Succeeded.value) {

                        var processTrackingContext = {
                            onClose: function () {
                                loadRatePlan();
                            }
                        };

                        BusinessProcess_BPInstanceService.openProcessTracking(response.ProcessInstanceId, processTrackingContext);
                    }
                }).catch(function (error) {
                    createProcessDeferred.reject(error);
                });
            });

            return UtilsService.waitMultiplePromises(promises);
        }

        function getOwnerId() {
            var ownerId = null;

            if ($scope.showSellingProductSelector)
                ownerId = $scope.selectedSellingProduct.SellingProductId;
            else if ($scope.showCarrierAccountSelector)
                ownerId = $scope.selectedCustomer.CarrierAccountId;

            return ownerId;
        }
        function getCurrencyId() {
            return (ownerTypeSelectorAPI.getSelectedIds() == WhS_BE_SalePriceListOwnerTypeEnum.SellingProduct.value) ? systemCurrencyId : currencySelectorAPI.getSelectedIds();
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
    }

    appControllers.controller("WhS_Sales_RatePlanController", RatePlanController);

})(appControllers);