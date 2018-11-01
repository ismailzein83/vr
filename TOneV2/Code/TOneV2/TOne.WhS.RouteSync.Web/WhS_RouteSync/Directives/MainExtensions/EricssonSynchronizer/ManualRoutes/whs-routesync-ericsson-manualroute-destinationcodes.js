(function (app) {

    'use strict';

    ManualRouteDestinationCodes.$inject = ["UtilsService", 'VRUIUtilsService'];

    function ManualRouteDestinationCodes(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new ManualRouteDestinationCodesCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_RouteSync/Directives/MainExtensions/EricssonSynchronizer/ManualRoutes/Templates/EricssonManualRouteDestinationCodesTemplate.html"
        };

        function ManualRouteDestinationCodesCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;


            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.destinationCodes = [];

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    if (payload != undefined) {
                        if (payload.RouteDestinations != undefined) {
                            $scope.scopeModel.destinationCodes = payload.RouteDestinations.DestinationCodes;
                        }
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var codes = [];
                    if ($scope.scopeModel.destinationCodes != undefined && $scope.scopeModel.destinationCodes.length > 0) {
                        for (var i = 0; i < $scope.scopeModel.destinationCodes.length; i++) {
                            codes.push(parseInt($scope.scopeModel.destinationCodes[i]));
                        }
                    }
                    var data = {
                        $type: "TOne.WhS.RouteSync.Ericsson.Entities.EricssonManualRouteDestinationCodes, TOne.WhS.RouteSync.Ericsson",
                        DestinationCodes: $scope.scopeModel.destinationCodes
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('whsRoutesyncEricssonManualrouteDestinationcodes', ManualRouteDestinationCodes);

})(app);