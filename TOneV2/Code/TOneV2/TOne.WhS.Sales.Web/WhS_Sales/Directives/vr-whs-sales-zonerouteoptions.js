"use strict";

app.directive("vrWhsSalesZonerouteoptions", ["UtilsService", "VRUIUtilsService",
function (UtilsService, VRUIUtilsService) {
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

        function initCtrl() {
            ctrl.routeOptions = [];

            getAPI();

            function getAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        for (var i = 0; i < payload.RouteOptions.length; i++)
                            ctrl.routeOptions.push(payload.RouteOptions[i]);
                    }
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }
}]);
