(function (app) {

    'use strict';

    numberLengthInternationalDirective.$inject = ['UtilsService'];

    function numberLengthInternationalDirective(UtilsService) {

        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new numberLengthInternationalDirectiveCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Whs_RouteSync/Directives/NumberLength/MainExtensions/Templates/NumberLengthInternationalTemplate.html'
        };

        function numberLengthInternationalDirectiveCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var interantionalNumberLengthEvaluator;

                    if (payload != undefined) {
                        interantionalNumberLengthEvaluator = payload.numberLengthEvaluator;
                    }

                    var promises = [];
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.RouteSync.MainExtensions.NumberLength.InternationalNumberLengthEvaluator, TOne.WhS.RouteSync.MainExtensions"
                    };
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('whsRoutesyncNumberlengthInternational', numberLengthInternationalDirective);

})(app);