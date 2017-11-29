"use strict";

app.directive("vrWhsRoutingZonerouteoptions", ["WhS_Routing_RPRouteService", "WhS_Routing_SupplierStatusEnum", "WhS_BE_ZoneRouteOptionsEnum", "UtilsService",
function (WhS_Routing_RPRouteService, WhS_Routing_SupplierStatusEnum, WhS_BE_ZoneRouteOptionsEnum, UtilsService) {
    return {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var zoneRouteOptions = new ZoneRouteOptions(ctrl, $scope);
            zoneRouteOptions.initCtrl();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/WhS_Routing/Directives/RPRoute/Templates/ZoneRouteOptionsTemplate.html"
    };

    function ZoneRouteOptions(ctrl, $scope) {
        this.initCtrl = initCtrl;

        var routingProductId;
        var saleZoneId;
        var routingDatabaseId;
        var currencyId;
        var display;

        function initCtrl() {
            ctrl.routeOptions = [];

            ctrl.viewSupplier = function (routeOption) {
                WhS_Routing_RPRouteService.viewRPRouteOptionSupplier(routingDatabaseId, routingProductId, saleZoneId, routeOption.Entity.SupplierId, currencyId);
            };

            defineAPI();
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                ctrl.routeOptions = [];

                if (payload != undefined) {
                    routingDatabaseId = payload.RoutingDatabaseId;
                    routingProductId = payload.RoutingProductId;
                    saleZoneId = payload.SaleZoneId;
                    currencyId = payload.CurrencyId;
                    if (currencyId == undefined)
                        currencyId = payload.currencyId;

                    display = payload.display;

                    ctrl.routeOptions = [];
                    if (payload.RouteOptions) {
                        for (var i = 0; i < payload.RouteOptions.length; i++) {
                            var currentItem = payload.RouteOptions[i];
                            currentItem.title = buildTitle(currentItem.SupplierName, currentItem.Entity.Percentage, currentItem.ACD, currentItem.ASR);
                            currentItem.titleToDisplay = buildTitleToDisplay(currentItem, display);

                            if (currentItem.Entity.SupplierStatus == WhS_Routing_SupplierStatusEnum.Block.value)
                                currentItem.Color = 'Red';

                            ctrl.routeOptions.push(currentItem);
                        }
                    }
                }
            };

            if (ctrl.onReady && typeof (ctrl.onReady) == "function")
                ctrl.onReady(api);
        }

        function buildTitle(supplierName, percentage, acd, asr) {
            var result = supplierName;
            if (percentage) {
                result = result + ' ' + percentage + '%';
            }
            if (asr) {
                result = result + ', ASR:' + asr;
            }
            if (acd) {
                result = result + ', ACD:' + acd;
            }
            return result;
        }
        function buildTitleToDisplay(currentItem, display) {

            switch (display) {

                case WhS_BE_ZoneRouteOptionsEnum.SupplierRateWithNameAndPercentage.value: currentItem.isNumber = false; return currentItem.title;

                case WhS_BE_ZoneRouteOptionsEnum.SupplierRate.value: currentItem.isNumber = true; return currentItem.ConvertedSupplierRate;

                default: currentItem.isNumber = true; return currentItem.ConvertedSupplierRate;
            }
        }

        //function getRowStyle(optionStatus) {

        //    if (optionStatus == WhS_Routing_SupplierStatusEnum.PartialActive.value)
        //        return '#d0c89e'; //'BurlyWood';

        //    if (optionStatus == WhS_Routing_SupplierStatusEnum.Block.value)
        //        return '#efa2a2'; //'Red';
        //}
    }
}]);
