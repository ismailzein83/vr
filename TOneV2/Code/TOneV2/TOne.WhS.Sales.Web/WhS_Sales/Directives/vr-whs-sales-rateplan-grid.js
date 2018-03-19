"use strict";

app.directive("vrWhsSalesRateplanGrid", ["WhS_Sales_RatePlanAPIService", "UtilsService", "VRUIUtilsService", "VRNotificationService", "VRValidationService", "VRCommon_RateTypeAPIService", "WhS_Sales_RatePlanUtilsService", "WhS_Sales_RatePlanService", "WhS_BE_SalePriceListOwnerTypeEnum", "WhS_BE_PrimarySaleEntityEnum", "UISettingsService", "VRDateTimeService", 'VRCommon_TextFilterTypeEnum', 'VRLocalizationService', 'WhS_Sales_SupplierStatusEnum',
    function (WhS_Sales_RatePlanAPIService, UtilsService, VRUIUtilsService, VRNotificationService, VRValidationService, VRCommon_RateTypeAPIService, WhS_Sales_RatePlanUtilsService, WhS_Sales_RatePlanService, WhS_BE_SalePriceListOwnerTypeEnum, WhS_BE_PrimarySaleEntityEnum, UISettingsService, VRDateTimeService, VRCommon_TextFilterTypeEnum, VRLocalizationService, WhS_Sales_SupplierStatusEnum) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                ispreview: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                if ($attrs.ispreview != undefined) {
                    UtilsService.setContextReadOnly($scope);
                    ctrl.maxHeight = (window.innerHeight - 337) + 'px';
                } else {
                    var sh = innerHeight;
                    var h = 28;
                    h += sh - 332;

                    ctrl.maxHeight = (h < 30 ? 30 : h) + 'px';
                }

                var ratePlanGrid = new RatePlanGrid($scope, ctrl, $attrs, $element);
                ratePlanGrid.initializeController();
            },
            controllerAs: "ratePlanGridCtrl",
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_Sales/Directives/Templates/RatePlanGridTemplate.html"
        };

        function RatePlanGrid($scope, ctrl, attrs, $element) {
            this.initializeController = initializeController;

            var gridAPI;
            var gridQuery;
            var gridDrillDownDefinitions;

            var isLoadingMoreData;
            var stopPagingOnScroll;

            var routingProductTab;
            var otherRatesTab;

            var newRateDayOffset = 0;
            var increasedRateDayOffset = 0;
            var decreasedRateDayOffset = 0;

            var ownerName;

            var columnsConfig;
            var allZoneItems = [];
            var filteredZoneItems = [];

            var sumFixedWidthes;
            var sumWidthFactors;

            var allZonesLetter = "ALL ZONES";

            function initializeController() {

                $scope.scopeModel = {};
                $scope.zoneLetters = [];
                $scope.scopeModel.selectedZoneLetterIndex = 0;
                $scope.scopeModel.isPreview = (attrs.ispreview != undefined);

                $scope.normalPrecision = UISettingsService.getNormalPrecision();
                $scope.longPrecision = UISettingsService.getLongPrecision();
                $scope.layoutOption = UISettingsService.getGridLayoutOptions();

                $scope.onZoneLetterSelectionChanged = function () {
                    var promises = [];
                    $scope.isLoading = true;

                    var saveDraftDeferred = UtilsService.createPromiseDeferred();
                    promises.push(saveDraftDeferred.promise);

                    if (gridQuery.BulkAction != undefined)
                        saveDraftDeferred.resolve();
                    else {
                        gridQuery.context.saveDraft().then(function () {
                            saveDraftDeferred.resolve();
                        }).catch(function (error) {
                            saveDraftDeferred.reject(error);
                        });
                    }

                    var loadZoneItemsDeferred = UtilsService.createPromiseDeferred();
                    promises.push(loadZoneItemsDeferred.promise);

                    saveDraftDeferred.promise.then(function () {
                        //gridAPI.clearDataAndContinuePaging();
                        stopPagingOnScroll = false;
                        allZoneItems.length = 0;
                        filteredZoneItems.length = 0;
                        $scope.zoneItems.length = 0;
                        loadZoneItems().then(function () {
                            loadZoneItemsDeferred.resolve();
                        }).catch(function (error) {
                            loadZoneItemsDeferred.reject(error);
                        });
                    });

                    return UtilsService.waitMultiplePromises(promises).finally(function () {
                        $scope.isLoading = false;
                    });
                };
                $scope.isGridScope = true;
                $scope.zoneItems = [];
                $scope.costCalculationMethods = [];
                gridDrillDownDefinitions = getGridDrillDownDefinitions();

                $scope.onFilterRoutingProductSelectorReady = function (api) {
                    filterRoutingProductSelectorAPI = api;
                    filterRoutingProductSelectorReadyDeferred.resolve();
                };

                initGridFilter();
                defineScrollEvent();

                $scope.onZoneNameClicked = function (dataItem) {
                    var primarySaleEntity = (gridQuery.SaleAreaSettings != undefined) ? gridQuery.SaleAreaSettings.PrimarySaleEntity : undefined;
                    WhS_Sales_RatePlanService.viewZoneInfo(gridQuery.OwnerType, gridQuery.OwnerId, dataItem.ZoneId, dataItem.ZoneName, dataItem.ZoneBED, dataItem.ZoneEED, gridQuery.CurrencyId, dataItem.CountryId, primarySaleEntity);
                };

                $scope.onCurrentRateClicked = function (dataItem) {
                    WhS_Sales_RatePlanService.viewFutureRate(dataItem.ZoneName, dataItem.FutureNormalRate, gridQuery.SaleAreaSettings.PrimarySaleEntity, gridQuery.OwnerType);
                };

                $scope.onTQIClicked = function (dataItem) {
                    var onTQIEvaluated = function (evaluatedRate) {
                        if (evaluatedRate != undefined) {
                            dataItem.NewRate = evaluatedRate;
                            $scope.onNewRateBlurred(dataItem);
                        }
                    };

                    var context = {
                        zoneItem: dataItem,
                        ownerName: ownerName,
                        routingDatabaseId: gridQuery.RoutingDatabaseId,
                        currencyId: gridQuery.CurrencyId,
                        ratePlanSettings: gridQuery.Settings,
                        longPrecision: $scope.longPrecision
                    };
                    if (dataItem.RPRouteDetail != undefined && dataItem.RPRouteDetail.RouteOptionsDetails != undefined && dataItem.RPRouteDetail.RouteOptionsDetails.length > 0)
                        WhS_Sales_RatePlanService.openTQIEditor(context, onTQIEvaluated);
                    else
                        VRNotificationService.showInformation("TQI can not be calculated for zones having no route options");
                };

                $scope.onNewRateChanged = function (dataItem) {
                    dataItem.IsDirty = true;
                    dataItem.ProfitPerc = (dataItem.NewRate != null && dataItem.NewRate > 0 && dataItem.RPRouteDetail != null && dataItem.RPRouteDetail.RouteOptionsDetails != null) ? ((dataItem.NewRate - dataItem.RPRouteDetail.RouteOptionsDetails[0].ConvertedSupplierRate) * 100 / dataItem.RPRouteDetail.RouteOptionsDetails[0].ConvertedSupplierRate) : 0;
                    WhS_Sales_RatePlanUtilsService.onNewRateChanged(dataItem);
                };

                $scope.onNewRateBlurred = function (dataItem) {
                    dataItem.IsDirty = true;
                    var settings = {
                        newRateDayOffset: newRateDayOffset,
                        increasedRateDayOffset: increasedRateDayOffset,
                        decreasedRateDayOffset: decreasedRateDayOffset
                    };
                    WhS_Sales_RatePlanUtilsService.onNewRateBlurred(dataItem, settings);
                };

                $scope.onEffectiveRPClicked = function (dataItem) {
                    if (dataItem.EffectiveRoutingProductName != null && routingProductTab.setTabSelected != undefined)
                        routingProductTab.setTabSelected(dataItem);
                };

                $scope.onRowClicked = function (evnt) {
                    $('.vr-datagrid-row').removeClass('vr-datagrid-datacells-click');
                    $(evnt.target).closest('.vr-datagrid-row').addClass('vr-datagrid-datacells-click');
                };

                $scope.getRowStyle = function (dataItem) {
                    var rowStyle;
                    var rate; // The rate to validate

                    if (!WhS_Sales_RatePlanUtilsService.isStringEmpty(dataItem.NewRate))
                        rate = Number(dataItem.NewRate);
                    else if (dataItem.CurrentRate != null)
                        rate = dataItem.CurrentRate;

                    var routeOptions = getRouteOptions(dataItem.RPRouteDetail);
                    var routeOptionsForView = dataItem.RouteOptionsDetailsForView;
                    if (rate == undefined) {
                        setColorOfRouteOptions(routeOptions, null);
                        rowStyle = { CssClass: 'bg-success' };
                    }
                    else { // Validate the rate
                        if (routeOptionsForView != null && routeOptionsForView.length > 0) {
                            var array = []; // Stores the indexes of route options having a greater rate than the rate to validate

                            for (var i = 0; i < routeOptionsForView.length; i++) {
                                if (routeOptionsForView[i].ConvertedSupplierRate > rate && routeOptionsForView[i].SupplierStatus != WhS_Sales_SupplierStatusEnum.Block.value)
                                    array.push(i);
                            }

                            if (array.length == routeOptions.length) {
                                setColorOfRouteOptions(routeOptionsForView, null);
                                rowStyle = { CssClass: "bg-danger" };
                            }
                            else if (array.length > 0) {
                                for (var i = 0; i < routeOptionsForView.length; i++)
                                    routeOptionsForView[i].Color = (UtilsService.contains(array, i)) ? 'orange' : null;
                            }
                            else {
                                setColorOfRouteOptions(routeOptionsForView, null);
                            }
                        }
                    }

                    // Reload the route options directive to refresh the colors
                    if (dataItem.RouteOptionsAPI != undefined) {
                        var routeOptionsDirectivePayload = getRouteOptionsDirectivePayload(dataItem);
                        dataItem.RouteOptionsAPI.load(routeOptionsDirectivePayload);
                    }

                    return rowStyle;
                };

                $scope.expandRow = function (dataItem) {
                    dataItem.loadDrilldownTemplate = true;
                    dataItem.isRowExpanded = true;
                };

                $scope.collapseRow = function (dataItem) {
                    dataItem.isRowExpanded = false;
                };

                columnsConfig = {};
                $scope.columnsConfig = columnsConfig;
                $scope.addColumnFixedAndRecalculate = function (colName, width) {
                    var column = {
                        name: colName,
                        widthAsNb: width,
                        width: width + "px"
                    };
                    columnsConfig[colName] = column;
                    recalculateColumnWidthes();
                };
                $scope.addColumnWidthFactorAndRecalculate = function (colName, width) {
                    var column = {
                        name: colName,
                        widthFactor: width
                    };
                    columnsConfig[colName] = column;
                    recalculateColumnWidthes();
                };

                function recalculateColumnWidthes() {
                    sumFixedWidthes = 0;
                    sumWidthFactors = 0;
                    for (var colName in columnsConfig) {
                        var column = columnsConfig[colName];
                        if (column.widthFactor != undefined)
                            sumWidthFactors += column.widthFactor;
                        else
                            sumFixedWidthes += column.widthAsNb;
                    }

                    for (var colName in columnsConfig) {
                        var column = columnsConfig[colName];
                        if (column.widthFactor != undefined) {
                            column.width = "calc(" + (100 * column.widthFactor / sumWidthFactors) + "% - " + (sumFixedWidthes * (100 * column.widthFactor / sumWidthFactors) / 100) + "px)";
                        }
                    }
                }

                function setColorOfRouteOptions(routeOptions, colorValue) {
                    if (routeOptions != null)
                        for (var i = 0; i < routeOptions.length; i++)
                            routeOptions[i].Color = colorValue;
                }

                $scope.filterZones = function () {
                    lastTriggerFilterZonesTime = VRDateTimeService.getNowDateTime();
                    triggerfilterZones();
                };
                $scope.onNewRateFilterOptionChanged = function () {
                    return onNewRateFilterOptionChanged();
                };
                $scope.resetGridFilter = function () {
                    resetGridFilter();
                    return $scope.filterZones();
                };
                $scope.sortZones = function (col) {
                    sortZones(col);
                };

                UtilsService.waitMultiplePromises([filterRoutingProductSelectorReadyDeferred.promise]).then(function () {
                    defineAPI();
                });
            }

            function setColumnsWidthesInCSS() {
                //for (var colName in columnsConfig) {
                //    var column = columnsConfig[colName];
                //    if (column.widthFactor != undefined) {
                //        column.width = "calc(" + (100 * column.widthFactor / sumWidthFactors) + "% - " + (sumFixedWidthes * (100 * column.widthFactor / sumWidthFactors) / 100) + "px)";
                //    }
                //}
            }

            function getGridDrillDownDefinitions() {
                routingProductTab = {
                    title: "Routing Product",
                    directive: "vr-whs-sales-routingproduct-zone",
                    loadDirective: function (zoneRoutingProductDirectiveAPI, zoneItem) {
                        var payload = { zoneItem: zoneItem };
                        return zoneRoutingProductDirectiveAPI.load(payload);
                    }
                };
                otherRatesTab = {
                    title: "Other Rates",
                    directive: "vr-whs-sales-otherrate-grid",
                    loadDirective: function (rateTypeGridAPI, zoneItem) {
                        var query = {
                            zoneItem: zoneItem,
                            settings: {
                                newRateDayOffset: newRateDayOffset,
                                increasedRateDayOffset: increasedRateDayOffset,
                                decreasedRateDayOffset: decreasedRateDayOffset
                            },
                            ownerType: gridQuery.OwnerType,
                            ownerCurrencyId: gridQuery.CurrencyId,
                            saleAreaSetting: gridQuery.SaleAreaSettings
                        };
                        return rateTypeGridAPI.loadGrid(query);
                    }
                };
                return [routingProductTab, otherRatesTab];
            }

            function defineAPI() {

                var api = {};

                api.load = function (query) {
                    gridQuery = query;

                    if (query != undefined) {
                        ownerName = query.OwnerName;
                        setCostCalculationMethods(query.CostCalculationMethods, query.RateCalculationMethod);
                    }

                    defineRouteOptionNumbers();
                    defineDefaultCostComparisonOptions();
                    resetGridFilter();

                    $scope.isCurrentRateSourceFilterHidden = (gridQuery.OwnerType == WhS_BE_SalePriceListOwnerTypeEnum.SellingProduct.value);

                    var promises = [];
                    $scope.isLoading = true;

                    var loadZoneLettersPromise = loadZoneLetters();
                    promises.push(loadZoneLettersPromise);

                    var loadZoneItemsDeferred = UtilsService.createPromiseDeferred();
                    promises.push(loadZoneItemsDeferred.promise);

                    var setDayOffsetsDeferred = setDayOffsets();
                    promises.push(setDayOffsetsDeferred);

                    var loadFilterRoutingProductSelectorPromise = loadFilterRoutingProductSelector();
                    promises.push(loadFilterRoutingProductSelectorPromise);

                    loadZoneLettersPromise.then(function () {
                        if ($scope.zoneLetters.length > 0) {
                            $scope.showDirective = true;
                            gridQuery.context.onZoneLettersLoaded();
                            showApplicableZonesTab(true);

                            allZoneItems.length = 0;
                            filteredZoneItems.length = 0;
                            $scope.zoneItems.length = 0;
                            //gridAPI.clearDataAndContinuePaging();
                            stopPagingOnScroll = false;
                            loadZoneItems().then(function () {
                                loadZoneItemsDeferred.resolve();
                            }).catch(function (error) {
                                loadZoneItemsDeferred.reject(error);
                            });
                        } else {
                            loadZoneItemsDeferred.resolve();
                            $scope.showDirective = false;

                            if (gridQuery.BulkAction != undefined) {
                                VRNotificationService.showInformation('No applicable zones exist');
                                showApplicableZonesTab(false);
                            }
                            else {
                                gridQuery.context.showRatePlan(false);

                                if (gridQuery.context.isFilterApplied())
                                    VRNotificationService.showInformation('No effective zones match the filter');
                                else {
                                    if (gridQuery.OwnerType == WhS_BE_SalePriceListOwnerTypeEnum.SellingProduct.value)
                                        VRNotificationService.showInformation('No effective zones exist for this selling product');
                                    else
                                        VRNotificationService.showInformation("No countries are sold to this customer or no effective zones exist for its assigned selling product");
                                }
                            }
                        }
                    });

                    function setCostCalculationMethods(costCalculationMethods, rateCalculationMethod) {
                        if (costCalculationMethods != null) {
                            $scope.costCalculationMethods = [];
                            for (var i = 0; i < costCalculationMethods.length; i++)
                                $scope.costCalculationMethods.push(costCalculationMethods[i]);
                        }
                        else {
                            $scope.costCalculationMethods.length = 0;
                        }
                        $scope.rateCalculationMethod = rateCalculationMethod;
                    }
                    function setDayOffsets() {
                        return WhS_Sales_RatePlanAPIService.GetPricingSettings(gridQuery.OwnerType, gridQuery.OwnerId).then(function (response) {
                            if (response == undefined)
                                return;
                            if (response.NewRateDayOffset != null) {
                                newRateDayOffset = Number(response.NewRateDayOffset);
                            }
                            if (response.IncreasedRateDayOffset != null) {
                                increasedRateDayOffset = Number(response.IncreasedRateDayOffset);
                            }
                            if (response.DecreasedRateDayOffset != null) {
                                decreasedRateDayOffset = Number(response.DecreasedRateDayOffset);
                            }
                        });
                    }
                    function loadZoneLetters() {

                        // Reset the zone letters
                        $scope.showZoneLetters = false;
                        $scope.zoneLetters.length = 0;
                        $scope.scopeModel.selectedZoneLetterIndex = 0;

                        var getZoneLettersInput =
                        {
                            OwnerType: gridQuery.OwnerType,
                            OwnerId: gridQuery.OwnerId,
                            EffectiveOn: UtilsService.getDateFromDateTime(VRDateTimeService.getNowDateTime()),
                            BulkAction: gridQuery.BulkAction
                        };

                        if (gridQuery.Filter != null) {
                            getZoneLettersInput.CountryIds = gridQuery.Filter.CountryIds;
                            getZoneLettersInput.ZoneNameFilterType = gridQuery.Filter.ZoneNameFilterType;
                            getZoneLettersInput.ZoneNameFilter = gridQuery.Filter.ZoneNameFilter;
                            getZoneLettersInput.BulkActionFilter = gridQuery.Filter.BulkActionFilter;
                            getZoneLettersInput.ExcludedZoneIds = gridQuery.Filter.ExcludedZoneIds;
                        }

                        return WhS_Sales_RatePlanAPIService.GetZoneLetters(getZoneLettersInput).then(function (response) {
                            if (response != undefined && response.length > 0) {
                                $scope.showZoneLetters = true;
                                for (var i = 0; i < response.length; i++) {
                                    $scope.zoneLetters.push(response[i]);
                                }
                                //this line will be added after optimizing grid performance
                                $scope.zoneLetters.push(allZonesLetter);
                            }
                        });
                    }
                    function loadFilterRoutingProductSelector() {
                        var filterRoutingProductLoadDeferred = UtilsService.createPromiseDeferred();
                        var filterRoutingProductSelectorPayload = {
                            filter: {
                                SellingNumberPlanId: gridQuery.ownerSellingNumberPlanId,
                                AssignableToOwnerType: gridQuery.OwnerType,
                                AssignableToOwnerId: gridQuery.OwnerId
                            }
                        };
                        VRUIUtilsService.callDirectiveLoad(filterRoutingProductSelectorAPI, filterRoutingProductSelectorPayload, filterRoutingProductLoadDeferred);
                        return filterRoutingProductLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises).finally(function () {
                        $scope.isLoading = false;
                    });
                };
                api.doesGridHasCustomerData = function()
                {
                    return ($scope.zoneLetters.length > 0 && gridQuery.OwnerType == WhS_BE_SalePriceListOwnerTypeEnum.Customer.value)
                }
                api.getZoneDrafts = function () {
                    var zoneDrafts = [];

                    for (var i = 0; i < allZoneItems.length; i++) {
                        var item = allZoneItems[i];

                        if (item.IsDirty)
                            applyChanges(zoneDrafts, item);
                    }

                    return zoneDrafts.length > 0 ? zoneDrafts : null;
                };

                api.clearDataSource = function () {
                    allZoneItems.length = 0;
                    filteredZoneItems.length = 0;
                    $scope.zoneItems.length = 0;
                    stopPagingOnScroll = false;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function loadZoneItems() {

                var promises = [];
                setGridQueryProperties();

                var getZoneItemsPromise = WhS_Sales_RatePlanAPIService.GetZoneItems(gridQuery);
                promises.push(getZoneItemsPromise);

                var loadDirectivesDeferred = UtilsService.createPromiseDeferred();
                promises.push(loadDirectivesDeferred.promise);

                getZoneItemsPromise.then(function (response) {

                    if (response != undefined) {
                        for (var i = 0; i < response.length; i++) {
                            var zoneItem = response[i];
                            zoneItem.isRowExpanded = false;
                            zoneItem.loadDrilldownTemplate = false;
                            extendZoneItem(zoneItem);
                            setDrillDownExtensionObject(zoneItem);

                            WhS_Sales_RatePlanUtilsService.onNewRateChanged(zoneItem);

                            allZoneItems.push(zoneItem);
                        }

                    }
                    clearSorting();
                    filterZones().then(function () {
                        loadDirectivesDeferred.resolve();
                    }).catch(function (error) {
                        loadDirectivesDeferred.reject(error);
                    });
                });

                function setGridQueryProperties() {
                    gridQuery.IncludeBlockedSuppliers = gridQuery.Settings.IncludeBlockedSuppliers;
                    //var pageInfo = gridAPI.getPageInfo();
                    if (gridQuery.Filter == undefined)
                        gridQuery.Filter = {};
                    gridQuery.Filter.FromRow = 1; //pageInfo.fromRow;
                    gridQuery.Filter.ToRow = 30000;//pageInfo.toRow;
                    var zoneLetter = $scope.zoneLetters[$scope.scopeModel.selectedZoneLetterIndex];
                    gridQuery.Filter.ZoneLetter = zoneLetter != allZonesLetter ? zoneLetter : null;
                }

                return UtilsService.waitMultiplePromises(promises);
            }

            function displayZoneItems() {
                var promises = [];
                for (var i = $scope.zoneItems.length; i < filteredZoneItems.length; i++) {
                    var zoneItem = filteredZoneItems[i];
                    zoneItem.displayRow = (i <= 30);
                    //promises.push(zoneItem.RouteOptionsLoadDeferred.promise);
                    //promises.push(zoneItem.serviceViewerLoadDeferred.promise);

                    $scope.zoneItems.push(zoneItem);
                }
                setTimeout(function () {
                    refreshHeaderWidth();
                    UtilsService.safeApply($scope);
                });
                return UtilsService.waitMultiplePromises(promises);
            }

            function extendZoneItem(zoneItem) {
                zoneItem.IsDirty = isRatePlanZoneDirty();
                zoneItem.OwnerType = gridQuery.OwnerType;
                zoneItem.isSellingProductZone = (zoneItem.OwnerType == WhS_BE_SalePriceListOwnerTypeEnum.SellingProduct.value);
                zoneItem.showRateChangeType = true;

                zoneItem.currentRateEED = zoneItem.CurrentRateEED; // Maintains the original value of zoneItem.CurrentRateEED in case the user deletes the new rate
                setRouteOptionProperties(zoneItem);
                setServiceViewerLoad(zoneItem);
                WhS_Sales_RatePlanUtilsService.setNormalRateIconProperties(zoneItem, gridQuery.OwnerType, gridQuery.SaleAreaSettings);
                setCurrencyIconProperties(zoneItem);


                zoneItem.IsCurrentRateEditable = (zoneItem.IsCurrentRateEditable == null) ? false : zoneItem.IsCurrentRateEditable;

                if (zoneItem.NewRate != null) {
                    zoneItem.IsDirty = true;

                    if (zoneItem.NewRateEED == null)
                        zoneItem.NewRateEED = zoneItem.ZoneEED;
                }
                else if (zoneItem.CurrentRate != null) {
                    var rateEED = zoneItem.CurrentRateEED;

                    if (zoneItem.CurrentRateNewEED != null) {
                        zoneItem.IsDirty = true;
                        rateEED = zoneItem.CurrentRateNewEED;
                    }

                    zoneItem.CurrentRateEED = (rateEED != null && compareDates(rateEED, zoneItem.ZoneEED) == 2) ? rateEED : zoneItem.ZoneEED;
                }
                else if (zoneItem.CurrentRate != null && zoneItem.CurrentRateEED == null) {
                    zoneItem.CurrentRateEED = zoneItem.ZoneEED;
                }

                zoneItem.validateNewRate = function () {
                    return WhS_Sales_RatePlanUtilsService.validateNewRate(zoneItem, gridQuery.CurrencyId);
                };

                zoneItem.onCurrentRateEEDChanged = function (zoneItem) {
                    zoneItem.IsDirty = true;
                };

                zoneItem.refreshZoneItem = function (zoneItem) {

                    var promises = [];

                    var saveChangesPromise = saveZoneItemChanges();
                    promises.push(saveChangesPromise);

                    var getZoneItemDeferred = UtilsService.createPromiseDeferred();
                    promises.push(getZoneItemDeferred.promise);

                    var loadEffectiveServicesDeferred = UtilsService.createPromiseDeferred();
                    promises.push(loadEffectiveServicesDeferred.promise);

                    var loadRouteOptionsDeferred = UtilsService.createPromiseDeferred();
                    promises.push(loadRouteOptionsDeferred.promise);

                    saveChangesPromise.then(function () {
                        WhS_Sales_RatePlanAPIService.GetZoneItem(getZoneItemInput()).then(function (response) {

                            getZoneItemDeferred.resolve();

                            var gridZoneItem = UtilsService.getItemByVal(allZoneItems, response.ZoneId, "ZoneId");

                            gridZoneItem.EffectiveRoutingProductId = response.EffectiveRoutingProductId;
                            gridZoneItem.EffectiveRoutingProductName = response.EffectiveRoutingProductName;

                            gridZoneItem.Margin = response.Margin;
                            gridZoneItem.MarginPercentage = response.MarginPercentage;
                            gridZoneItem.CalculatedRate = response.CalculatedRate;

                            gridZoneItem.RPRouteDetail = response.RPRouteDetail;
                            var routeOptions = getRouteOptions(response.RPRouteDetail);
                            gridZoneItem.RouteOptionsDetailsForView = response.RouteOptionsDetailsForView;
                            var routeOptionsDirectivePayload = getRouteOptionsDirectivePayload(gridZoneItem);
                            UtilsService.convertToPromiseIfUndefined(gridZoneItem.RouteOptionsAPI.load(routeOptionsDirectivePayload)).then(function () {
                                loadRouteOptionsDeferred.resolve();
                            }).catch(function (error) {
                                loadRouteOptionsDeferred.reject(error);
                            });

                            gridZoneItem.EffectiveServiceIds = response.EffectiveServiceIds;

                            var serviceViewerPaylod = { selectedIds: response.EffectiveServiceIds };
                            UtilsService.convertToPromiseIfUndefined(gridZoneItem.serviceViewerAPI.load(serviceViewerPaylod)).then(function () {
                                loadEffectiveServicesDeferred.resolve();
                            }).catch(function (error) {
                                loadEffectiveServicesDeferred.reject(error);
                            });

                            if (response.Costs != null) {
                                gridZoneItem.Costs = [];
                                for (var i = 0; i < response.Costs.length; i++) {
                                    gridZoneItem.Costs[i] = response.Costs[i];
                                }
                            }

                        }).catch(function (error) { getZoneItemDeferred.reject(error); });
                    });

                    UtilsService.waitMultiplePromises(promises).catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });

                    function saveZoneItemChanges() {
                        $scope.isLoading = true;
                        return gridQuery.context.saveDraft()
                            .then(function () {
                                $scope.isLoading = false;
                            });
                    }

                    function getZoneItemInput() {
                        return {
                            OwnerType: gridQuery.OwnerType,
                            OwnerId: gridQuery.OwnerId,
                            ZoneId: zoneItem.ZoneId,
                            RoutingDatabaseId: gridQuery.RoutingDatabaseId,
                            PolicyConfigId: gridQuery.PolicyConfigId,
                            NumberOfOptions: gridQuery.NumberOfOptions,
                            CostCalculationMethods: gridQuery.CostCalculationMethods,
                            RateCalculationCostColumnConfigId: gridQuery.CostCalculationMethodConfigId,
                            RateCalculationMethod: gridQuery.RateCalculationMethod,
                            CurrencyId: gridQuery.CurrencyId
                        };
                    }
                };

                zoneItem.validateRateChangeDates = function () {
                    var errorMessage = validateTimeRange(zoneItem.CurrentRateBED, zoneItem.CurrentRateEED);
                    if (errorMessage != null)
                        return errorMessage;

                    if (zoneItem.ZoneEED != null && compareDates(zoneItem.CurrentRateEED, zoneItem.ZoneEED) == 1)
                        return 'EED of current rate > EED of zone';

                    return null;
                };

                zoneItem.validateNewRateDates = function () {
                    return WhS_Sales_RatePlanUtilsService.validateNewRateDates(zoneItem);
                };

                function isRatePlanZoneDirty() {
                    return ((zoneItem.NewRates != undefined && zoneItem.NewRates.length > 0) || (zoneItem.ClosedRates != undefined && zoneItem.ClosedRates.length > 0) || zoneItem.NewRoutingProduct != null || zoneItem.ResetRoutingProduct != null);
                }
                function validateTimeRange(date1, date2) {
                    return VRValidationService.validateTimeRange(date1, date2);
                }
                function compareDates(date1, date2) {
                    if (!date1 && !date2)
                        return 0;
                    if (!date2)
                        return 2;

                    var d1 = new Date(date1);
                    var d2 = new Date(date2);

                    var year1 = d1.getYear();
                    var year2 = d2.getYear();
                    if (year1 > year2)
                        return 1;
                    if (year2 > year1)
                        return 2;

                    var month1 = d1.getMonth();
                    var month2 = d2.getMonth();
                    if (month1 > month2)
                        return 1;
                    if (month2 > month1)
                        return 2;

                    var day1 = d1.getDay();
                    var day2 = d2.getDay();
                    if (day1 > day2)
                        return 1;
                    if (day2 > day1)
                        return 2;

                    return 0;
                }
                function setRouteOptionProperties(zoneItem) {
                    zoneItem.RouteOptionsLoadDeferred = UtilsService.createPromiseDeferred();

                    zoneItem.onRouteOptionsReady = function (api) {
                        zoneItem.RouteOptionsAPI = api;
                        var routeOptionsDirectivePayload = getRouteOptionsDirectivePayload(zoneItem);
                        VRUIUtilsService.callDirectiveLoad(zoneItem.RouteOptionsAPI, routeOptionsDirectivePayload, zoneItem.RouteOptionsLoadDeferred);
                    };
                }
                function setServiceViewerLoad(zoneItem) {
                    zoneItem.serviceViewerLoadDeferred = UtilsService.createPromiseDeferred();
                    zoneItem.onServiceViewerReady = function (api) {
                        zoneItem.serviceViewerAPI = api;
                        var serviceViewerPayload = { selectedIds: zoneItem.EffectiveServiceIds };
                        VRUIUtilsService.callDirectiveLoad(zoneItem.serviceViewerAPI, serviceViewerPayload, zoneItem.serviceViewerLoadDeferred);
                    };
                }

                function setCurrencyIconProperties(dataItem) {
                    if (gridQuery.OwnerType == WhS_BE_SalePriceListOwnerTypeEnum.SellingProduct.value)
                        return;
                    if (dataItem.CurrentRate == null)
                        return;
                    if (gridQuery.CurrencyId == undefined || gridQuery.CurrencyId == null)
                        return;
                    if (gridQuery.CurrencyId != dataItem.CurrentRateCurrencyId) {
                        dataItem.currencyIconType = 'exchange';
                        dataItem.currencyIconTooltip =  'The real currency for this rate is ' + dataItem.CurrentRateCurrencySymbol;
                    }
                }
            }

            function setDrillDownExtensionObject(zoneItem) {
                zoneItem.drillDownExtensionObject = {};
                zoneItem.drillDownExtensionObject.drillDownDirectiveTabs = [];
                for (var i = 0; i < gridDrillDownDefinitions.length; i++) {
                    addDrillDownTab(gridDrillDownDefinitions[i], zoneItem);
                }
            }

            function addDrillDownTab(drillDownDefinition, zoneItem) {
                var drillDownDirectiveTab = {};

                drillDownDirectiveTab.title = drillDownDefinition.title;
                drillDownDirectiveTab.directive = drillDownDefinition.directive;
                drillDownDirectiveTab.loadDirective = function (directiveAPI) {
                    return drillDownDefinition.loadDirective(directiveAPI, zoneItem);
                };
                zoneItem.drillDownExtensionObject.drillDownDirectiveTabs.push(drillDownDirectiveTab);
            }

            function getRouteOptionsDirectivePayload(dataItem) {
                var rate;
                if (!WhS_Sales_RatePlanUtilsService.isStringEmpty(dataItem.NewRate))
                    rate = Number(dataItem.NewRate);
                else if (dataItem.CurrentRate != null)
                    rate = dataItem.CurrentRate;
                return {
                    RoutingDatabaseId: gridQuery.RoutingDatabaseId,
                    RoutingProductId: dataItem.EffectiveRoutingProductId,
                    SaleZoneId: dataItem.ZoneId,
                    RouteOptions: dataItem.RouteOptionsDetailsForView,
                    CurrencyId: gridQuery.CurrencyId,
                    saleRate: rate
                };
            }

            function getRouteOptions(rpRouteDetail) {
                return (rpRouteDetail != undefined) ? rpRouteDetail.RouteOptionsDetails : null;
            }

            function sortZones(col) {
                var currentSortDirection = col.sortDirection;
                var newSortDirection = currentSortDirection != "ASC" ? "ASC" : "DESC";

                var sortFunction;
                switch (col.name) {
                    case "zone": sortFunction = function (zoneItem1, zoneItem2) {
                        if (zoneItem1.ZoneName > zoneItem2.ZoneName)
                            return 1;
                        else
                            return -1;
                    }; break;
                    case "product": sortFunction = function (zoneItem1, zoneItem2) {
                        if (zoneItem1.EffectiveRoutingProductName > zoneItem2.EffectiveRoutingProductName)
                            return 1;
                        else
                            return -1;
                    }; break;
                    case "currentRate": sortFunction = function (zoneItem1, zoneItem2) {
                        if (zoneItem1.CurrentRate > zoneItem2.CurrentRate)
                            return 1;
                        else
                            return -1;
                    }; break;
                    case "inherIcontype": sortFunction = function (zoneItem1, zoneItem2) {
                        var zoneItem1Empty = zoneItem1.iconType == undefined;
                        var zoneItem2Empty = zoneItem2.iconType == undefined;
                        if (zoneItem1Empty && zoneItem2Empty)
                            return 0;
                        else if (zoneItem1Empty)
                            return -1;
                        else if (zoneItem2Empty)
                            return 1;
                        if (zoneItem1.iconType > zoneItem2.iconType)
                            return 1;
                        else
                            return -1;
                    }; break;
                    case "routeOptions": sortFunction = function (zoneItem1, zoneItem2) {
                        var zoneItem1Empty = zoneItem1.RPRouteDetail == undefined || zoneItem1.RPRouteDetail.RouteOptionsDetails == undefined || zoneItem1.RPRouteDetail.RouteOptionsDetails.length < 1;
                        var zoneItem2Empty = zoneItem2.RPRouteDetail == undefined || zoneItem2.RPRouteDetail.RouteOptionsDetails == undefined || zoneItem2.RPRouteDetail.RouteOptionsDetails.length < 1;
                        if (zoneItem1Empty && zoneItem2Empty)
                            return 0;
                        else if (zoneItem1Empty)
                            return -1;
                        else if (zoneItem2Empty)
                            return 1;
                        else if (zoneItem1.RPRouteDetail.RouteOptionsDetails[0].ConvertedSupplierRate > zoneItem2.RPRouteDetail.RouteOptionsDetails[0].ConvertedSupplierRate)
                            return 1;
                        else
                            return -1;
                    }; break;
                    case "margin": sortFunction = function (zoneItem1, zoneItem2) {
                        if (zoneItem1.Margin > zoneItem2.Margin)
                            return 1;
                        else
                            return -1;
                    }; break;
                    case "marginPerc": sortFunction = function (zoneItem1, zoneItem2) {
                        if (zoneItem1.MarginPercentage > zoneItem2.MarginPercentage)
                            return 1;
                        else
                            return -1;
                    }; break;
                    case "profitPerc": sortFunction = function (zoneItem1, zoneItem2) {
                        if (zoneItem1.ProfitPerc == null)
                            return -1;
                        if (zoneItem2.ProfitPerc == null)
                            return 1;
                        if (zoneItem1.ProfitPerc > zoneItem2.ProfitPerc)
                            return 1;
                        return -1;
                    }; break;
                    case "newRate": sortFunction = function (zoneItem1, zoneItem2) {
                        if (zoneItem1.NewRate > zoneItem2.NewRate)
                            return 1;
                        else
                            return -1;
                    }; break;
                    case "rateChangeIcon": sortFunction = function (zoneItem1, zoneItem2) {
                        var zoneItem1Empty = zoneItem1.RateChangeTypeIconTooltip == undefined;
                        var zoneItem2Empty = zoneItem2.RateChangeTypeIconTooltip == undefined;
                        if (zoneItem1Empty && zoneItem2Empty)
                            return 0;
                        else if (zoneItem1Empty)
                            return -1;
                        else if (zoneItem2Empty)
                            return 1;
                        if (zoneItem1.RateChangeTypeIconTooltip > zoneItem2.RateChangeTypeIconTooltip)
                            return 1;
                        else
                            return -1;
                    }; break;
                    case "bed": sortFunction = function (zoneItem1, zoneItem2) {
                        var date1 = zoneItem1.NewRate != null ? zoneItem1.NewRateBED : zoneItem1.CurrentRateBED;
                        var date2 = zoneItem2.NewRate != null ? zoneItem2.NewRateBED : zoneItem2.CurrentRateBED;
                        var zoneItem1Empty = date1 == undefined;
                        var zoneItem2Empty = date2 == undefined;
                        if (zoneItem1Empty && zoneItem2Empty)
                            return 0;
                        else if (zoneItem1Empty)
                            return -1;
                        else if (zoneItem2Empty)
                            return 1;
                        if (date1 > date2)
                            return 1;
                        else
                            return -1;
                    }; break;
                    case "eed": sortFunction = function (zoneItem1, zoneItem2) {
                        var date1 = zoneItem1.NewRate != null ? zoneItem1.NewRateEED : zoneItem1.CurrentRateEED;
                        var date2 = zoneItem2.NewRate != null ? zoneItem2.NewRateEED : zoneItem2.CurrentRateEED;
                        var zoneItem1Empty = date1 == undefined;
                        var zoneItem2Empty = date2 == undefined;
                        if (zoneItem1Empty && zoneItem2Empty)
                            return 0;
                        else if (zoneItem1Empty)
                            return -1;
                        else if (zoneItem2Empty)
                            return 1;
                        if (date1 > date2)
                            return 1;
                        else
                            return -1;
                    }; break;
                }
                if (sortFunction == undefined) {
                    if (col.name.indexOf('costColumn_') >= 0) {
                        var costColumnIndex = parseInt(col.name.substring('costColumn_'.length));
                        sortFunction = function (zoneItem1, zoneItem2) {
                            if (zoneItem1.Costs[costColumnIndex] > zoneItem2.Costs[costColumnIndex])
                                return 1;
                            else
                                return -1;
                        };
                    }
                }

                var finalSortFunction = newSortDirection == "ASC" ? sortFunction : function (zoneItem1, zoneItem2) { return -sortFunction(zoneItem1, zoneItem2); };
                $scope.zoneItems.sort(finalSortFunction);
                clearSorting();
                col.sortDirection = newSortDirection;
                scrollGrid();
            }

            function clearSorting() {
                for (var colName in columnsConfig) {
                    columnsConfig[colName].sortDirection = undefined;
                }
            }

            function applyChanges(zoneChanges, zoneItem) {
                if (zoneItem.IsDirty) {
                    var zoneItemChanges = {
                        ZoneId: zoneItem.ZoneId,
                        ZoneName: zoneItem.ZoneName,
                        CountryId: zoneItem.CountryId,
                        ProfitPerc: zoneItem.ProfitPerc,
                    };

                    setDraftRateToChange(zoneItemChanges, zoneItem);
                    if (WhS_Sales_RatePlanUtilsService.isStringEmpty(zoneItem.NewRate)) {
                        setDraftRateToClose(zoneItemChanges, zoneItem);
                    }

                    finalizeZoneItemBeforeUnload(zoneItem);

                    applyRoutingProductChanges();
                    applyOtherRateChanges();
                    applyServiceChanges();

                    zoneChanges.push(zoneItemChanges);
                }

                function setDraftRateToChange(zoneChanges, zoneItem) {
                    zoneChanges.NewRate = null;

                    if (zoneItem.NewRate) {
                        var newRate = {
                            ZoneId: zoneItem.ZoneId,
                            Rate: zoneItem.NewRate,
                            BED: zoneItem.NewRateBED,
                            EED: zoneItem.NewRateEED,
                            IsCancellingRate: zoneItem.IsNewRateCancelling
                        };
                        zoneChanges.NewRates = [newRate];
                    }
                }
                function setDraftRateToClose(zoneChanges, zoneItem) {
                    zoneChanges.RateChange = null;

                    if (zoneItem.IsCurrentRateEditable && !WhS_Sales_RatePlanService.areDatesTheSame(zoneItem.CurrentRateEED, zoneItem.currentRateEED)) {
                        var rateToClose = {
                            ZoneId: zoneItem.ZoneId,
                            RateId: zoneItem.CurrentRateId,
                            EED: zoneItem.CurrentRateEED
                        };
                        zoneChanges.ClosedRates = [rateToClose];
                    }

                }

                function applyRoutingProductChanges() {
                    zoneItemChanges.NewRoutingProduct = zoneItem.NewRoutingProduct;
                    zoneItemChanges.RoutingProductChange = zoneItem.ResetRoutingProduct;
                }
                function applyOtherRateChanges() {
                    if (zoneItem.NewRates != null) {
                        if (zoneItemChanges.NewRates == null)
                            zoneItemChanges.NewRates = [];

                        for (var i = 0; i < zoneItem.NewRates.length; i++) {
                            if (zoneItem.NewRates[i].RateTypeId != null)
                                zoneItemChanges.NewRates.push(zoneItem.NewRates[i]);
                        }
                    }

                    if (zoneItem.ClosedRates != null) {
                        if (zoneItemChanges.ClosedRates == null)
                            zoneItemChanges.ClosedRates = [];

                        for (var i = 0; i < zoneItem.ClosedRates.length; i++) {
                            if (zoneItem.ClosedRates[i].RateTypeId != null)
                                zoneItemChanges.ClosedRates.push(zoneItem.ClosedRates[i]);
                        }
                    }
                }
                function applyServiceChanges() {
                    zoneItemChanges.NewService = zoneItem.NewService;
                    zoneItemChanges.ClosedService = zoneItem.ClosedService;
                    zoneItemChanges.ResetService = zoneItem.ResetService;
                }
            }

            function finalizeZoneItemBeforeUnload(zoneItem) {
                for (var i = 0; i < zoneItem.drillDownExtensionObject.drillDownDirectiveTabs.length; i++) {
                    var item = zoneItem.drillDownExtensionObject.drillDownDirectiveTabs[i];

                    if (item.directiveAPI && item.directiveAPI.applyChanges)
                        item.directiveAPI.applyChanges();
                }
            }
            function showApplicableZonesTab(value) {
                if (gridQuery.context != undefined && gridQuery.context.showApplicableZonesTab != undefined)
                    gridQuery.context.showApplicableZonesTab(value);
            }

            function refreshHeaderWidth() {
                var div = $($element).find("#gridBodyContainer")[0];// need real DOM Node, not jQuery wrapper
                var hasVerticalScrollbar = div.scrollHeight > div.clientHeight;
                if (hasVerticalScrollbar) {
                    $(div).css({ "overflow-y": 'auto', "overflow-x": 'hidden' });
                    if (VRLocalizationService.isLocalizationRTL()) {
                        $scope.headerStyle = {
                            "padding-left": getScrollbarWidth() + "px"
                        };
                    }
                    else {
                        $scope.headerStyle = {
                            "padding-right": getScrollbarWidth() + "px"
                        };
                    }


                }

                else {
                    $(div).css({ "overflow-y": 'auto', "overflow-x": 'hidden' });

                    if (VRLocalizationService.isLocalizationRTL()) {
                        $scope.headerStyle = {
                            "padding-left": "0px"
                        };
                    }
                    else {
                        $scope.headerStyle = {
                            "padding-right": "0px"
                        };
                    }
                }
                for (var colName in columnsConfig) {
                    var column = columnsConfig[colName];
                    if (column.widthFactor != undefined)
                        $('.rpcolumn_' + colName).css({ width: column.width });
                }

            }

            function getScrollbarWidth() {
                var outer = document.createElement("div");
                outer.style.visibility = "hidden";
                outer.style.width = "100px";
                outer.style.msOverflowStyle = "scrollbar"; // needed for WinJS apps

                document.body.appendChild(outer);

                var widthNoScroll = outer.offsetWidth;
                // force scrollbars
                outer.style.overflow = "scroll";

                // add innerdiv
                var inner = document.createElement("div");
                inner.style.width = "100%";
                outer.appendChild(inner);

                var widthWithScroll = inner.offsetWidth;

                // remove divs
                outer.parentNode.removeChild(outer);

                return widthNoScroll - widthWithScroll;
            }

            var gridBodyElement = $element.find("#gridBody");
            var gridBodyContainer = $($element.find("#gridBodyContainer"));

            var lastScrollingTime;
            var handleScrollingStarted = false;

            function defineScrollEvent() {
                gridBodyContainer.on('scroll', scrollGrid);
                $element.on('$destroy', function () {
                    gridBodyContainer.unbind('scroll', scrollGrid);
                    $scope.$destroy();
                });
            }

            function scrollGrid() {
                lastScrollingTime = VRDateTimeService.getNowDateTime();
                if (!handleScrollingStarted)
                    handleScrolling();
            }

            function handleScrolling() {
                handleScrollingStarted = true;
                var diffInMilliseconds = (VRDateTimeService.getNowDateTime()).getTime() - lastScrollingTime;
                if (diffInMilliseconds < 150)
                    setTimeout(handleScrolling, 150);
                else {
                    updateRowDisplays();
                    handleScrollingStarted = false;
                }
            }

            function updateRowDisplays() {
                var diffInMilliseconds = (VRDateTimeService.getNowDateTime()).getTime() - lastScrollingTime;
                if (diffInMilliseconds < 150)
                    return;
                var scrollTop = gridBodyContainer.scrollTop();
                var scrollPercentage = 100 * scrollTop / (gridBodyElement.height() - gridBodyContainer.height());
                var middleElementIndex = Math.round(scrollPercentage * $scope.zoneItems.length / 100);
                var nbOfItemsChanged = 0;
                var maxItemsToRender = 10;

                var startingIndex = ($scope.zoneItems.length - middleElementIndex) > 30 ? 10 : 0;

                for (var i = startingIndex; i < $scope.zoneItems.length && nbOfItemsChanged < maxItemsToRender; i++) {
                    if (i >= 0)
                        setItemsRowDisplay(i, true);
                }

                for (var i = 0; i < $scope.zoneItems.length && nbOfItemsChanged < maxItemsToRender; i++) {
                    setItemsRowDisplay(i, true);
                }

                for (var i = 0; i < $scope.zoneItems.length && nbOfItemsChanged < maxItemsToRender; i++) {
                    setItemsRowDisplay(i, false);
                }

                if (nbOfItemsChanged > 0)
                    $scope.$apply();
                if (nbOfItemsChanged == maxItemsToRender)
                    setTimeout(updateRowDisplays, 200);

                function setItemsRowDisplay(itemIndex, tryDisplay) {
                    var zoneItem = $scope.zoneItems[itemIndex];
                    var difference = Math.abs(itemIndex - middleElementIndex);
                    if (tryDisplay) {
                        if (difference <= 30 && zoneItem.displayRow == false) {
                            zoneItem.displayRow = true;
                            nbOfItemsChanged++;
                        }
                    }
                    else {
                        if (difference > 200 && zoneItem.displayRow == true && zoneItem.validationContext.validate() == null) {//hide displayed row only if it's 200 rows away from the scroll position
                            finalizeZoneItemBeforeUnload(zoneItem);
                            zoneItem.displayRow = false;
                            nbOfItemsChanged++;
                        }
                    }
                }
            }

            /*##### Grid Filter #####*/
            var lastTriggerFilterZonesTime;
            var filterRoutingProductSelectorAPI;
            var filterRoutingProductSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var comparisonOptions;
            var rateSourceOptions;
            var newRateFilterOptions;
            var rateComparisonOptions;

            function initGridFilter() {
                defineGridFilterDataSources();
                defineGridFilterDefaultValues();
                function defineGridFilterDataSources() {
                    $scope.zoneNameFilterOptions = UtilsService.getArrayEnum(VRCommon_TextFilterTypeEnum);

                    comparisonOptions = {
                        Equals: { value: 1, text: 'Equals', symbol: '=' },
                        GreaterThan: { value: 2, text: 'Greater Than', symbol: '>' },
                        GreaterThanOrEquals: { value: 3, text: 'Greater Than Or Equals', symbol: '>=' },
                        LessThan: { value: 4, text: 'Less Than', symbol: '<' },
                        LessThanOrEquals: { value: 5, text: 'Less Than Or Equals', symbol: '<=' }
                    };
                    $scope.comparisonOptions = UtilsService.getArrayEnum(comparisonOptions);

                    rateSourceOptions = {
                        Inherited: { value: 1, text: 'Inherited', symbol: 'I' },
                        Explicit: { value: 2, text: 'Explicit', symbol: 'E' }
                    };
                    $scope.rateSourceOptions = UtilsService.getArrayEnum(rateSourceOptions);

                    newRateFilterOptions = {
                        Priced: { value: 6, text: 'Priced', symbol: 'Priced' },
                        Unpriced: { value: 7, text: 'Unpriced', symbol: 'Unpriced' },
                        Equals: { value: 1, text: 'Equals', symbol: '=' },
                        GreaterThan: { value: 2, text: 'Greater Than', symbol: '>' },
                        GreaterThanOrEquals: { value: 3, text: 'Greater Than Or Equals', symbol: '>=' },
                        LessThan: { value: 4, text: 'Less Than', symbol: '<' },
                        LessThanOrEquals: { value: 5, text: 'Less Than Or Equals', symbol: '<=' }
                    };
                    $scope.newRateFilterOptions = UtilsService.getArrayEnum(newRateFilterOptions);

                    rateComparisonOptions = {
                        Increase: { value: 1, text: 'Increase', symbol: 'I' },
                        Decrease: { value: 2, text: 'Decrease', symbol: 'D' }
                    };
                    $scope.rateComparisonOptions = UtilsService.getArrayEnum(rateComparisonOptions);
                }
                function defineGridFilterDefaultValues() {
                    $scope.routeOptionNumbers = [];
                    $scope.selectedRateSourceOptions = [];
                    $scope.costComparisonOptions = [];
                    $scope.costFilterValues = [];
                    $scope.selectedRateComparisonOptions = [];

                    $scope.zoneNameFilterOption = VRCommon_TextFilterTypeEnum.Contains;
                    var defaultComparisonOption = getDefaultComparisonOption();
                    $scope.currentRateComparisonOption = defaultComparisonOption;
                    $scope.marginComparisonOption = defaultComparisonOption;
                    $scope.marginPercentageComparisonOption = defaultComparisonOption;
                    $scope.profitPercentageComparisonOption = defaultComparisonOption;
                    $scope.routeOptionComparisonOption = defaultComparisonOption;
                    $scope.newRateFilterOption = getDefaultNewRateFilterOption();
                }
            }
            function defineRouteOptionNumbers() {
                $scope.routeOptionNumbers.length = 0;
                for (var i = 0; i < gridQuery.NumberOfOptions; i++) {
                    var optionNumberValue = i + 1;
                    $scope.routeOptionNumbers.push({
                        value: optionNumberValue,
                        text: 'Option ' + optionNumberValue
                    });
                }
                if ($scope.routeOptionNumbers.length > 0)
                    $scope.routeOptionNumber = $scope.routeOptionNumbers[0];
            }
            function defineDefaultCostComparisonOptions() {
                $scope.costComparisonOptions.length = 0;
                var defaultComparisonOption = getDefaultComparisonOption();
                if (gridQuery.CostCalculationMethods != null) {
                    for (var i = 0; i < gridQuery.CostCalculationMethods.length; i++) {
                        $scope.costComparisonOptions.push(defaultComparisonOption);
                    }
                }
            }
            function getDefaultComparisonOption() {
                return comparisonOptions.GreaterThan;
            }
            function getDefaultNewRateFilterOption() {
                return newRateFilterOptions.GreaterThan;
            }

            function triggerfilterZones() {
                var diffInMilliseconds = (VRDateTimeService.getNowDateTime()).getTime() - lastTriggerFilterZonesTime;
                if (diffInMilliseconds < 1000) {
                    setTimeout(triggerfilterZones, 1000);
                }
                else {
                    filterZones();
                    lastTriggerFilterZonesTime = undefined;
                }
            }
            function filterZones() {
                filteredZoneItems.length = 0;
                $scope.zoneItems.length = 0;
                for (var i = 0; i < allZoneItems.length; i++) {
                    var zoneItem = allZoneItems[i];
                    if (!isZoneItemExcluded(zoneItem))
                        filteredZoneItems.push(zoneItem);
                }
                return displayZoneItems();
            }
            function isZoneItemExcluded(zoneItem) {

                function zoneNameFilter(zoneName) {
                    if ($scope.zoneNameFilterValue == undefined || $scope.zoneNameFilterOption == undefined)
                        return true;
                    if (zoneName == undefined)
                        return false;
                    var formattedZoneName = zoneName.toLowerCase();
                    var formattedZoneNameFilterValue = $scope.zoneNameFilterValue.toLowerCase();
                    switch ($scope.zoneNameFilterOption.value) {
                        case VRCommon_TextFilterTypeEnum.StartsWith.value:
                            return formattedZoneName.startsWith(formattedZoneNameFilterValue);
                            break;
                        case VRCommon_TextFilterTypeEnum.Contains.value:
                            return (formattedZoneName.indexOf(formattedZoneNameFilterValue) >= 0);
                        case VRCommon_TextFilterTypeEnum.Equals.value:
                            return (formattedZoneName == formattedZoneNameFilterValue);
                            break;
                    }
                    return false;
                }
                function effectiveRoutingProductFilter(effectiveRoutingProductId) {
                    if ($scope.filterRoutingProducts.length == 0)
                        return true;
                    if (effectiveRoutingProductId == undefined)
                        return false;
                    return UtilsService.getItemIndexByVal($scope.filterRoutingProducts, effectiveRoutingProductId, 'RoutingProductId') != -1;
                }
                function currentRateSourceFilter(isCurrentRateEditable) {
                    if ($scope.selectedRateSourceOptions.length == 0)
                        return true;
                    if (isCurrentRateEditable == undefined)
                        return false;
                    for (var i = 0; i < $scope.selectedRateSourceOptions.length; i++) {
                        switch ($scope.selectedRateSourceOptions[i].value) {
                            case rateSourceOptions.Inherited.value:
                                if (!isCurrentRateEditable)
                                    return true;
                                break;
                            case rateSourceOptions.Explicit.value:
                                if (isCurrentRateEditable)
                                    return true;
                                break;
                        }
                    }
                    return false;
                }
                function currentRateFilter(currentRateValue) {
                    return comparisonOptionFilter(currentRateValue, $scope.currentRateFilterValue, $scope.currentRateComparisonOption);
                }
                function newRateFilter(newRateValue) {
                    if ($scope.newRateFilterOption == undefined)
                        return true;
                    if ($scope.newRateFilterOption.value == newRateFilterOptions.Priced.value)
                        return newRateValue != undefined;
                    if ($scope.newRateFilterOption.value == newRateFilterOptions.Unpriced.value)
                        return newRateValue == undefined;
                    return comparisonOptionFilter(newRateValue, $scope.newRateFilterValue, $scope.newRateFilterOption);
                }
                function rateComparisionFilter(currentRateValue, newRateValue) {
                    if ($scope.selectedRateComparisonOptions.length == 0)
                        return true;
                    if (currentRateValue == undefined || newRateValue == undefined || currentRateValue == newRateValue)
                        return false;
                    for (var i = 0; i < $scope.selectedRateComparisonOptions.length; i++) {
                        switch ($scope.selectedRateComparisonOptions[i].value) {
                            case rateComparisonOptions.Increase.value:
                                if (newRateValue > currentRateValue)
                                    return true;
                                break;
                            case rateComparisonOptions.Decrease.value:
                                if (newRateValue < currentRateValue)
                                    return true;
                                break;
                        }
                    }
                    return false;
                }
                function routeOptionFilter(rpRouteDetail) {
                    if ($scope.routeOptionNumber == undefined || $scope.routeOptionComparisonOption == undefined || $scope.routeOptionFilterValue == undefined)
                        return true;
                    if (rpRouteDetail == undefined || rpRouteDetail.RouteOptionsDetails == null || rpRouteDetail.RouteOptionsDetails.length == 0)
                        return false;
                    if ($scope.routeOptionNumber.value > rpRouteDetail.RouteOptionsDetails.length)
                        return false;
                    var routeOption = rpRouteDetail.RouteOptionsDetails[$scope.routeOptionNumber.value - 1];
                    return comparisonOptionFilter(routeOption.ConvertedSupplierRate, $scope.routeOptionFilterValue, $scope.routeOptionComparisonOption);
                }
                function costsFilter(costValues) {
                    for (var i = 0; i < $scope.costCalculationMethods.length; i++) {
                        var costValue = costValues[i];
                        var costFilterValue = $scope.costFilterValues[i];
                        var costComparisonOption = $scope.costComparisonOptions[i];
                        if (!comparisonOptionFilter(costValue, costFilterValue, costComparisonOption))
                            return false;
                    }
                    return true;
                }
                function marginFilter(marginValue) {
                    return comparisonOptionFilter(marginValue, $scope.marginFilterValue, $scope.marginComparisonOption);
                }
                function marginPercentageFilter(marginPercentageValue) {
                    return comparisonOptionFilter(marginPercentageValue, $scope.marginPercentageFilterValue, $scope.marginPercentageComparisonOption);
                }
                function profitPercentageFilter(profitPercentageValue) {
                    return comparisonOptionFilter(profitPercentageValue, $scope.profitPercentageFilterValue, $scope.profitPercentageComparisonOption);
                }
                function comparisonOptionFilter(value, filterValue, comparisonOption) {
                    if (comparisonOption == undefined || filterValue == undefined)
                        return true;
                    if (value == undefined)
                        return false;
                    var parsedValue = parseFloat(value);
                    var parsedFilterValue = parseFloat(filterValue);
                    switch (comparisonOption.value) {
                        case comparisonOptions.Equals.value:
                            if (parsedValue == parsedFilterValue)
                                return true;
                            break;
                        case comparisonOptions.GreaterThan.value:
                            if (parsedValue > parsedFilterValue)
                                return true;
                            break;
                        case comparisonOptions.GreaterThanOrEquals.value:
                            if (parsedValue >= parsedFilterValue)
                                return true;
                            break;
                        case comparisonOptions.LessThan.value:
                            if (parsedValue < parsedFilterValue)
                                return true;
                            break;
                        case comparisonOptions.LessThanOrEquals.value:
                            if (parsedValue <= parsedFilterValue)
                                return true;
                            break;
                    }
                    return false;
                }
                function pricingOptionFilter(value, pricingOption) {
                    if (pricingOption == undefined)
                        return true;
                    switch (pricingOption.text) {
                        case 'Priced':
                            if (value != undefined)
                                return true;
                            break;
                        case 'Unpriced':
                            if (value == undefined)
                                return true;
                            break;
                    }
                    return false;
                }

                if (!zoneNameFilter(zoneItem.ZoneName))
                    return true;
                if (!effectiveRoutingProductFilter(zoneItem.EffectiveRoutingProductId))
                    return true;
                if (!currentRateSourceFilter(zoneItem.IsCurrentRateEditable))
                    return true;
                if (!currentRateFilter(zoneItem.CurrentRate))
                    return true;
                if (!newRateFilter(zoneItem.NewRate))
                    return true;
                if (!rateComparisionFilter(zoneItem.CurrentRate, zoneItem.NewRate))
                    return true;
                if (!costsFilter(zoneItem.Costs))
                    return true;
                if (!routeOptionFilter(zoneItem.RPRouteDetail))
                    return true;
                if (!marginFilter(zoneItem.Margin))
                    return true;
                if (!marginPercentageFilter(zoneItem.MarginPercentage))
                    return true;
                if (!profitPercentageFilter((zoneItem.ProfitPerc != null) ? zoneItem.ProfitPerc : 0))
                    return true;

                return false;
            }
            function resetGridFilter() {
                $scope.showfilter = false;
                var defaultComparisonOption = getDefaultComparisonOption();

                resetZoneFilter();
                resetRoutingProductFilter();
                resetCurrentRateFilter();
                resetRouteOptionFilter();
                resetCostFilters();
                resetMarginFilter();
                resetMarginPercentageFilter();
                resetProfitPercentageFilter();
                resetNewRateFilter();

                function resetZoneFilter() {
                    $scope.zoneNameFilterValue = undefined;
                }
                function resetRoutingProductFilter() {
                    $scope.filterRoutingProducts.length = 0;
                }
                function resetCurrentRateFilter() {
                    $scope.currentRateComparisonOption = defaultComparisonOption;
                    $scope.currentRateFilterValue = undefined;
                    $scope.selectedRateSourceOptions.length = 0;
                }
                function resetRouteOptionFilter() {
                    $scope.routeOptionNumber = ($scope.routeOptionNumbers.length > 0) ? $scope.routeOptionNumbers[0] : undefined;
                    $scope.routeOptionComparisonOption = defaultComparisonOption;
                    $scope.routeOptionFilterValue = undefined;
                }
                function resetCostFilters() {
                    $scope.costComparisonOptions.length = 0;
                    $scope.costFilterValues.length = 0;
                    if (gridQuery.CostCalculationMethods != null) {
                        for (var i = 0; i < gridQuery.CostCalculationMethods.length; i++) {
                            $scope.costComparisonOptions.push(defaultComparisonOption);
                            $scope.costFilterValues.push(undefined);
                        }
                    }
                }
                function resetMarginFilter() {
                    $scope.marginComparisonOption = defaultComparisonOption;
                    $scope.marginFilterValue = undefined;
                }
                function resetMarginPercentageFilter() {
                    $scope.marginPercentageComparisonOption = defaultComparisonOption;
                    $scope.marginPercentageFilterValue = undefined;
                }
                function resetProfitPercentageFilter() {
                    $scope.profitPercentageComparisonOption = defaultComparisonOption;
                    $scope.profitPercentageFilterValue = undefined;
                }
                function resetNewRateFilter() {
                    $scope.newRateFilterOption = getDefaultNewRateFilterOption();
                    $scope.newRateFilterValue = undefined;
                    $scope.selectedRateComparisonOptions.length = 0;
                }
            }

            function onNewRateFilterOptionChanged() {
                $scope.hideNewRateFilterValue = ($scope.newRateFilterOption != undefined && ($scope.newRateFilterOption.value == newRateFilterOptions.Priced.value || $scope.newRateFilterOption.value == newRateFilterOptions.Unpriced.value));
                return $scope.filterZones();
            }
            /*##### Grid Filter #####*/
        }
    }]);