﻿(function (appControllers) {

    "use strict";

    RatePlanController.$inject = [
        "$scope",
        "WhS_Sales_RatePlanService",
        "WhS_Sales_RatePlanAPIService",
        "WhS_Sales_SalePriceListOwnerTypeEnum",
        "WhS_Sales_RatePlanStatusEnum",
        "UtilsService",
        "VRUIUtilsService",
        "VRNotificationService"
    ];

    function RatePlanController($scope, WhS_Sales_RatePlanService, WhS_Sales_RatePlanAPIService, WhS_Sales_SalePriceListOwnerTypeEnum, WhS_Sales_RatePlanStatusEnum, UtilsService, VRUIUtilsService, VRNotificationService) {

        var sellingProductSelectorAPI;
        var sellingProductSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var carrierAccountSelectorAPI;
        var carrierAccountSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var databaseSelectorAPI;
        var databaseSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var policySelectorAPI;
        var policySelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var defaultItem;
        var defaultItemAPI;
        var defaultItemReadyDeferred = UtilsService.createPromiseDeferred();

        var gridAPI;
        var gridReadyDeferred = UtilsService.createPromiseDeferred();

        var settings;
        var pricingSettings;
        var isSavingPriceList; // This flag var prevents the app from saving an empty state after the user saves a price list

        defineScope();
        load();

        function defineScope() {
            $scope.ownerTypes = UtilsService.getArrayEnum(WhS_Sales_SalePriceListOwnerTypeEnum);
            $scope.selectedOwnerType = $scope.ownerTypes[0];
            $scope.showSellingProductSelector = true;
            $scope.showCarrierAccountSelector = false;

            $scope.onOwnerTypeChanged = function () {
                clearRatePlan();

                if ($scope.selectedOwnerType != undefined) {
                    if ($scope.selectedOwnerType.value == WhS_Sales_SalePriceListOwnerTypeEnum.SellingProduct.value) {
                        $scope.showSellingProductSelector = true;
                        $scope.showCarrierAccountSelector = false;
                        $scope.selectedCustomer = undefined;
                    }
                    else if ($scope.selectedOwnerType.value == WhS_Sales_SalePriceListOwnerTypeEnum.Customer.value) {
                        $scope.showSellingProductSelector = false;
                        $scope.showCarrierAccountSelector = true;
                        $scope.selectedSellingProduct = undefined;
                    }
                }
            };

            $scope.selectedSellingProduct = undefined;
            $scope.onSellingProductSelectorReady = function (api) {
                sellingProductSelectorAPI = api;
                sellingProductSelectorReadyDeferred.resolve();
            };
            $scope.onSellingProductChanged = function () {
                clearRatePlan();
            };

            $scope.selectedCustomer = undefined;
            $scope.onCarrierAccountSelectorReady = function (api) {
                carrierAccountSelectorAPI = api;
                carrierAccountSelectorReadyDeferred.resolve();
            };
            $scope.onCarrierAccountChanged = function () {
                if ($scope.selectedCustomer) {
                    clearRatePlan();

                    $scope.isLoadingFilterSection = true;

                    WhS_Sales_RatePlanAPIService.ValidateCustomer(getOwnerId(), new Date()).then(function (isCustomerValid) {
                        if (!isCustomerValid) {
                            VRNotificationService.showInformation($scope.selectedCustomer.Name + " is not related to a selling product");
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

            $scope.onRoutingDatabaseSelectorChange = function (item) {

                var payload;

                if (item != undefined && item.Information != undefined)
                    payload = {
                        filteredIds: item.Information.SelectedPoliciesIds,
                        selectedId: item.Information.DefaultPolicyId
                    };
                else {
                    payload = {
                        filteredIds: []
                    };
                }
                policySelectorAPI.load(payload);
            }

            $scope.onPolicySelectorReady = function (api) {
                policySelectorAPI = api;
                policySelectorReadyDeferred.resolve();
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

            $scope.onDefaultItemReady = function (api) {
                defaultItemAPI = api;
                defaultItemReadyDeferred.resolve();
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
            $scope.editSettings = function () {
                var onSettingsUpdate = function (updatedSettings) {
                    if (updatedSettings) {
                        settings = {};
                        settings.CostCalculationMethods = [];

                        if (updatedSettings.CostCalculationMethods) {
                            for (var i = 0; i < updatedSettings.CostCalculationMethods.length; i++)
                                settings.CostCalculationMethods.push(updatedSettings.CostCalculationMethods[i]);
                        }
                    }

                    $scope.showApplyButton = false;
                    VRNotificationService.showSuccess("Settings saved");
                    pricingSettings = null;
                    loadGrid();
                };
                WhS_Sales_RatePlanService.editSettings(settings, onSettingsUpdate);
            };
            $scope.editPricingSettings = function () {
                var onPricingSettingsUpdated = function (updatedPricingSettings) {
                    pricingSettings = updatedPricingSettings;

                    $scope.showApplyButton = true;
                    VRNotificationService.showSuccess("Pricing settings saved");
                    loadGrid();
                };
                WhS_Sales_RatePlanService.editPricingSettings(settings, pricingSettings, onPricingSettingsUpdated);
            };
            $scope.applyCalculatedRates = function () {
                var confirmPromise = VRNotificationService.showConfirmation("Are you sure you want to apply the calculated rates?");

                confirmPromise.then(function (confirmed) {
                    if (confirmed) {
                        $scope.showApplyButton = false;
                        

                        var input = {
                            OwnerType: $scope.selectedOwnerType.value,
                            OwnerId: getOwnerId(),
                            EffectiveOn: new Date(),
                            RoutingDatabaseId: databaseSelectorAPI ? databaseSelectorAPI.getSelectedIds() : null,
                            PolicyConfigId: policySelectorAPI ? policySelectorAPI.getSelectedIds() : null,
                            NumberOfOptions: $scope.numberOfOptions,
                            CostCalculationMethods: settings ? settings.CostCalculationMethods : null,
                            SelectedCostCalculationMethodConfigId: pricingSettings ? pricingSettings.selectedCostColumn.ConfigId : null,
                            RateCalculationMethod: pricingSettings ? pricingSettings.selectedRateCalculationMethodData : null
                        };

                        return WhS_Sales_RatePlanAPIService.ApplyCalculatedRates(input).then(function () {
                            VRNotificationService.showSuccess("Rates applied");
                            pricingSettings = null;
                            $scope.showRemoveButton = true;
                            loadGrid();
                        }).catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
            };
            $scope.deleteDraft = function () {
                return WhS_Sales_RatePlanAPIService.DeleteDraft($scope.selectedOwnerType.value, getOwnerId()).then(function (response) {
                    if (response) {
                        VRNotificationService.showSuccess("Draft deleted");
                        $scope.showRemoveButton = false;
                        loadRatePlan();
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
            };
            
            // Validation functions
            $scope.disableSaveButton = function () {
                return !validateRatePlan();
            };
            $scope.disableSettingsButton = function () {
                return !validateRatePlan();
            };
            $scope.disablePricingButton = function () {
                return !validateRatePlan();
            };
            
            defineSaveButtonMenuActions();
        }

        function load() {
            loadAllControls();

            function loadAllControls() {
                $scope.isLoadingFilterSection = true;

                UtilsService.waitMultipleAsyncOperations([loadOwnerFilterSection, loadRouteOptionsFilterSection]).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                }).finally(function () {
                    $scope.isLoadingFilterSection = false;
                });

                function loadOwnerFilterSection() {
                    var promises = [];

                    var sellingProductSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    promises.push(sellingProductSelectorLoadDeferred.promise);

                    sellingProductSelectorReadyDeferred.promise.then(function () {
                        var payload = { filter: null, selectedIds: null };
                        VRUIUtilsService.callDirectiveLoad(sellingProductSelectorAPI, payload, sellingProductSelectorLoadDeferred);
                    });

                    var carrierAccountSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    promises.push(carrierAccountSelectorLoadDeferred.promise);

                    carrierAccountSelectorReadyDeferred.promise.then(function () {
                        var payload = { filter: null, selectedIds: null };
                        VRUIUtilsService.callDirectiveLoad(carrierAccountSelectorAPI, payload, carrierAccountSelectorLoadDeferred);
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

                    var policySelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    promises.push(policySelectorLoadDeferred.promise);

                    policySelectorReadyDeferred.promise.then(function () {
                        VRUIUtilsService.callDirectiveLoad(policySelectorAPI, undefined, policySelectorLoadDeferred);
                    });

                    return UtilsService.waitMultiplePromises(promises);
                }
            }
        }

        function loadRatePlan() {
            var promises = [];

            var isLoadingZoneLetters = true;
            var zoneLettersGetPromise = getZoneLetters();
            promises.push(zoneLettersGetPromise);

            var loadGridDeferred = UtilsService.createPromiseDeferred();
            promises.push(loadGridDeferred.promise);

            loadGridDeferred.promise;

            var checkIfDraftExistsPromise = checkIfDraftExists();
            promises.push(checkIfDraftExistsPromise);

            zoneLettersGetPromise.then(function () {
                if ($scope.zoneLetters.length > 0) {
                    loadGrid().then(function () {
                        loadGridDeferred.resolve();
                        showRatePlan(true); // At this point, there's no guarantee that the default item has loaded. But that's okay since the tab directive displays a loader for the default item
                    }).catch(function (error) { loadGridDeferred.reject(); });
                }
                else {
                    loadGridDeferred.resolve();
                    VRNotificationService.showInformation("No countries were sold to this customer");
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
                    defaultItem.OwnerType = $scope.selectedOwnerType.value;
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
                return WhS_Sales_RatePlanAPIService.CheckIfDraftExists($scope.selectedOwnerType.value, getOwnerId()).then(function (response) {
                    if (response) {
                        $scope.showRemoveButton = true;
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope); // The user can perform other tasks if CheckIfDraftExists fails
                });
            }

            function getZoneLetters() {
                return WhS_Sales_RatePlanAPIService.GetZoneLetters($scope.selectedOwnerType.value, getOwnerId()).then(function (response) {
                    if (response) {
                        $scope.zoneLetters.length = 0;

                        for (var i = 0; i < response.length; i++) {
                            $scope.zoneLetters.push(response[i]);
                        }
                    }
                });
            }

            function getDefaultItem() {
                return WhS_Sales_RatePlanAPIService.GetDefaultItem($scope.selectedOwnerType.value, getOwnerId());
            }
        }

        function loadGrid() {
            var gridLoadDeferred = UtilsService.createPromiseDeferred();

            gridReadyDeferred.promise.then(function () {
                var gridQuery = getGridQuery();
                VRUIUtilsService.callDirectiveLoad(gridAPI, gridQuery, gridLoadDeferred);
            });

            gridLoadDeferred.promise.catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });

            return gridLoadDeferred.promise;

            function getGridQuery() {
                return {
                    OwnerType: $scope.selectedOwnerType.value,
                    OwnerId: getOwnerId(),
                    ZoneLetter: $scope.zoneLetters[$scope.connector.selectedZoneLetterIndex],
                    RoutingDatabaseId: databaseSelectorAPI.getSelectedIds(),
                    PolicyConfigId: policySelectorAPI.getSelectedIds(),
                    NumberOfOptions: $scope.numberOfOptions,
                    CostCalculationMethods: settings ? settings.CostCalculationMethods : null,
                    CostCalculationMethodConfigId: pricingSettings ? pricingSettings.selectedCostColumn.ConfigId : null,
                    RateCalculationMethod: pricingSettings ? pricingSettings.selectedRateCalculationMethodData : null
                };
            }
        }

        function onDefaultItemChange() {
            if (!isSavingPriceList)
                saveChanges(true);
            else
                isSavingPriceList = false;
        }

        function saveChanges(shouldLoadGrid) {
            var input = getSaveChangesInput();

            return WhS_Sales_RatePlanAPIService.SaveChanges(input).then(function (response) {
                if (shouldLoadGrid)
                    loadGrid();
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });

            function getSaveChangesInput() {
                var changes = null;

                var defaultChanges = getDefaultChanges();
                var zoneChanges = gridAPI.getZoneChanges();

                return {
                    OwnerType: $scope.selectedOwnerType.value,
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
        }

        function clearRatePlan() {
            $scope.zoneLetters.length=0;
            $scope.connector.selectedZoneLetterIndex = 0;
            showRatePlan(false);
        }

        function showRatePlan(show) {
            $scope.showZoneLetters = show;
            $scope.showDefaultItem = show;
            $scope.showGrid = show;
        }

        function defineSaveButtonMenuActions() {
            $scope.saveButtonMenuActions = [{
                name: "Price List",
                clicked: onSavePriceListClick
            }, {
                name: "Draft",
                clicked: function () {
                    return saveChanges(false).then(function () {
                        VRNotificationService.showSuccess("Draft saved");
                    });
                }
            }];

            function onSavePriceListClick() {
                var promises = [];

                var saveChangesPromise = saveChanges(false);
                promises.push(saveChangesPromise);

                saveChangesPromise.then(function () {
                    WhS_Sales_RatePlanService.viewChanges($scope.selectedOwnerType.value, getOwnerId(), onRatePlanChangesClose);
                });

                var savePriceListDeferred = UtilsService.createPromiseDeferred();
                promises.push(savePriceListDeferred.promise);

                savePriceListDeferred.promise.then(function (saveClicked) {
                    if (saveClicked) {
                        VRNotificationService.showSuccess("Price list saved");
                        loadRatePlan();
                    }
                });

                return UtilsService.waitMultiplePromises(promises).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });

                function onRatePlanChangesClose(saveClicked) {
                    if (saveClicked) {
                        isSavingPriceList = true;
                        savePriceList().then(function () {
                            savePriceListDeferred.resolve(saveClicked);
                        }).catch(function (error) { savePriceListDeferred.reject(); });
                    }
                    else savePriceListDeferred.resolve(saveClicked);
                }

                function savePriceList() {
                    return WhS_Sales_RatePlanAPIService.SavePriceList($scope.selectedOwnerType.value, getOwnerId());
                }
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

        function validateRatePlan() {
            return ($scope.zoneLetters && $scope.zoneLetters.length > 0);
        }
    }

    appControllers.controller("WhS_Sales_RatePlanController", RatePlanController);

})(appControllers);
