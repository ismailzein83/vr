(function (app) {

    'use strict';

    RouteSyncReader.$inject = ["UtilsService", 'VRUIUtilsService', 'WhS_Routing_TOneRouteRangeType'];

    function RouteSyncReader(UtilsService, VRUIUtilsService, WhS_Routing_TOneRouteRangeType) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var _routeSyncReader = new RouteSyncReader($scope, ctrl, $attrs);
                _routeSyncReader.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_Routing/Directives/Extensions/RouteReader/Templates/RouteSyncReaderTemplate.html"

        };
        function RouteSyncReader($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.routeRangeType = UtilsService.getArrayEnum(WhS_Routing_TOneRouteRangeType);

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var routeReader;

                    if (payload != undefined) {
                        routeReader = payload.routeReader;
                    }

                    if (routeReader != undefined) {
                        $scope.scopeModel.selectedRouteRangeType = UtilsService.getEnum(WhS_Routing_TOneRouteRangeType, "value", routeReader.RangeType);
                    }
                };

                api.getData = function () {
                    var data = {
                        $type: "TOne.WhS.Routing.Business.Extensions.RouteSyncReader, TOne.WhS.Routing.Business",
                        RangeType: $scope.scopeModel.selectedRouteRangeType.description
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrWhsRoutingRoutesyncreader', RouteSyncReader);

})(app);