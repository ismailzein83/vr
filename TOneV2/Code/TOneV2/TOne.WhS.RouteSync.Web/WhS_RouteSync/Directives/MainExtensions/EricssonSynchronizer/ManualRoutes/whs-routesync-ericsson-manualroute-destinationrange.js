(function (app) {

    'use strict';

    ManualRouteDestinationRange.$inject = ["UtilsService", 'VRUIUtilsService'];

    function ManualRouteDestinationRange(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new ManualRouteDestinationRangeCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_RouteSync/Directives/MainExtensions/EricssonSynchronizer/ManualRoutes/Templates/EricssonManualRouteDestinationRangeTemplate.html"
        };

        function ManualRouteDestinationRangeCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;


            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.validateRange = function () {
                    if ($scope.scopeModel.toCode == undefined || $scope.scopeModel.toCode == '' || $scope.scopeModel.fromCode == undefined || $scope.scopeModel.toCode == '')
                        return null;
                    if ($scope.scopeModel.toCode <= $scope.scopeModel.fromCode)
                        return "To Code value should be greater than From Code value.";
                    if (String($scope.scopeModel.toCode).length != String($scope.scopeModel.fromCode).length)
                        return "To Code and From Code should be of same length.";
                    return null;
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    if (payload != undefined) {
                        if (payload.RouteDestinations != undefined) {
                            $scope.scopeModel.fromCode = payload.RouteDestinations.FromCode;
                            $scope.scopeModel.toCode = payload.RouteDestinations.ToCode;
                        }
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data = {
                        $type: "TOne.WhS.RouteSync.Ericsson.Entities.EricssonManualRouteDestinationRange, TOne.WhS.RouteSync.Ericsson",
                        FromCode: $scope.scopeModel.fromCode,
                        ToCode: $scope.scopeModel.toCode
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('whsRoutesyncEricssonManualrouteDestinationrange', ManualRouteDestinationRange);

})(app);