(function (app) {

    'use strict';

    codeChargeFixedDirective.$inject = ['UtilsService'];

    function codeChargeFixedDirective(UtilsService) {

        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new codeChargeFixedDirectiveCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Whs_RouteSync/Directives/CodeCharge/MainExtensions/Templates/CodeChargeFixedTemplate.html'
        };

        function codeChargeFixedDirectiveCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var fixedCodeChargeEvaluator;

                    if (payload != undefined) {
                        fixedCodeChargeEvaluator = payload.codeChargeEvaluator;
                    }

                    if (fixedCodeChargeEvaluator != undefined) {
                        $scope.scopeModel.codeCharge = fixedCodeChargeEvaluator.CodeCharge;
                    }

                    var promises = [];
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.RouteSync.MainExtensions.CodeCharge.FixedCodeChargeEvaluator, TOne.WhS.RouteSync.MainExtensions",
                        CodeCharge: $scope.scopeModel.codeCharge
                    };
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('whsRoutesyncCodechargeFixed', codeChargeFixedDirective);
})(app);