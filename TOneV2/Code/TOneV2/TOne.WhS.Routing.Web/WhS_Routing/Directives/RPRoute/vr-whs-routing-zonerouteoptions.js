"use strict";

app.directive("vrWhsRoutingZonerouteoptions", ["WhS_Routing_RPRouteService", "UtilsService",
function (WhS_Routing_RPRouteService, UtilsService) {
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

        function initCtrl() {
            ctrl.routeOptions = [];

            ctrl.viewSupplier = function (routeOption) {
                WhS_Routing_RPRouteService.viewRPRouteOptionSupplier(routingDatabaseId, routingProductId, saleZoneId, routeOption.Entity.SupplierId, currencyId);
            };

            defineAPI();
        }

        function buildTitle(supplierName, percentage) {
            var result = supplierName;
            if (percentage) {
                result = result + ' Percentage: ' + percentage + '%';
            }
            return result;
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

                    ctrl.routeOptions = [];
                    if (payload.RouteOptions)
                        for (var i = 0; i < payload.RouteOptions.length; i++) {

                            var currentItem = payload.RouteOptions[i];
                            currentItem.title = buildTitle(currentItem.SupplierName, currentItem.Entity.Percentage);
                            ctrl.routeOptions.push(currentItem);
                        }
                }
            };

            if (ctrl.onReady && typeof (ctrl.onReady) == "function")
                ctrl.onReady(api);
        }
    }
}]);
