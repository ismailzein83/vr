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

        var defaultItem;

        var gridAPI;
        var gridReadyDeferred = UtilsService.createPromiseDeferred();

        var settings;
        var pricingSettings;
        var ratePlanSettingsData;

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
                    if (isConfirmed) {
                        WhS_Sales_RatePlanAPIService.DeleteChangedRates(ownerTypeSelectorAPI.getSelectedIds(), getOwnerId(), selectedId).then(function () {
                            draftCurrencyId = selectedId;
                            loadGrid();
                        });
                    }
                    else { currencySelectorAPI.selectedCurrency(draftCurrencyId); }
                });
            };

            $scope.defaultItemTabs = [{
                title: "Default Services",
                directive: "vr-whs-sales-default-service",
                loadDirective: function (api) {
                    defaultItem.onChange = onDefaultItemChange;

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
                    defaultItem.onChange = onDefaultItemChange;
                    return api.load(defaultItem);
                }
            }];

            $scope.zoneLetters = [];
            $scope.connector = {};
            $scope.connector.selectedZoneLetterIndex = 0;

            $scope.onZoneLetterSelectionChanged = function () {
                return saveChanges(true);
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
                var onCountriesSold = function (customerZones) {
                    $scope.connector.selectedZoneLetterIndex = 0;

                    if (databaseSelectorAPI.getSelectedIds() != null && policySelectorAPI.getSelectedIds() != null)
                        loadRatePlan();
                };

                WhS_Sales_RatePlanService.sellNewCountries(customerId, onCountriesSold);
            };
            $scope.editSettings = function ()
            {
                var onSettingsUpdated = function (updatedSettings)
                {
                    if (updatedSettings != undefined)
                    {
                        settings = {};
                        if (updatedSettings.costCalculationMethods != undefined)
                        {
                            settings.costCalculationMethods = [];
                            for (var i = 0; i < updatedSettings.costCalculationMethods.length; i++) {
                                settings.costCalculationMethods.push(updatedSettings.costCalculationMethods[i]);
                            }
                        }
                        $scope.showPricingButton = (settings.costCalculationMethods != undefined);
                    }

                    $scope.showApplyButton = false;
                    pricingSettings = null;

                    VRNotificationService.showSuccess("Settings saved");

                    loadGrid().catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
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

                    loadGrid().catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
                };
                WhS_Sales_RatePlanService.editPricingSettings(settings, pricingSettings, onPricingSettingsUpdated);
            };
            $scope.applyCalculatedRates = function ()
            {
                var promises = [];

                var confirmPromise = VRNotificationService.showConfirmation("Are you sure you want to apply the calculated rates?");
                promises.push(confirmPromise);

                var applyDeferred = UtilsService.createPromiseDeferred();
                promises.push(applyDeferred.promise);

                confirmPromise.then(function (confirmed) {
                    if (confirmed) {
                        $scope.showApplyButton = false;

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
                            CurrencyId: getCurrencyId()
                        };

                        WhS_Sales_RatePlanAPIService.ApplyCalculatedRates(input).then(function ()
                        {
                            applyDeferred.resolve();
                            VRNotificationService.showSuccess("Rates applied");
                            pricingSettings = null;
                            $scope.showCancelButton = true;
                            
                            loadGrid().catch(function (error) {
                                VRNotificationService.notifyException(error, $scope);
                            });

                        }).catch(function (error) {
                            applyDeferred.reject(error);
                            VRNotificationService.notifyException(error, $scope);
                        });
                    }
                    else {
                        applyDeferred.resolve();
                    }
                });

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
            return UtilsService.waitMultipleAsyncOperations([loadOwnerFilterSection, loadRouteOptionsFilterSection, loadCurrencySelector, loadRatePlanSettingsData, getSystemCurrencyId]).catch(function (error) {
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
        function getSystemCurrencyId() {
            return VRCommon_CurrencyAPIService.GetSystemCurrencyId().then(function (response) {
                systemCurrencyId = response;
            });
        }

        function loadRatePlan()
        {
            $scope.isLoadingRatePlan = true;
            var promises = [];

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

                    loadGrid().then(function () {
                        loadGridDeferred.resolve();
                        showRatePlan(true); // At this point, there's no guarantee that the default item has loaded. But that's okay since the tab directive displays a loader for the default item
                    }).catch(function (error) { loadGridDeferred.reject(error); });
                }
                else {
                    loadGridDeferred.resolve();

                    if (ownerTypeValue == WhS_BE_SalePriceListOwnerTypeEnum.SellingProduct.value)
                        VRNotificationService.showInformation('No effective zones exist for this selling product');
                    else
                        VRNotificationService.showInformation("No countries are sold to this customer or no effective zones exist for its assigned selling product");
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
                return WhS_Sales_RatePlanAPIService.GetZoneLetters(ownerTypeValue, getOwnerId()).then(function (response) {
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
        function loadGrid()
        {
            var gridLoadDeferred = UtilsService.createPromiseDeferred();
            
            var gridQuery = getGridQuery();
            VRUIUtilsService.callDirectiveLoad(gridAPI, gridQuery, gridLoadDeferred);

            function getGridQuery() {
                return {
                    OwnerType: ownerTypeSelectorAPI.getSelectedIds(),
                    OwnerId: getOwnerId(),
                    ZoneLetter: $scope.zoneLetters[$scope.connector.selectedZoneLetterIndex],
                    RoutingDatabaseId: databaseSelectorAPI.getSelectedIds(),
                    PolicyConfigId: policySelectorAPI.getSelectedIds(),
                    NumberOfOptions: $scope.numberOfOptions,
                    CostCalculationMethods: settings ? settings.costCalculationMethods : null,
                    CostCalculationMethodConfigId: pricingSettings ? pricingSettings.selectedCostColumn.ConfigId : null,
                    RateCalculationMethod: pricingSettings ? pricingSettings.selectedRateCalculationMethodData : null,
                    Settings: ratePlanSettingsData,
                    CurrencyId: getCurrencyId(),
                    onNewZoneServiceChanged: onNewZoneServiceChanged
                };
            }

            return gridLoadDeferred.promise;
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
        
        function saveChanges(shouldLoadGrid)
        {
            var promises = [];

            var input = getSaveChangesInput();

            if (input.NewChanges != null)
            {
                var saveChangesPromise = WhS_Sales_RatePlanAPIService.SaveChanges(input);
                promises.push(saveChangesPromise);

                saveChangesPromise.then(function ()
                {
                    $scope.showCancelButton = true;
                    loadGridOnChangesSaved();
                });
            }
            else
                loadGridOnChangesSaved();

            function getSaveChangesInput()
            {
                var defaultChanges = getDefaultChanges();
                var zoneChanges = gridAPI.getZoneChanges();

                var newChanges = null;
                if (defaultChanges != null || zoneChanges != null) {
                    newChanges = {
                        CurrencyId: getCurrencyId(),
                        DefaultChanges: defaultChanges,
                        ZoneChanges: zoneChanges
                    };
                }
                
                return {
                    OwnerType: ownerTypeSelectorAPI.getSelectedIds(),
                    OwnerId: getOwnerId(),
                    NewChanges: newChanges
                };

                function getDefaultChanges() {
                    var defaultChanges = null;

                    if (defaultItem.IsDirty) {
                        defaultChanges = {};

                        for (var i = 0; i < $scope.defaultItemTabs.length; i++) {
                            var item = $scope.defaultItemTabs[i];

                            if (item.directiveAPI)
                                item.directiveAPI.applyChanges(defaultChanges);
                        }

                        defaultChanges.NewService = defaultItem.NewService;
                        defaultChanges.ClosedService = defaultItem.ClosedService;
                        defaultChanges.ResetService = defaultItem.ResetService;
                    }

                    return defaultChanges;
                }
            }
            function loadGridOnChangesSaved() {
                if (shouldLoadGrid) {
                    loadGrid().catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
                }
            }

            return UtilsService.waitMultiplePromises(promises);
        }
        function onDefaultItemChange() {
            saveChanges(true);
        }
        function onNewZoneServiceChanged() {
            saveChanges(false);
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
        }

        function defineSaveButtonMenuActions() {
            $scope.saveButtonMenuActions = [{
                name: "Draft",
                clicked: saveDraft
            }, {
                name: "Apply Changes",
                clicked: applyChanges
            }];
        }
        function saveDraft() {
            return saveChanges(false).then(function () {
                VRNotificationService.showSuccess("Draft saved");
            });
        }
        function applyChanges()
        {
            var promises = [];

            var saveChangesPromise = saveChanges(false);
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
    }

    appControllers.controller("WhS_Sales_RatePlanController", RatePlanController);

})(appControllers);