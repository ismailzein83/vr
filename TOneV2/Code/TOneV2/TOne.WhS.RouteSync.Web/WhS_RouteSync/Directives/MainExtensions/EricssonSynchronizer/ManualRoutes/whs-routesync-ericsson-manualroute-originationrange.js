(function (app) {

    'use strict';

    ManualRouteOriginationRange.$inject = ["UtilsService", 'VRUIUtilsService'];

    function ManualRouteOriginationRange(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new ManualRouteOriginationRangeCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_RouteSync/Directives/MainExtensions/EricssonSynchronizer/ManualRoutes/Templates/EricssonManualRouteOriginationRangeTemplate.html"
        };

        function ManualRouteOriginationRangeCtor($scope, ctrl, $attrs) {
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
                        if (payload.RouteOriginations != undefined) {
                            console.log(payload.RouteOriginations);
                            $scope.scopeModel.fromCode = payload.RouteOriginations.FromCode;
                            $scope.scopeModel.toCode = payload.RouteOriginations.ToCode;
                        }
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data = {
                        $type: "TOne.WhS.RouteSync.Ericsson.Entities.EricssonManualRouteOriginationRange, TOne.WhS.RouteSync.Ericsson",
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

    app.directive('whsRoutesyncEricssonManualrouteOriginationrange', ManualRouteOriginationRange);

})(app);