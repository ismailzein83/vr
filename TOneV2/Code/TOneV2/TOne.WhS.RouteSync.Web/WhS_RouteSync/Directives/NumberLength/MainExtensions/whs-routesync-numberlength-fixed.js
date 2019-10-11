(function (app) {

    'use strict';

    numberLengthFixedDirective.$inject = ['UtilsService'];

    function numberLengthFixedDirective(UtilsService) {

        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new numberLengthFixedDirectiveCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Whs_RouteSync/Directives/NumberLength/MainExtensions/Templates/NumberLengthFixedTemplate.html'
        };

        function numberLengthFixedDirectiveCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.isCodeLengthValid = function () {
                    if ($scope.scopeModel.minCodeLength != undefined && $scope.scopeModel.maxCodeLength != undefined && parseInt($scope.scopeModel.minCodeLength) > parseInt($scope.scopeModel.maxCodeLength))
                        return 'Maximum Code Length should be greater than Minimum Code Length.';
                    return null;
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};


                api.load = function (payload) {

                    var fixedNumberLengthEvaluator;

                    if (payload != undefined) {
                        fixedNumberLengthEvaluator = payload.numberLengthEvaluator;
                    }

                    if (fixedNumberLengthEvaluator != undefined) {
                        $scope.scopeModel.minCodeLength = fixedNumberLengthEvaluator.MinCodeLength;
                        $scope.scopeModel.maxCodeLength = fixedNumberLengthEvaluator.MaxCodeLength;
                    }

                    var promises = [];
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.RouteSync.MainExtensions.NumberLength.FixedNumberLengthEvaluator, TOne.WhS.RouteSync.MainExtensions",
                        MinCodeLength: $scope.scopeModel.minCodeLength,
                        MaxCodeLength: $scope.scopeModel.maxCodeLength
                    };
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('whsRoutesyncNumberlengthFixed', numberLengthFixedDirective);
})(app);