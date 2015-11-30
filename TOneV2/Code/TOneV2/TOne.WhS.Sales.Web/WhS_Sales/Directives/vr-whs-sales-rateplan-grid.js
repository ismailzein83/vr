﻿"use strict";

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
            }];
        }

        function getAPI() {
            var api = {};

            api.load = function (query) {
                gridQuery = query;
                gridAPI.clearDataAndContinuePaging();
                return loadZoneItems();
            };

            api.applyChanges = function (changes) {
                var zoneChanges = [];

                for (var i = 0; i < $scope.zoneItems.length; i++) {
                    var item = $scope.zoneItems[i];

                    if (item.IsDirty)
                        applyChanges(zoneChanges, item);
                }

                changes.ZoneChanges = zoneChanges.length > 0 ? zoneChanges : null;
            };

            return api;
        }

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

        function applyChanges(zoneChanges, zoneItem) {
            if (zoneItem.IsDirty) {
                var zoneItemChanges = {
                    ZoneId: zoneItem.ZoneId
                };

                setNewRate(zoneItemChanges, zoneItem);
                setRateChange(zoneItemChanges, zoneItem);

                for (var i = 0; i < zoneItem.drillDownExtensionObject.drillDownDirectiveTabs.length; i++) {
                    var item = zoneItem.drillDownExtensionObject.drillDownDirectiveTabs[i];

                    if (item.directiveAPI)
                        item.directiveAPI.applyChanges(zoneItemChanges);
                }

                zoneChanges.push(zoneItemChanges);
            }
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

            if (zoneItem.IsCurrentRateEditable && !compareDates(zoneItem.CurrentRateEED, zoneItem.NewRateEED)) {
                zoneChanges.RateChange = {
                    RateId: zoneItem.CurrentRateId,
                    EED: zoneItem.NewRateEED
                };
            }
        }

        function compareDates(date1, date2) {
            if (date1 && date2)
                return date1.getTime() == date2.getTime();
            else if (!date1 && !date2)
                return true;
            else
                return false;
        }
    }
}]);
