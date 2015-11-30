"use strict";

app.directive("vrWhsSalesZonerouteoptions", ["WhS_Sales_MainService", "UtilsService",
function (WhS_Sales_MainService, UtilsService) {
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
        templateUrl: "/Client/Modules/WhS_Sales/Directives/Templates/ZoneRouteOptionsTemplate.html"
    };

    function ZoneRouteOptions(ctrl, $scope) {
        this.initCtrl = initCtrl;

        var routingProductId;
        var saleZoneId;

        function initCtrl() {
            ctrl.routeOptions = [];

            ctrl.viewSupplier = function (routeOption) {
                WhS_Sales_MainService.viewRPRouteOptionSupplier(routingProductId, saleZoneId, routeOption.Entity.SupplierId, routeOption.Entity.SupplierName);
            };

            getAPI();
        }

        function getAPI() {
            var api = {};

            api.load = function (payload) {
                if (payload != undefined) {
                    saleZoneId = payload.SaleZoneId;
                    routingProductId = payload.RoutingProductId;

                    for (var i = 0; i < payload.RouteOptions.length; i++)
                        ctrl.routeOptions.push(payload.RouteOptions[i]);
                }
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);
