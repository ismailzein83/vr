(function (app) {

    'use strict';

    RouteOptionMarginRateCalculation.$inject = ["UtilsService", 'VRUIUtilsService'];

    function RouteOptionMarginRateCalculation(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new RouteOptionMarginRateCalculationCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "routeOptionMarginRateCalculationCtrl",
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_Sales/Directives/Extensions/PricingTemplate/Templates/RouteOptionMarginRateCalculationTemplate.html"

        };
        function RouteOptionMarginRateCalculationCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.optionNumbers = [{ value: 1, description: 'Option 1' }, { value: 2, description: 'Option 2' }, { value: 3, description: 'Option 3' }];

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var marginRateCalculation;

                    if (payload != undefined) {
                        marginRateCalculation = payload.marginRateCalculation;
                    }

                    if (marginRateCalculation != undefined) {
                        $scope.scopeModel.selectedOptionNumber = UtilsService.getItemByVal($scope.scopeModel.optionNumbers, marginRateCalculation.RPRouteOptionNumber, 'value');
                    }
                };

                api.getData = function getData() {
                    var data = {
                        $type: "TOne.WhS.Sales.MainExtensions.PricingTemplateRate.RouteOptionMarginRateCalculation, TOne.WhS.Sales.MainExtensions",
                        RPRouteOptionNumber: $scope.scopeModel.selectedOptionNumber ? $scope.scopeModel.selectedOptionNumber.value : undefined
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrWhsSalesMarginratecalculationRouteoption', RouteOptionMarginRateCalculation);

})(app);