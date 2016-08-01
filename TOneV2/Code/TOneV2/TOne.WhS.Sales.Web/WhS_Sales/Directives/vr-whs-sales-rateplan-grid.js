﻿"use strict";

app.directive("vrWhsSalesRateplanGrid", ["WhS_Sales_RatePlanAPIService", "UtilsService", "VRUIUtilsService", "VRNotificationService", "VRValidationService", "VRCommon_RateTypeAPIService",
    function (WhS_Sales_RatePlanAPIService, UtilsService, VRUIUtilsService, VRNotificationService, VRValidationService, VRCommon_RateTypeAPIService) {
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

        function RatePlanGrid($scope, ctrl) {
            this.initializeController = initializeController;

            var gridAPI;
            var gridQuery;
            var gridDrillDownTabs;
            var rateTypes = [];

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
            }

            function getGridDrillDownDefinitions() {
                return [{
                    title: "Routing Product",
                    directive: "vr-whs-sales-zoneroutingproduct",
                    loadDirective: function (zoneRoutingProductDirectiveAPI, zoneItem) {
                        return zoneRoutingProductDirectiveAPI.load(zoneItem);
                    }
                }, {
                    title: "Rates",
                    directive: "vr-whs-be-salerate-grid",
                    loadDirective: function (saleRateGridAPI, zoneItem) {
                        var query = {
                            OwnerType: gridQuery.OwnerType,
                            OwnerId: gridQuery.OwnerId,
                            ZonesIds: [zoneItem.ZoneId],
                            //EffectiveOn: new Date()
                        };
                        return saleRateGridAPI.loadGrid(query);
                    }
                }, {
                    title: "Codes",
                    directive: "vr-whs-be-salecode-grid",
                    loadDirective: function (saleCodeGridAPI, zoneItem) {
                        var query = {
                            SellingNumberPlanId: null,
                            ZonesIds: [zoneItem.ZoneId],
                            //EffectiveOn: new Date()
                        };
                        return saleCodeGridAPI.loadGrid(query);
                    }
                }];

                //var otherRatesDrillDownDefinition = {
                //    title: "Other Rates",
                //    directive: "vr-whs-sales-rate-type-grid",
                //    loadDirective: function (rateTypeGridAPI, zoneItem) {
                //        var query = {
                //            dataItem: zoneItem
                //        };
                //        return rateTypeGridAPI.loadGrid(query);
                //    }
                //};
            }

            function defineAPI() {
                var api = {};

                api.load = function (query) {
                    gridQuery = query;

                    if (query && query.CostCalculationMethods) {
                        $scope.costCalculationMethods = [];

                        for (var i = 0; i < query.CostCalculationMethods.length; i++)
                            $scope.costCalculationMethods.push(query.CostCalculationMethods[i]);
                    }

                    $scope.rateCalculationMethod = query.RateCalculationMethod;

                    gridAPI.clearDataAndContinuePaging();
                    return loadZoneItems();
                };

                api.getZoneChanges = function () {
                    var zoneChanges = [];

                    for (var i = 0; i < $scope.zoneItems.length; i++) {
                        var item = $scope.zoneItems[i];

                        if (item.IsDirty)
                            applyChanges(zoneChanges, item);
                    }

                    return zoneChanges.length > 0 ? zoneChanges : null;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function loadZoneItems() {
                var promises = [];
                $scope.isLoading = true;
                var ratesPromis = VRCommon_RateTypeAPIService.GetAllRateTypes().then(function (response) {
                    rateTypes = response;
                });
                promises.push(ratesPromis);
                var zoneItemsGetPromise = WhS_Sales_RatePlanAPIService.GetZoneItems(getZoneItemsInput());
                promises.push(zoneItemsGetPromise);


                zoneItemsGetPromise.then(function (response) {
                    if (response != null) {
                        var zoneItems = [];

                        for (var i = 0; i < response.length; i++) {
                            var zoneItem = response[i];
                            extendZoneItem(zoneItem);
                            gridDrillDownTabs.setDrillDownExtensionObject(zoneItem);
                            promises.push(zoneItem.RouteOptionsLoadDeferred.promise);

                            zoneItems.push(zoneItem);
                        }

                        gridAPI.addItemsToSource(zoneItems);
                    }
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
                        ToRow: pageInfo.toRow
                    };
                }

                function addRateTypesToZoneItem(zoneItem) {

                    for (var i = 0; i < rateTypes.length; i++) {
                        zoneItem.CurrentOtherRates[rateTypes[i].Name] = 0;
                    }
                }

                function extendZoneItem(zoneItem) {
                    zoneItem.IsDirty = false;
                    if (zoneItem.CurrentOtherRates == undefined)
                        zoneItem.CurrentOtherRates = {};
                    if (zoneItem.NewOtherRates == undefined)
                        zoneItem.NewOtherRates = {};
                    addRateTypesToZoneItem(zoneItem);
                    zoneItem.currentRateEED = zoneItem.CurrentRateEED; // Maintains the original value of zoneItem.CurrentRateEED in case the user deletes the new rate
                    setRouteOptionProperties(zoneItem);

                    zoneItem.IsCurrentRateEditable = (zoneItem.IsCurrentRateEditable == null) ? false : zoneItem.IsCurrentRateEditable;

                    if (zoneItem.NewRate != null) {
                        zoneItem.IsDirty = true;
                        zoneItem.showNewRateBED = true;
                        zoneItem.showNewRateEED = true;
                    }

                    zoneItem.setNewRateBED = function () {
                        zoneItem.IsDirty = true;

                        if (zoneItem.NewRate) {
                            zoneItem.showNewRateBED = true;
                            zoneItem.showNewRateEED = true;
                            zoneItem.CurrentRateEED = zoneItem.currentRateEED;

                            if (!zoneItem.AreNewDatesSet) {
                                zoneItem.NewRateBED = (zoneItem.CurrentRate == null || Number(zoneItem.NewRate) > zoneItem.CurrentRate) ? new Date(new Date().setDate(new Date().getDate() + 7)) : new Date();
                                zoneItem.NewRateEED = null;
                                zoneItem.AreNewDatesSet = true;
                            }
                        }
                        else {
                            zoneItem.NewRateBED = null;
                            zoneItem.NewRateEED = null;
                            zoneItem.showNewRateBED = false;
                            zoneItem.showNewRateEED = false;
                            zoneItem.AreNewDatesSet = false;
                        }
                    };

                    //zoneItem.onNewRateChanged = function (zoneItem) {
                        
                    //};

                    zoneItem.validateNewRate = function (zoneItem) {
                        if (zoneItem.CurrentRate) {
                            if (Number(zoneItem.CurrentRate) === Number(zoneItem.NewRate)) {
                                return "New rate must be higher or lower than the current rate";
                            }
                        }
                        return null;
                    };

                    zoneItem.onCurrentRateEEDChanged = function (zoneItem) {
                        zoneItem.IsDirty = true;
                    };
                    zoneItem.OnOtherRateChanges = function (zoneItem, dataItem) {
                        zoneItem.NewOtherRates[dataItem.RateTypeId] = dataItem.NewRate;
                        console.log(dataItem);
                        zoneItem.IsDirty = true;
                    }

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

                                var payload = {
                                    RoutingDatabaseId: gridQuery.RoutingDatabaseId,
                                    SaleZoneId: response.ZoneId,
                                    RoutingProductId: response.EffectiveRoutingProductId,
                                    RouteOptions: response.RouteOptions
                                };

                                zoneItem.isLoadingRouteOptions = true;
                                VRUIUtilsService.callDirectiveLoad(zoneItem.RouteOptionsAPI, payload, loadRouteOptionsDeferred);

                                loadRouteOptionsDeferred.promise.finally(function () {
                                    zoneItem.isLoadingRouteOptions = false;
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
                                RateCalculationMethod: gridQuery.RateCalculationMethod
                            };
                        }
                    };

                    zoneItem.validateRateChangeDates = function () {
                        return VRValidationService.validateTimeRange(zoneItem.CurrentRateBED, zoneItem.CurrentRateEED);
                    };

                    zoneItem.validateNewRateDates = function () {
                        return VRValidationService.validateTimeRange(zoneItem.NewRateBED, zoneItem.NewRateEED);
                    };

                    function setRouteOptionProperties(zoneItem) {
                        zoneItem.RouteOptionsLoadDeferred = UtilsService.createPromiseDeferred();

                        zoneItem.onRouteOptionsReady = function (api) {
                            zoneItem.RouteOptionsAPI = api;
                            var payload = {
                                RoutingDatabaseId: gridQuery.RoutingDatabaseId,
                                RoutingProductId: zoneItem.EffectiveRoutingProductId,
                                SaleZoneId: zoneItem.ZoneId,
                                RouteOptions: zoneItem.RouteOptions
                            };
                            VRUIUtilsService.callDirectiveLoad(zoneItem.RouteOptionsAPI, payload, zoneItem.RouteOptionsLoadDeferred);
                        };
                    }
                }
            }

            function applyChanges(zoneChanges, zoneItem) {
                if (zoneItem.IsDirty) {
                    var zoneItemChanges = {
                        ZoneId: zoneItem.ZoneId
                    };

                    setDraftRateToChange(zoneItemChanges, zoneItem);
                    setDraftRateToClose(zoneItemChanges, zoneItem);

                    for (var i = 0; i < zoneItem.drillDownExtensionObject.drillDownDirectiveTabs.length; i++) {
                        var item = zoneItem.drillDownExtensionObject.drillDownDirectiveTabs[i];

                        if (item.directiveAPI && item.directiveAPI.applyChanges)
                            item.directiveAPI.applyChanges(zoneItemChanges);
                    }

                    zoneChanges.push(zoneItemChanges);
                }

                function setDraftRateToChange(zoneChanges, zoneItem) {
                    zoneChanges.NewRate = null;

                    if (zoneItem.NewRate) {
                        zoneChanges.NewRate = {
                            ZoneId: zoneItem.ZoneId,
                            NormalRate: zoneItem.NewRate,
                            BED: zoneItem.NewRateBED,
                            EED: zoneItem.NewRateEED,
                            OtherRates: getOtherRates(zoneItem)
                        };
                    }
                    //else if (zoneItem.CurrentRate != null) {
                    //    var otherRates = getOtherRates(zoneItem);
                    //    if (otherRates != null) {
                    //        zoneChanges.NewRate = {
                    //            ZoneId: zoneItem.ZoneId,
                    //            OtherRates: otherRates
                    //        };
                    //    }
                    //}
                }

                function getOtherRates(zoneItem) {
                    //TODO: get changed rates with current rates
                    return zoneItem.NewOtherRates;
                }
                function setDraftRateToClose(zoneChanges, zoneItem) {
                    zoneChanges.RateChange = null;

                    if (zoneItem.IsCurrentRateEditable && !compareDates(zoneItem.CurrentRateEED, zoneItem.currentRateEED)) {
                        zoneChanges.RateChange = {
                            RateId: zoneItem.CurrentRateId,
                            EED: zoneItem.CurrentRateEED
                        };
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
            }
        }
    }]);
