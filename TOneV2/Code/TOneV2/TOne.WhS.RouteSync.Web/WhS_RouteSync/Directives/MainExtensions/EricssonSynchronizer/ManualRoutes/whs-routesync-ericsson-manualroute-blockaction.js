(function (app) {

    'use strict';

    ManualRouteBlockAction.$inject = ["UtilsService", 'VRUIUtilsService'];

    function ManualRouteBlockAction(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new ManualRouteBlockActionCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_RouteSync/Directives/MainExtensions/EricssonSynchronizer/ManualRoutes/Templates/EricssonManualRouteBlockActionTemplate.html"
        };

        function ManualRouteBlockActionCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;


            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data = {
                        $type: "TOne.WhS.RouteSync.Ericsson.Entities.EricssonManualRouteBlockAction, TOne.WhS.RouteSync.Ericsson",
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('whsRoutesyncEricssonManualrouteBlockaction', ManualRouteBlockAction);

})(app);