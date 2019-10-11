(function (app) {

    'use strict';

    codeChargeInternationalDirective.$inject = ['UtilsService'];

    function codeChargeInternationalDirective(UtilsService) {

        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new codeChargeInternationalDirectiveCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Whs_RouteSync/Directives/CodeCharge/MainExtensions/Templates/CodeChargeInternationalTemplate.html'
        };

        function codeChargeInternationalDirectiveCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};

                defineAPI();
            }

            function defineAPI() {
                var api = {};


                api.load = function (payload) {

                    var internationalCodeChargeEvaluator;

                    if (payload != undefined) {
                        internationalCodeChargeEvaluator = payload.codeChargeEvaluator;
                    }

                    var promises = [];
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.RouteSync.MainExtensions.CodeCharge.InternationalCodeChargeEvaluator, TOne.WhS.RouteSync.MainExtensions"
                    };
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('whsRoutesyncCodechargeInternational', codeChargeInternationalDirective);
})(app);