"use strict";

app.directive("vrWhsSalesRateplanGrid", ["WhS_Sales_RatePlanAPIService", "UtilsService", "VRNotificationService", function (WhS_Sales_RatePlanAPIService, UtilsService, VRNotificationService) {

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

            $scope.zoneItems = [];

            $scope.onGridReady = function (api) {
                gridAPI = api;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(buildDirectiveAPI());

                function buildDirectiveAPI() {
                    var directiveAPI = {};

                    directiveAPI.loadGrid = function (query) {
                        gridQuery = query;
                        gridAPI.clearDataAndContinuePaging();
                        return loadZoneItems();
                    };

                    directiveAPI.getChanges = function () {
                        return {
                            RateChanges: {
                                NewRates: getNewRates()
                            }
                        };

                        function getNewRates() {
                            var newRates = [];

                            for (var i = 0; i < $scope.zoneItems.length; i++) {
                                var zoneItem = $scope.zoneItems[i];

                                if (zoneItem.NewRate != undefined && zoneItem.NewRate != null)
                                    newRates.push(buildNewRate(zoneItem));
                            }

                            if (newRates.length == 0)
                                newRates = null;

                            return newRates

                            function buildNewRate(zoneItem) {
                                return {
                                    ZoneId: zoneItem.ZoneId,
                                    NormalRate: zoneItem.NewRate,
                                    BED: zoneItem.RateBED,
                                    EED: zoneItem.RateEED
                                };
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
                    zoneItem.isDirty = false;
                    zoneItem.DisableBED = true;
                    zoneItem.DisableEED = true;

                    zoneItem.onNewRateChanged = function (dataItem) {
                        if (!dataItem.isDirty) {
                            dataItem.isDirty = true;
                            dataItem.DisableBED = false;
                            dataItem.DisableEED = false;

                            dataItem.RateBED = (dataItem.CurrentRate == null || dataItem.NewRate > dataItem.CurrentRate) ?
                                new Date(new Date().setDate(new Date().getDate() + 7)) : new Date();
                        }
                    };

                    zoneItem.onChildViewReady = function (api) {
                        zoneItem.ChildViewAPI = api;
                        delete zoneItem.onChildViewReady;
                    };
                }
            }
        }
    }
}]);
