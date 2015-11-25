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
            ratePlanGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/WhS_Sales/Directives/Templates/RatePlanGridTemplate.html"
    };

    function RatePlanGrid($scope, ctrl) {
        this.initializeController = initializeController;

        function initializeController() {
            var gridAPI;
            var gridQuery;
            var gridDrillDownTabs;

            $scope.zoneItems = [];

            $scope.onGridReady = function (api) {
                gridAPI = api;

                gridDrillDownTabs = VRUIUtilsService.defineGridDrillDownTabs(buildGridDrillDownDefinitions(), gridAPI, null);

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getAPI());

                function buildGridDrillDownDefinitions() {
                    return [{
                        title: "Routing Product",
                        directive: "vr-whs-sales-rateplanroutingproduct",
                        loadDirective: function (zoneRoutingProductDirectiveAPI, zoneItem) {
                            zoneItem.ZoneRoutingProductDirectiveAPI = zoneRoutingProductDirectiveAPI;
                            return loadZoneRoutingProductDirective(zoneItem);
                        }
                    }];

                    function loadZoneRoutingProductDirective(zoneItem) {
                        var zoneRoutingProductDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        var payload = {
                            IsCurrentRoutingProductEditable: zoneItem.IsCurrentRoutingProductEditable,
                            CurrentRoutingProductId: zoneItem.CurrentRoutingProductId,
                            NewRoutingProductId: zoneItem.NewRoutingProductId,
                            CurrentBED: zoneItem.RoutingProductBED,
                            CurrentEED: zoneItem.RoutingProductEED
                        };

                        VRUIUtilsService.callDirectiveLoad(zoneItem.ZoneRoutingProductDirectiveAPI, payload, zoneRoutingProductDirectiveLoadPromiseDeferred);

                        return zoneRoutingProductDirectiveLoadPromiseDeferred.promise;
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
                            var zoneItemChanges = buildZoneItemChanges($scope.zoneItems[i]);

                            if (zoneItemChanges != null)
                                zoneChanges.push(zoneItemChanges);
                        }

                        if (zoneChanges.length == 0)
                            zoneChanges = null;

                        return zoneChanges;

                        function buildZoneItemChanges(zoneItem) {
                            var zoneItemChanges = null;

                            var newRate = buildNewRate(zoneItem);
                            var rateChange = (newRate != null) ? null : buildRateChange(zoneItem);
                            var newRoutingProduct = buildNewRoutingProduct(zoneItem);
                            var routingProductChange = (newRoutingProduct == null) ? buildRoutingProductChange(zoneItem) : null;

                            if (newRate != null || rateChange != null || newRoutingProduct != null || routingProductChange != null) {
                                zoneItemChanges = {
                                    ZoneId: zoneItem.ZoneId,
                                    NewRate: newRate,
                                    RateChange: rateChange,
                                    NewRoutingProduct: newRoutingProduct,
                                    RoutingProductChange: routingProductChange
                                };
                            }

                            return zoneItemChanges;

                            function buildNewRate(zoneItem) {
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

                            function buildRateChange(zoneItem) {
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
                            
                            function buildNewRoutingProduct(zoneItem) {
                                if (zoneItem.ZoneRoutingProductDirectiveAPI != undefined) {
                                    var defaultChanges = zoneItem.ZoneRoutingProductDirectiveAPI.getChanges();
                                    var newZoneRoutingProduct = null;

                                    if (defaultChanges != null && defaultChanges.NewRoutingProduct != null) {
                                        newZoneRoutingProduct = {
                                            $type: "TOne.WhS.Sales.Entities.RatePlanning.NewZoneRoutingProduct, TOne.WhS.Sales.Entities",
                                            ZoneId: zoneItem.ZoneId,
                                            RoutingProductId: defaultChanges.NewRoutingProduct.RoutingProductId,
                                            BED: defaultChanges.NewRoutingProduct.BED,
                                            EED: defaultChanges.NewRoutingProduct.EED
                                        };
                                    }

                                    return newZoneRoutingProduct;
                                }
                            }

                            function buildRoutingProductChange(zoneItem) {
                                if (zoneItem.ZoneRoutingProductDirectiveAPI != undefined) {
                                    var defaultChanges = zoneItem.ZoneRoutingProductDirectiveAPI.getChanges();
                                    var zoneRoutingProductChange = null;

                                    if (defaultChanges != null && defaultChanges.RoutingProductChange != null) {
                                        zoneRoutingProductChange = {
                                            $type: "TOne.WhS.Sales.Entities.RatePlanning.ZoneRoutingProductChange, TOne.WhS.Sales.Entities",
                                            ZoneRoutingProductId: zoneItem.ZoneId,
                                            EED: defaultChanges.RoutingProductChange.EED
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
                $scope.isLoadingGrid = true;

                return WhS_Sales_RatePlanAPIService.GetZoneItems(buildZoneItemInput()).then(function (response) {
                    if (response == undefined || response == null)
                        return;

                    var zoneItems = [];

                    for (var i = 0; i < response.length; i++) {
                        var zoneItem = response[i];
                        extendZoneItem(zoneItem);
                        gridDrillDownTabs.setDrillDownExtensionObject(zoneItem);
                        zoneItems.push(zoneItem);
                    }

                    gridAPI.addItemsToSource(zoneItems);
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                }).finally(function () {
                    $scope.isLoadingGrid = false;
                });

                function buildZoneItemInput() {
                    var pageInfo = gridAPI.getPageInfo();

                    return {
                        Filter: gridQuery,
                        FromRow: pageInfo.fromRow,
                        ToRow: pageInfo.toRow
                    };
                }

                function extendZoneItem(zoneItem) {
                    zoneItem.IsCurrentRateEditable = (zoneItem.IsCurrentRateEditable == null) ? false : zoneItem.IsCurrentRateEditable;

                    zoneItem.CurrentRateBED = (zoneItem.CurrentRateBED != null) ? new Date(zoneItem.CurrentRateBED) : null;
                    zoneItem.NewRateBED = (zoneItem.NewRateBED != null) ? new Date(zoneItem.NewRateBED) : null;

                    zoneItem.CurrentRateEED = (zoneItem.CurrentRateEED != null) ? new Date(zoneItem.CurrentRateEED) : null;
                    zoneItem.NewRateEED = (zoneItem.NewRateEED != null) ? new Date(zoneItem.NewRateEED) : null;

                    zoneItem.showNewRateBED = !isEmpty(zoneItem.NewRate);
                    zoneItem.showNewRateEED = !isEmpty(zoneItem.NewRate);

                    zoneItem.onNewRateChanged = function (zoneItem) {
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
                }
            }

            function isEmpty(value) {
                return (value == undefined || value == null || value == "");
            }
        }
    }
}]);
