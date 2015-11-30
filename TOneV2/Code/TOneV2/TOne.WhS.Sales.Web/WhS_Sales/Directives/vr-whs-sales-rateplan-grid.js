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

        function initCtrl() {
            var gridAPI;
            var gridQuery;
            var gridDrillDownTabs;

            $scope.zoneItems = [];

            $scope.onGridReady = function (api) {
                gridAPI = api;

                gridDrillDownTabs = VRUIUtilsService.defineGridDrillDownTabs(getGridDrillDownDefinitions(), gridAPI, null);

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getAPI());

                function getGridDrillDownDefinitions() {
                    return [{
                        title: "Routing Product",
                        directive: "vr-whs-sales-zoneroutingproduct",
                        loadDirective: function (zoneRoutingProductDirectiveAPI, zoneItem) {
                            zoneItem.ZoneRoutingProductDirectiveAPI = zoneRoutingProductDirectiveAPI;
                            return loadZoneRoutingProductDirective(zoneItem);
                        }
                    }];

                    function loadZoneRoutingProductDirective(zoneItem) {
                        var zoneRoutingProductLoadDeferred = UtilsService.createPromiseDeferred();

                        var payload = {
                            CurrentRoutingProductId: zoneItem.CurrentRoutingProductId,
                            CurrentRoutingProductName: zoneItem.CurrentRoutingProductName,
                            CurrentRoutingProductBED: zoneItem.CurrentRoutingProductBED,
                            CurrentRoutingProductEED: zoneItem.CurrentRoutingProductEED,
                            IsCurrentRoutingProductEditable: zoneItem.IsCurrentRoutingProductEditable,
                            NewRoutingProductId: zoneItem.NewRoutingProductId,
                            NewRoutingProductBED: zoneItem.NewRoutingProductBED,
                            NewRoutingProductEED: zoneItem.NewRoutingProductEED
                        };
                        
                        VRUIUtilsService.callDirectiveLoad(zoneItem.ZoneRoutingProductDirectiveAPI, payload, zoneRoutingProductLoadDeferred);

                        return zoneRoutingProductLoadDeferred.promise;
                    }
                }

                function getAPI() {
                    var api = {};

                    api.load = function (query) {
                        gridQuery = query;
                        gridAPI.clearDataAndContinuePaging();
                        return loadZoneItems();
                    };

                    api.getChanges = function () {
                        var zoneChanges = [];

                        for (var i = 0; i < $scope.zoneItems.length; i++) {
                            var item = $scope.zoneItems[i];

                            if (item.IsDirty || (item.ZoneRoutingProductDirectiveAPI != undefined && item.ZoneRoutingProductDirectiveAPI.isDirty()))
                                zoneChanges.push(getZoneItemChanges(item));
                        }

                        if (zoneChanges.length == 0)
                            zoneChanges = null;

                        return zoneChanges;

                        function getZoneItemChanges(zoneItem) {
                            var newRate = getNewRate(zoneItem);
                            var rateChange = getRateChange(zoneItem);
                            var newRoutingProduct = getNewRoutingProduct(zoneItem);
                            var routingProductChange = getRoutingProductChange(zoneItem);

                            return {
                                ZoneId: zoneItem.ZoneId,
                                NewRate: newRate,
                                RateChange: rateChange,
                                NewRoutingProduct: newRoutingProduct,
                                RoutingProductChange: routingProductChange
                            };

                            function getNewRate(zoneItem) {
                                var newRate = null;

                                if (!isEmpty(zoneItem.NewRate)) {
                                    newRate = {
                                        ZoneId: zoneItem.ZoneId,
                                        NormalRate: zoneItem.NewRate,
                                        BED: zoneItem.NewRateBED,
                                        EED: zoneItem.NewRateEED
                                    };
                                }

                                return newRate;
                            }

                            function getRateChange(zoneItem) {
                                var rateChange = null;

                                if (zoneItem.IsCurrentRateEditable && !compareDates(zoneItem.CurrentRateEED, zoneItem.NewRateEED)) {
                                    return {
                                        RateId: zoneItem.CurrentRateId,
                                        EED: zoneItem.NewRateEED
                                    };
                                }

                                return rateChange;

                                function compareDates(date1, date2) {
                                    if (!isEmpty(date1) && !isEmpty(date2))
                                        return (date1.getDay() == date2.getDay() && date1.getMonth() == date2.getMonth() && date1.getYear() == date2.getYear());
                                    else if (isEmpty(date1) && isEmpty(date2))
                                        return true;
                                    else
                                        return false;
                                }
                            }
                            
                            function getNewRoutingProduct(zoneItem) {
                                if (zoneItem.ZoneRoutingProductDirectiveAPI != undefined) {
                                    var routingProductChanges = zoneItem.ZoneRoutingProductDirectiveAPI.getChanges();
                                    var newZoneRoutingProduct = null;

                                    if (routingProductChanges != null && routingProductChanges.NewRoutingProduct != null) {
                                        newZoneRoutingProduct = {
                                            ZoneId: zoneItem.ZoneId,
                                            ZoneRoutingProductId: routingProductChanges.NewRoutingProduct.RoutingProductId,
                                            BED: routingProductChanges.NewRoutingProduct.BED,
                                            EED: routingProductChanges.NewRoutingProduct.EED
                                        };
                                    }

                                    return newZoneRoutingProduct;
                                }
                            }

                            function getRoutingProductChange(zoneItem) {
                                if (zoneItem.ZoneRoutingProductDirectiveAPI != undefined) {
                                    var routingProductChanges = zoneItem.ZoneRoutingProductDirectiveAPI.getChanges();
                                    var zoneRoutingProductChange = null;

                                    if (routingProductChanges != null && routingProductChanges.RoutingProductChange != null) {
                                        zoneRoutingProductChange = {
                                            ZoneRoutingProductId: routingProductChanges.RoutingProductChange.RoutingProductId,
                                            EED: routingProductChanges.RoutingProductChange.EED
                                        };
                                    }

                                    return zoneRoutingProductChange;
                                }
                            }
                        }
                    };

                    return api;
                }
            };

            $scope.loadMoreData = function () {
                return loadZoneItems();
            };

            function loadZoneItems() {
                var deferred = UtilsService.createPromiseDeferred();
                $scope.isLoadingGrid = true;

                WhS_Sales_RatePlanAPIService.GetZoneItems(getZoneItemInput()).then(function (response) {
                    if (response != null) {
                        var promises = [];
                        var zoneItems = [];

                        for (var i = 0; i < response.length; i++) {
                            var zoneItem = response[i];
                            extendZoneItem(zoneItem);
                            gridDrillDownTabs.setDrillDownExtensionObject(zoneItem);
                            promises.push(zoneItem.RouteOptionsLoadDeferred.promise);

                            zoneItems.push(zoneItem);
                        }

                        gridAPI.addItemsToSource(zoneItems);

                        UtilsService.waitMultiplePromises(promises).then(function () {
                            deferred.resolve();
                        }).catch(function (error) {
                            handleError(error);
                        }).finally(function () {
                            $scope.isLoadingGrid = false;
                        });
                    }
                    else
                        deferred.resolve();
                }).catch(function (error) {
                    $scope.isLoadingGrid = false;
                    handleError(error);
                });

                return deferred.promise;

                function getZoneItemInput() {
                    var pageInfo = gridAPI.getPageInfo();

                    return {
                        Filter: gridQuery,
                        FromRow: pageInfo.fromRow,
                        ToRow: pageInfo.toRow
                    };
                }

                function extendZoneItem(zoneItem) {
                    zoneItem.IsDirty = false;
                    defineRouteOptionProperties(zoneItem);

                    zoneItem.IsCurrentRateEditable = (zoneItem.IsCurrentRateEditable == null) ? false : zoneItem.IsCurrentRateEditable;

                    zoneItem.CurrentRateBED = (zoneItem.CurrentRateBED != null) ? new Date(zoneItem.CurrentRateBED) : null;
                    zoneItem.NewRateBED = (zoneItem.NewRateBED != null) ? new Date(zoneItem.NewRateBED) : null;

                    zoneItem.CurrentRateEED = (zoneItem.CurrentRateEED != null) ? new Date(zoneItem.CurrentRateEED) : null;
                    zoneItem.NewRateEED = (zoneItem.NewRateEED != null) ? new Date(zoneItem.NewRateEED) : null;

                    zoneItem.showNewRateBED = !isEmpty(zoneItem.NewRate);
                    zoneItem.showNewRateEED = !isEmpty(zoneItem.NewRate);

                    zoneItem.onNewRateChanged = function (zoneItem) {
                        zoneItem.IsDirty = true;

                        if (!isEmpty(zoneItem.NewRate)) {
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

                    function defineRouteOptionProperties(zoneItem) {
                        zoneItem.RouteOptionsReadyDeferred = UtilsService.createPromiseDeferred();

                        zoneItem.onRouteOptionsReady = function (api) {
                            zoneItem.RouteOptionsAPI = api;
                            zoneItem.RouteOptionsReadyDeferred.resolve();
                        };

                        zoneItem.RouteOptionsLoadDeferred = UtilsService.createPromiseDeferred();
                        zoneItem.RouteOptionsReadyDeferred.promise.then(function () {
                            var payload = {
                                SaleZoneId: zoneItem.ZoneId,
                                RoutingProductId: zoneItem.EffectiveRoutingProductId,
                                RouteOptions: zoneItem.RouteOptions
                            };
                            VRUIUtilsService.callDirectiveLoad(zoneItem.RouteOptionsAPI, payload, zoneItem.RouteOptionsLoadDeferred);
                        });
                    }
                }

                function handleError(error) {
                    deferred.reject();
                    VRNotificationService.notifyException(error, $scope);
                }
            }

            function isEmpty(value) {
                return (value == undefined || value == null || value == "");
            }
        }
    }
}]);
