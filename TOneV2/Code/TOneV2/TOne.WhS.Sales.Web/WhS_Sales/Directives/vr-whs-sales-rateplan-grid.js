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
                    ctrl.onReady(buildAPI());

                function buildGridDrillDownDefinitions() {
                    return [{
                        title: "Routing Product",
                        directive: "vr-whs-sales-defaultroutingproduct",
                        loadDirective: function (zoneRoutingProductDirectiveAPI, zoneItem) {
                            zoneItem.ZoneRoutingProductDirectiveAPI = zoneRoutingProductDirectiveAPI;
                            return loadZoneRoutingProductDirective(zoneItem);
                        }
                    }];

                    function loadZoneRoutingProductDirective(zoneItem) {
                        var zoneRoutingProductDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        var payload = {
                            IsCurrentDefaultRoutingProductEditable: zoneItem.IsCurrentRoutingProductEditable,
                            CurrentDefaultRoutingProductId: zoneItem.CurrentRoutingProductId,
                            NewDefaultRoutingProductId: zoneItem.NewRoutingProductId,
                            CurrentBED: zoneItem.RoutingProductBED,
                            CurrentEED: zoneItem.RoutingProductEED
                        };

                        console.log(payload);

                        VRUIUtilsService.callDirectiveLoad(zoneItem.ZoneRoutingProductDirectiveAPI, payload, zoneRoutingProductDirectiveLoadPromiseDeferred);

                        return zoneRoutingProductDirectiveLoadPromiseDeferred.promise;
                    }
                }

                function buildAPI() {
                    var directiveAPI = {};

                    directiveAPI.load = function (query) {
                        gridQuery = query;
                        gridAPI.clearDataAndContinuePaging();
                        return loadZoneItems();
                    };

                    directiveAPI.getZoneChanges = function () {
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
                                var currentRate = zoneItem.CurrentRate;
                                var newRate = zoneItem.NewRate;

                                if (!isEmpty(zoneItem.NewRate)) {
                                    newRate = {
                                        ZoneId: zoneItem.ZoneId,
                                        NormalRate: zoneItem.NewRate,
                                        BED: zoneItem.RateNewBED,
                                        EED: zoneItem.RateNewEED
                                    };
                                }

                                return newRate;
                            }

                            function buildRateChange(zoneItem) {
                                var rateChange = null;

                                if (!compareDates(zoneItem.RateEED, zoneItem.RateNewEED)) {
                                    return {
                                        RateId: zoneItem.CurrentRateId,
                                        EED: zoneItem.RateNewEED
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
                                    var defaultChanges = zoneItem.ZoneRoutingProductDirectiveAPI.getDefaultChanges();
                                    var newZoneRoutingProduct = null;

                                    if (defaultChanges != null && defaultChanges.NewDefaultRoutingProduct != null) {
                                        newZoneRoutingProduct = {
                                            $type: "TOne.WhS.Sales.Entities.RatePlanning.NewZoneRoutingProduct, TOne.WhS.Sales.Entities",
                                            ZoneId: zoneItem.ZoneId,
                                            RoutingProductId: defaultChanges.NewDefaultRoutingProduct.DefaultRoutingProductId,
                                            BED: defaultChanges.NewDefaultRoutingProduct.BED,
                                            EED: defaultChanges.NewDefaultRoutingProduct.EED
                                        };
                                    }

                                    return newZoneRoutingProduct;
                                }
                            }

                            function buildRoutingProductChange(zoneItem) {
                                if (zoneItem.ZoneRoutingProductDirectiveAPI != undefined) {
                                    var defaultChanges = zoneItem.ZoneRoutingProductDirectiveAPI.getDefaultChanges();
                                    var zoneRoutingProductChange = null;

                                    if (defaultChanges != null && defaultChanges.DefaultRoutingProductChange != null) {
                                        zoneRoutingProductChange = {
                                            $type: "TOne.WhS.Sales.Entities.RatePlanning.ZoneRoutingProductChange, TOne.WhS.Sales.Entities",
                                            ZoneRoutingProductId: zoneItem.ZoneId,
                                            EED: defaultChanges.DefaultRoutingProductChange.EED
                                        };
                                    }

                                    return zoneRoutingProductChange;
                                }
                            }
                        }
                    };

                    return directiveAPI;
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
                    zoneItem.PreviousNewRate = zoneItem.NewRate;
                    defineDateProperties(zoneItem);

                    zoneItem.onNewRateChanged = function (zoneItem) {
                        zoneItem.DisableBED = isEmpty(zoneItem.NewRate);
                        zoneItem.DisableEED = (isEmpty(zoneItem.NewRate) && !zoneItem.IsCurrentRateEditable);

                        if (!isEmpty(zoneItem.NewRate) && isEmpty(zoneItem.PreviousNewRate)) {
                            zoneItem.RateNewBED = (zoneItem.CurrentRate == null || Number(zoneItem.NewRate) > zoneItem.CurrentRate) ?
                                new Date(new Date().setDate(new Date().getDate() + 7)) : new Date();
                            zoneItem.RateNewEED = null;
                        }
                        else if (isEmpty(zoneItem.NewRate) && !isEmpty(zoneItem.PreviousNewRate)) {
                            zoneItem.RateNewBED = (zoneItem.CurrentRate != null && zoneItem.RateBED != null) ? new Date(zoneItem.RateBED) : null;
                            zoneItem.RateNewEED = (zoneItem.CurrentRate != null && zoneItem.RateEED != null) ? new Date(zoneItem.RateEED) : null;
                        }

                        zoneItem.PreviousNewRate = zoneItem.NewRate;
                    };

                    function defineDateProperties(zoneItem) {
                        zoneItem.DisableBED = isEmpty(zoneItem.NewRate);
                        zoneItem.DisableEED = (isEmpty(zoneItem.NewRate) && !zoneItem.IsCurrentRateEditable);

                        zoneItem.RateBED = (zoneItem.RateBED != null) ? new Date(zoneItem.RateBED) : null;
                        zoneItem.RateNewBED = (zoneItem.RateBED != null) ? new Date(zoneItem.RateBED) : null;

                        zoneItem.RateEED = (zoneItem.RateEED != null) ? new Date(zoneItem.RateEED) : null;
                        zoneItem.RateNewEED = (zoneItem.RateEED != null) ? new Date(zoneItem.RateEED) : null;
                    }
                }
            }

            function isEmpty(value) {
                return (value == undefined || value == null || value == "");
            }
        }
    }
}]);
