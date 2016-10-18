"use strict";

app.directive("vrWhsSalesRateplanGrid", ["WhS_Sales_RatePlanAPIService", "UtilsService", "VRUIUtilsService", "VRNotificationService", "VRValidationService", "VRCommon_RateTypeAPIService", 'WhS_Sales_RatePlanUtilsService', 'WhS_Sales_RatePlanService', 'WhS_BE_SalePriceListOwnerTypeEnum', 'WhS_BE_PrimarySaleEntityEnum', function (WhS_Sales_RatePlanAPIService, UtilsService, VRUIUtilsService, VRNotificationService, VRValidationService, VRCommon_RateTypeAPIService, WhS_Sales_RatePlanUtilsService, WhS_Sales_RatePlanService, WhS_BE_SalePriceListOwnerTypeEnum, WhS_BE_PrimarySaleEntityEnum) {
    return {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ratePlanGrid = new RatePlanGrid($scope, ctrl);
            ratePlanGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/WhS_Sales/Directives/Templates/RatePlanGridTemplate.html"
    };

    function RatePlanGrid($scope, ctrl)
    {
        this.initializeController = initializeController;

        var gridAPI;
        var gridQuery;
        var gridDrillDownTabs;

        var servicesTab;
        var routingProductTab;
        var otherRatesTab;

        var increasedRateDayOffset = 0;
        var decreasedRateDayOffset = 0;
        var newServiceDayOffset = 0;

        function initializeController() {
            $scope.zoneItems = [];
            $scope.costCalculationMethods = [];

            $scope.onGridReady = function (api) {
                gridAPI = api;
                gridDrillDownTabs = VRUIUtilsService.defineGridDrillDownTabs(getGridDrillDownDefinitions(), gridAPI, null);
                defineAPI();
            };
            $scope.loadMoreData = function () {
                return loadZoneItems();
            };

            $scope.onZoneNameClicked = function (dataItem) {
                WhS_Sales_RatePlanService.viewZoneInfo(gridQuery.OwnerType, gridQuery.OwnerId, dataItem.ZoneId, dataItem.ZoneName, dataItem.ZoneBED, dataItem.ZoneEED, gridQuery.CurrencyId);
            };

            $scope.onCurrentRateClicked = function (dataItem) {
                WhS_Sales_RatePlanService.viewFutureRate(dataItem.ZoneName, dataItem.FutureNormalRate);
            };

            $scope.onNewRateChanged = function (dataItem) {
                dataItem.IsDirty = true;
                WhS_Sales_RatePlanUtilsService.onNewRateChanged(dataItem);
            };

            $scope.onNewRateBlurred = function (dataItem) {
                dataItem.IsDirty = true;
                var settings = {
                    increasedRateDayOffset: increasedRateDayOffset,
                    decreasedRateDayOffset: decreasedRateDayOffset
                };
                WhS_Sales_RatePlanUtilsService.onNewRateBlurred(dataItem, settings);
            };

            $scope.onEffectiveServiceClicked = function (dataItem) {
                if (dataItem.EffectiveServices != null && servicesTab.setTabSelected != undefined)
                    servicesTab.setTabSelected(dataItem);
            };

            $scope.onEffectiveRPClicked = function (dataItem) {
                if (dataItem.EffectiveRoutingProductName != null && routingProductTab.setTabSelected != undefined)
                    routingProductTab.setTabSelected(dataItem);
            };

            $scope.getRowStyle = function (dataItem)
            {
                var rowStyle;
                var rate; // The rate to validate

                if (!WhS_Sales_RatePlanUtilsService.isStringEmpty(dataItem.NewRate))
                    rate = Number(dataItem.NewRate);
                else if (dataItem.CurrentRate != null)
                    rate = dataItem.CurrentRate;

                if (rate == undefined) {
                    setColorOfRouteOptions(dataItem.RouteOptions, null);
                    rowStyle = { CssClass: 'bg-success' };
                }
                else { // Validate the rate
                    if (dataItem.RouteOptions != null) {
                        var array = []; // Stores the indexes of route options having a greater rate than the rate to validate

                        for (var i = 0; i < dataItem.RouteOptions.length; i++) {
                            if (dataItem.RouteOptions[i].ConvertedSupplierRate > rate)
                                array.push(i);
                        }

                        if (array.length == dataItem.RouteOptions.length) {
                            setColorOfRouteOptions(dataItem.RouteOptions, null);
                            rowStyle = { CssClass: "bg-danger" };
                        }
                        else if (array.length > 0) {
                            for (var i = 0; i < dataItem.RouteOptions.length; i++)
                                dataItem.RouteOptions[i].Color = (UtilsService.contains(array, i)) ? 'orange' : null;
                        }
                        else {
                            setColorOfRouteOptions(dataItem.RouteOptions, null);
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
            function setColorOfRouteOptions(routeOptions, colorValue) {
                if (routeOptions != null)
                    for (var i = 0; i < routeOptions.length; i++)
                        routeOptions[i].Color = colorValue;
            }
        }

        function getGridDrillDownDefinitions() {
            servicesTab = {
                title: "Services",
                directive: "vr-whs-sales-zone-service",
                loadDirective: function (zoneServiceAPI, zoneItem) {
                    zoneItem.OwnerId = gridQuery.OwnerId;
                    zoneItem.context = gridQuery.context;
                    var zoneServicePayload = {
                        zoneItem: zoneItem,
                        settings: {
                            newServiceDayOffset: newServiceDayOffset
                        }
                    };
                    return zoneServiceAPI.load(zoneServicePayload);
                }
            };
            routingProductTab = {
                title: "Routing Product",
                directive: "vr-whs-sales-zoneroutingproduct",
                loadDirective: function (zoneRoutingProductDirectiveAPI, zoneItem) {
                    return zoneRoutingProductDirectiveAPI.load(zoneItem);
                }
            };
            otherRatesTab = {
                title: "Other Rates",
                directive: "vr-whs-sales-otherrate-grid",
                loadDirective: function (rateTypeGridAPI, zoneItem) {
                    var query = {
                        zoneItem: zoneItem,
                        settings: {
                            increasedRateDayOffset: increasedRateDayOffset,
                            decreasedRateDayOffset: decreasedRateDayOffset
                        }
                    };
                    return rateTypeGridAPI.loadGrid(query);
                }
            };
            return [servicesTab, routingProductTab, otherRatesTab];
        }

        function defineAPI() {
            var api = {};

            api.loadGrid = function (query) {
                gridQuery = query;

                if (query != undefined) {
                    if (query.CostCalculationMethods != null) {
                        $scope.costCalculationMethods = [];
                        for (var i = 0; i < query.CostCalculationMethods.length; i++)
                            $scope.costCalculationMethods.push(query.CostCalculationMethods[i]);
                    }
                    else {
                        $scope.costCalculationMethods.length = 0;
                    }
                    $scope.rateCalculationMethod = query.RateCalculationMethod;
                    setDayOffsets(query.Settings);
                }

                gridAPI.clearDataAndContinuePaging();
                return loadZoneItems();
            };

            function setDayOffsets(settings) {
                if (settings == undefined)
                    return;
                if (settings.IncreasedRateDayOffset != undefined)
                    increasedRateDayOffset = Number(settings.IncreasedRateDayOffset);
                if (settings.DecreasedRateDayOffset != undefined)
                    decreasedRateDayOffset = Number(settings.DecreasedRateDayOffset);
                if (settings.NewServiceDayOffset != undefined)
                    newServiceDayOffset = Number(settings.NewServiceDayOffset);
            }

            api.getZoneDrafts = function () {
                var zoneDrafts = [];

                for (var i = 0; i < $scope.zoneItems.length; i++) {
                    var item = $scope.zoneItems[i];

                    if (item.IsDirty)
                        applyChanges(zoneDrafts, item);
                }

                return zoneDrafts.length > 0 ? zoneDrafts : null;
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function loadZoneItems() {
            var promises = [];
            $scope.isLoading = true;

            var zoneItemsGetPromise = WhS_Sales_RatePlanAPIService.GetZoneItems(getZoneItemsInput());
            promises.push(zoneItemsGetPromise);

            var loadDirectivesDeferred = UtilsService.createPromiseDeferred();
            promises.push(loadDirectivesDeferred.promise);

            zoneItemsGetPromise.then(function (response)
            {
                var directivePromises = [];

                if (response != null) {
                    var zoneItems = [];

                    for (var i = 0; i < response.length; i++) {
                        var zoneItem = response[i];
                        extendZoneItem(zoneItem);
                        gridDrillDownTabs.setDrillDownExtensionObject(zoneItem);
                        directivePromises.push(zoneItem.RouteOptionsLoadDeferred.promise);
                        directivePromises.push(zoneItem.serviceViewerLoadDeferred.promise);

                        WhS_Sales_RatePlanUtilsService.onNewRateChanged(zoneItem);

                        zoneItems.push(zoneItem);
                    }

                    gridAPI.addItemsToSource(zoneItems);
                }

                UtilsService.waitMultiplePromises(directivePromises).then(function () {
                    loadDirectivesDeferred.resolve();
                }).catch(function (error) {
                    loadDirectivesDeferred.reject(error);
                });
            });

            return UtilsService.waitMultiplePromises(promises).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });

            function getZoneItemsInput() {
                var pageInfo = gridAPI.getPageInfo();

                return {
                    Filter: gridQuery,
                    FromRow: pageInfo.fromRow,
                    ToRow: pageInfo.toRow,
                    CurrencyId: gridQuery.CurrencyId
                };
            }
        }
        function extendZoneItem(zoneItem)
        {
            zoneItem.IsDirty = false;
            zoneItem.OwnerType = gridQuery.OwnerType;
            zoneItem.showRateChangeType = true;

            zoneItem.currentRateEED = zoneItem.CurrentRateEED; // Maintains the original value of zoneItem.CurrentRateEED in case the user deletes the new rate
            setRouteOptionProperties(zoneItem);
            setNormalRateIconProperties(zoneItem);

            zoneItem.serviceViewerLoadDeferred = UtilsService.createPromiseDeferred();
            zoneItem.onServiceViewerReady = function (api) {
                zoneItem.serviceViewerAPI = api;

                var serviceViewerPayload;
                if (zoneItem.EffectiveServices != null) {
                    serviceViewerPayload = {
                        selectedIds: UtilsService.getPropValuesFromArray(zoneItem.EffectiveServices, 'ServiceId')
                    };
                }

                VRUIUtilsService.callDirectiveLoad(zoneItem.serviceViewerAPI, serviceViewerPayload, zoneItem.serviceViewerLoadDeferred);
            };

            zoneItem.IsCurrentRateEditable = (zoneItem.IsCurrentRateEditable == null) ? false : zoneItem.IsCurrentRateEditable;

            if (zoneItem.NewRate != null) {
                zoneItem.IsDirty = true;
                zoneItem.showNewRateBED = true;
                zoneItem.showNewRateEED = true;

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
                return WhS_Sales_RatePlanUtilsService.validateNewRate(zoneItem);
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

                var loadRouteOptionsDeferred = UtilsService.createPromiseDeferred();
                promises.push(loadRouteOptionsDeferred.promise);

                saveChangesPromise.then(function () {
                    WhS_Sales_RatePlanAPIService.GetZoneItem(getZoneItemInput()).then(function (response) {
                        getZoneItemDeferred.resolve();

                        var gridZoneItem = UtilsService.getItemByVal($scope.zoneItems, response.ZoneId, "ZoneId");

                        gridZoneItem.EffectiveRoutingProductId = response.EffectiveRoutingProductId;
                        gridZoneItem.EffectiveRoutingProductName = response.EffectiveRoutingProductName;
                        gridZoneItem.CalculatedRate = response.CalculatedRate;

                        gridZoneItem.RouteOptions = response.RouteOptions;

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
                    var zoneChanges = [];
                    applyChanges(zoneChanges, zoneItem);

                    var input = {
                        OwnerType: gridQuery.OwnerType,
                        OwnerId: gridQuery.OwnerId,
                        NewChanges: {
                            ZoneChanges: zoneChanges
                        }
                    };

                    return WhS_Sales_RatePlanAPIService.SaveChanges(input);
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
            function setNormalRateIconProperties(dataItem)
            {
                if (gridQuery.OwnerType == WhS_BE_SalePriceListOwnerTypeEnum.SellingProduct.value)
                    return;
                if (dataItem.CurrentRate == null)
                    return;
                if (gridQuery.SaleAreaSettings == undefined || gridQuery.SaleAreaSettings.PrimarySaleEntity == null)
                    return;
                if (gridQuery.SaleAreaSettings.PrimarySaleEntity == WhS_BE_PrimarySaleEntityEnum.SellingProduct.value) {
                    if (dataItem.IsCurrentRateEditable === true) {
                        dataItem.iconType = 'explicit';
                        dataItem.iconTooltip = 'Explicit';
                    }
                }
                else if (dataItem.IsCurrentRateEditable === false) {
                    dataItem.iconType = 'inherited';
                    dataItem.iconTooltip = 'Inherited';
                }
            }
        }

        function getRouteOptionsDirectivePayload(dataItem) {
            return {
                RoutingDatabaseId: gridQuery.RoutingDatabaseId,
                RoutingProductId: dataItem.EffectiveRoutingProductId,
                SaleZoneId: dataItem.ZoneId,
                RouteOptions: dataItem.RouteOptions,
                CurrencyId: gridQuery.CurrencyId
            };
        }

        function applyChanges(zoneChanges, zoneItem) {
            if (zoneItem.IsDirty) {
                var zoneItemChanges = {
                    ZoneId: zoneItem.ZoneId,
                    ZoneName: zoneItem.ZoneName
                };

                setDraftRateToChange(zoneItemChanges, zoneItem);
                setDraftRateToClose(zoneItemChanges, zoneItem);

                for (var i = 0; i < zoneItem.drillDownExtensionObject.drillDownDirectiveTabs.length; i++) {
                    var item = zoneItem.drillDownExtensionObject.drillDownDirectiveTabs[i];

                    if (item.directiveAPI && item.directiveAPI.applyChanges)
                        item.directiveAPI.applyChanges(zoneItemChanges);
                }

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
                        EED: zoneItem.NewRateEED
                    };
                    zoneChanges.NewRates = [newRate];
                }
            }
            function setDraftRateToClose(zoneChanges, zoneItem) {
                zoneChanges.RateChange = null;

                if (zoneItem.IsCurrentRateEditable && !compareDates(zoneItem.CurrentRateEED, zoneItem.currentRateEED)) {
                    var rateToClose = {
                        ZoneId: zoneItem.ZoneId,
                        RateId: zoneItem.CurrentRateId,
                        EED: zoneItem.CurrentRateEED
                    };
                    zoneChanges.ClosedRates = [rateToClose];
                }

                function compareDates(date1, date2) {
                    if (date1 && date2) {
                        if (typeof date1 == 'string')
                            date1 = new Date(date1);
                        if (typeof date2 == 'string')
                            date2 = new Date(date2);
                        return (date1.getDay() == date2.getDay() && date1.getMonth() == date2.getMonth() && date1.getYear() == date2.getYear());
                    }
                    else if (!date1 && !date2)
                        return true;
                    else
                        return false;
                }
            }

            function applyRoutingProductChanges() {
                if (zoneItem.NewRoutingProductId != null) {
                    zoneItemChanges.NewRoutingProduct = {
                        ZoneId: zoneItemChanges.ZoneId,
                        ZoneRoutingProductId: zoneItem.NewRoutingProductId,
                        BED: zoneItem.NewRoutingProductBED,
                        EED: zoneItem.NewRoutingProductEED
                    };
                }
                else if (zoneItem.RoutingProductChangeEED != null) {
                    zoneItemChanges.RoutingProductChange = {
                        ZoneId: zoneItem.ZoneId,
                        ZoneRoutingProductId: zoneItem.CurrentRoutingProductId,
                        EED: zoneItem.RoutingProductChangeEED
                    };
                }
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
    }
}]);