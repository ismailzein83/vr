(function (app) {

    'use strict';

    ManualRouteOriginationCodes.$inject = ["UtilsService", 'VRUIUtilsService'];

    function ManualRouteOriginationCodes(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new ManualRouteOriginationCodesCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_RouteSync/Directives/MainExtensions/EricssonSynchronizer/ManualRoutes/Templates/EricssonManualRouteOriginationCodesTemplate.html"
        };

        function ManualRouteOriginationCodesCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;


            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.originationCodes = [];

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    if (payload != undefined) {
                        if (payload.RouteOriginations != undefined) {
                            $scope.scopeModel.originationCodes = payload.RouteOriginations.OriginationCodes;
                        }
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var codes = [];
                    if ($scope.scopeModel.originationCodes != undefined && $scope.scopeModel.originationCodes.length > 0) {
                        for (var i = 0; i < $scope.scopeModel.originationCodes.length; i++) {
                            codes.push(parseInt($scope.scopeModel.originationCodes[i]));
                        }
                    }
                    var data = {
                        $type: "TOne.WhS.RouteSync.Ericsson.Entities.EricssonManualRouteOriginationCodes, TOne.WhS.RouteSync.Ericsson",
                        OriginationCodes: $scope.scopeModel.originationCodes
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('whsRoutesyncEricssonManualrouteOriginationcodes', ManualRouteOriginationCodes);

})(app);