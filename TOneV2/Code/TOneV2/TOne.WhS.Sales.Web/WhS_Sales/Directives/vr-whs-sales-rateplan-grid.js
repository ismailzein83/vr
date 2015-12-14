"use strict";

app.directive("vrWhsSalesRateplanGrid", ["WhS_Sales_RatePlanAPIService", "UtilsService", "VRUIUtilsService", "VRNotificationService",
    function (WhS_Sales_RatePlanAPIService, UtilsService, VRUIUtilsService, VRNotificationService) {
    return {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ratePlanGrid = new RatePlanGrid($scope, ctrl);
            ratePlanGrid.initCtrl();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/WhS_Sales/Directives/Templates/RatePlanGridTemplate.html"
    };

    function RatePlanGrid($scope, ctrl) {
        this.initCtrl = initCtrl;

        var gridAPI;
        var gridQuery;
        var gridDrillDownTabs;

        function initCtrl() {
            $scope.zoneItems = [];
            $scope.connector = {
                costCalculationMethods: []
            };

            $scope.onGridReady = function (api) {
                gridAPI = api;
                gridDrillDownTabs = VRUIUtilsService.defineGridDrillDownTabs(getGridDrillDownDefinitions(), gridAPI, null);

                if (ctrl.onReady && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getAPI());
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
        }

        function getAPI() {
            var api = {};

            api.load = function (query) {
                gridQuery = query;
                
                if (query.CostCalculationMethods) {
                    $scope.connector.costCalculationMethods = [];

                    for (var i = 0; i < query.CostCalculationMethods.length; i++)
                        $scope.connector.costCalculationMethods.push(query.CostCalculationMethods[i]);
                }

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

            return api;
        }

        function loadZoneItems() {
            var promises = [];

            $scope.isLoading = true;
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

            function extendZoneItem(zoneItem) {
                zoneItem.IsDirty = false;
                setRouteOptionProperties(zoneItem);

                zoneItem.IsCurrentRateEditable = (zoneItem.IsCurrentRateEditable == null) ? false : zoneItem.IsCurrentRateEditable;

                zoneItem.CurrentRateBED = (zoneItem.CurrentRateBED != null) ? new Date(zoneItem.CurrentRateBED) : null;
                zoneItem.NewRateBED = (zoneItem.NewRateBED != null) ? new Date(zoneItem.NewRateBED) : null;

                zoneItem.CurrentRateEED = (zoneItem.CurrentRateEED != null) ? new Date(zoneItem.CurrentRateEED) : null;
                zoneItem.NewRateEED = (zoneItem.NewRateEED != null) ? new Date(zoneItem.NewRateEED) : null;

                zoneItem.showNewRateBED = zoneItem.NewRate;
                zoneItem.showNewRateEED = zoneItem.NewRate;

                zoneItem.onNewRateChanged = function (zoneItem) {
                    zoneItem.IsDirty = true;

                    if (zoneItem.NewRate) {
                        zoneItem.showNewRateBED = true;
                        zoneItem.showNewRateEED = true;

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

                zoneItem.onNewRateEEDChange = function (zoneItem) {
                    zoneItem.IsDirty = true;
                };

                zoneItem.refreshZoneItem = function (zoneItem) {
                    var promises = [];

                    var saveChangesPromise = saveZoneItemChanges();
                    promises.push(saveChangesPromise);

                    saveChangesPromise.then(function () {
                        var zoneItemGetPromise = WhS_Sales_RatePlanAPIService.GetZoneItem(getZoneItemInput());
                        promises.push(zoneItemGetPromise);

                        var routeOptionsLoadDeferred = UtilsService.createPromiseDeferred();
                        promises.push(routeOptionsLoadDeferred.promise);

                        zoneItemGetPromise.then(function (response) {
                            console.log(response);
                            if (response) {
                                var gridZoneItem = UtilsService.getItemByVal($scope.zoneItems, response.ZoneId, "ZoneId");

                                if (gridZoneItem) {
                                    gridZoneItem.EffectiveRoutingProductId = response.EffectiveRoutingProductId;
                                    gridZoneItem.EffectiveRoutingProductName = response.EffectiveRoutingProductName;

                                    routeOptionsLoadDeferred.promise.finally(function () {
                                        zoneItem.isLoadingRouteOptions = false;
                                    });

                                    var payload = {
                                        SaleZoneId: response.ZoneId,
                                        RoutingProductId: response.EffectiveRoutingProductId,
                                        RouteOptions: response.RouteOptions
                                    };

                                    zoneItem.isLoadingRouteOptions = true;
                                    VRUIUtilsService.callDirectiveLoad(zoneItem.RouteOptionsAPI, payload, routeOptionsLoadDeferred);
                                }
                            }
                            else
                                routeOptionsLoadDeferred.reject();
                        });
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
                            PolicyConfigId: gridQuery.RPRoutePolicyConfigId,
                            NumberOfOptions: gridQuery.NumberOfOptions,
                            CostCalculationMethods: gridQuery.CostCalculationMethods
                        };
                    }
                };

                function setRouteOptionProperties(zoneItem) {
                    zoneItem.RouteOptionsReadyDeferred = UtilsService.createPromiseDeferred();

                    zoneItem.onRouteOptionsReady = function (api) {
                        zoneItem.RouteOptionsAPI = api;
                        zoneItem.RouteOptionsReadyDeferred.resolve();
                    };

                    zoneItem.RouteOptionsLoadDeferred = UtilsService.createPromiseDeferred();
                    zoneItem.RouteOptionsReadyDeferred.promise.then(function () {
                        var payload = {
                            RoutingDatabaseId: gridQuery.RoutingDatabaseId,
                            RoutingProductId: zoneItem.EffectiveRoutingProductId,
                            SaleZoneId: zoneItem.ZoneId,
                            RouteOptions: zoneItem.RouteOptions
                        };
                        VRUIUtilsService.callDirectiveLoad(zoneItem.RouteOptionsAPI, payload, zoneItem.RouteOptionsLoadDeferred);
                    });
                }
            }
        }

        function applyChanges(zoneChanges, zoneItem) {
            if (zoneItem.IsDirty) {
                var zoneItemChanges = {
                    ZoneId: zoneItem.ZoneId
                };

                setNewRate(zoneItemChanges, zoneItem);
                setRateChange(zoneItemChanges, zoneItem);

                for (var i = 0; i < zoneItem.drillDownExtensionObject.drillDownDirectiveTabs.length; i++) {
                    var item = zoneItem.drillDownExtensionObject.drillDownDirectiveTabs[i];

                    if (item.directiveAPI && item.directiveAPI.applyChanges)
                        item.directiveAPI.applyChanges(zoneItemChanges);
                }

                zoneChanges.push(zoneItemChanges);
            }

            function setNewRate(zoneChanges, zoneItem) {
                zoneChanges.NewRate = null;

                if (zoneItem.NewRate) {
                    zoneChanges.NewRate = {
                        ZoneId: zoneItem.ZoneId,
                        NormalRate: zoneItem.NewRate,
                        BED: zoneItem.NewRateBED,
                        EED: zoneItem.NewRateEED
                    };
                }
            }

            function setRateChange(zoneChanges, zoneItem) {
                zoneChanges.RateChange = null;
                return; // To be removed
                if (zoneItem.IsCurrentRateEditable && !compareDates(zoneItem.CurrentRateEED, zoneItem.NewRateEED)) {
                    zoneChanges.RateChange = {
                        RateId: zoneItem.CurrentRateId,
                        EED: zoneItem.NewRateEED
                    };
                }

                function compareDates(date1, date2) {
                    if (date1 && date1.getTime && date2 && date2.getTime)
                        return date1.getTime() == date2.getTime();
                    else if (!date1 && !date2)
                        return true;
                    else
                        return false;
                }
            }
        }
    }
}]);
