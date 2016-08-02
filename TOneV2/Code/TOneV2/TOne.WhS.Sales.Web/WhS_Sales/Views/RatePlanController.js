(function (appControllers) {

    "use strict";

    RatePlanController.$inject = [
        "$scope",
        "WhS_Sales_RatePlanService",
        "WhS_Sales_RatePlanAPIService",
        "WhS_BE_SalePriceListOwnerTypeEnum",
        "WhS_Sales_RatePlanStatusEnum",
        'BusinessProcess_BPInstanceAPIService',
        'BusinessProcess_BPInstanceService',
        'WhS_BP_CreateProcessResultEnum',
        'VRCommon_CurrencyAPIService',
        "UtilsService",
        "VRUIUtilsService",
        "VRNotificationService"
    ];

    function RatePlanController($scope, WhS_Sales_RatePlanService, WhS_Sales_RatePlanAPIService, WhS_BE_SalePriceListOwnerTypeEnum, WhS_Sales_RatePlanStatusEnum, BusinessProcess_BPInstanceAPIService, BusinessProcess_BPInstanceService, WhS_BP_CreateProcessResultEnum, VRCommon_CurrencyAPIService, UtilsService, VRUIUtilsService, VRNotificationService) {

        var ownerTypeSelectorAPI;
        var ownerTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var sellingProductSelectorAPI;
        var sellingProductSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var carrierAccountSelectorAPI;
        var carrierAccountSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var databaseSelectorAPI;
        var databaseSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var policySelectorAPI;

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
            $scope.onCarrierAccountChanged = function () {
                resetRatePlan();

                var selectedId = carrierAccountSelectorAPI.getSelectedIds();

                if (selectedId != undefined) {
                    $scope.isLoadingFilterSection = true;

                    WhS_Sales_RatePlanAPIService.ValidateCustomer(selectedId, new Date()).then(function (isCustomerValid) {
                        if (!isCustomerValid) {
                            VRNotificationService.showInformation($scope.selectedCustomer.Name + " is not assigned to a selling product");
                            $scope.selectedCustomer = undefined;
                        }
                    }).catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    }).finally(function () {
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

            $scope.defaultItemTabs = [{
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
                            RateCalculationMethod: pricingSettings ? pricingSettings.selectedRateCalculationMethodData : null
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
            loadAllControls();

            function loadAllControls() {
                $scope.isLoadingFilterSection = true;

                UtilsService.waitMultipleAsyncOperations([loadOwnerFilterSection, loadRouteOptionsFilterSection, loadRatePlanSettingsData]).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                }).finally(function () {
                    $scope.isLoadingFilterSection = false;
                });

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
            }
        }

        function loadRatePlan()
        {
            var promises = [];

            var ownerTypeValue = ownerTypeSelectorAPI.getSelectedIds();

            var isLoadingZoneLetters = true;
            var zoneLettersGetPromise = getZoneLetters();
            promises.push(zoneLettersGetPromise);

            var loadGridDeferred = UtilsService.createPromiseDeferred();
            promises.push(loadGridDeferred.promise);

            var checkIfDraftExistsPromise = checkIfDraftExists();
            promises.push(checkIfDraftExistsPromise);

            zoneLettersGetPromise.then(function () {
                if ($scope.zoneLetters.length > 0)
                {
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
                return WhS_Sales_RatePlanAPIService.GetDefaultItem(ownerTypeValue, getOwnerId());
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
                    Settings: ratePlanSettingsData
                };
            }

            return gridLoadDeferred.promise;
        }

        function loadRatePlanSettingsData() {
            return WhS_Sales_RatePlanAPIService.GetRatePlanSettingsData().then(function (response) {
                ratePlanSettingsData = response;
            });
        }

        function onDefaultItemChange() {
            saveChanges(true);
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

            function getSaveChangesInput() {
                var defaultChanges = getDefaultChanges();
                var zoneChanges = gridAPI.getZoneChanges();

                return {
                    OwnerType: ownerTypeSelectorAPI.getSelectedIds(),
                    OwnerId: getOwnerId(),
                    NewChanges: (defaultChanges != null || zoneChanges != null) ? { DefaultChanges: defaultChanges, ZoneChanges: zoneChanges } : null
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
                clicked: function () {
                    return saveChanges(false).then(function () {
                        VRNotificationService.showSuccess("Draft saved");
                    });
                }
            }, {
                name: "Pricelist",
                clicked: savePricelist
            }];

            function savePricelist()
            {
                var promises = [];
                var systemCurrencyId;

                var saveChangesPromise = saveChanges(false);
                promises.push(saveChangesPromise);

                var getSystemCurrencyIdPromise = getSystemCurrencyId();
                promises.push(getSystemCurrencyIdPromise);

                var createProcessDeferred = UtilsService.createPromiseDeferred();
                promises.push(createProcessDeferred.promise);

                UtilsService.waitMultiplePromises([saveChangesPromise, getSystemCurrencyIdPromise]).then(function () {
                    var inputArguments = {
                        $type: 'TOne.WhS.Sales.BP.Arguments.RatePlanInput, TOne.WhS.Sales.BP.Arguments',
                        OwnerType: ownerTypeSelectorAPI.getSelectedIds(),
                        OwnerId: getOwnerId(),
                        CurrencyId: systemCurrencyId,
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
                
                function getSystemCurrencyId() {
                    return VRCommon_CurrencyAPIService.GetSystemCurrencyId().then(function (response) {
                        systemCurrencyId = response;
                    });
                }

                return UtilsService.waitMultiplePromises(promises);
            }
        }

        function getOwnerId() {
            var ownerId = null;

            if ($scope.showSellingProductSelector)
                ownerId = $scope.selectedSellingProduct.SellingProductId;
            else if ($scope.showCarrierAccountSelector)
                ownerId = $scope.selectedCustomer.CarrierAccountId;

            return ownerId;
        }
    }

    appControllers.controller("WhS_Sales_RatePlanController", RatePlanController);

})(appControllers);