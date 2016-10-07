"use strict";

app.directive("vrWhsSalesMarginratecalculation", [function () {

    return {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var mixedRateCalculation = new MarginRateCalculation(ctrl, $scope);
            mixedRateCalculation.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/WhS_Sales/Directives/Extensions/RateCalculation/Templates/MarginRateCalculationTemplate.html"
    };

    function MarginRateCalculation(ctrl, $scope) {
        this.initializeController = initializeController;

        ctrl.margin;

        function initializeController() {
            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                if (payload != undefined) {
                    ctrl.margin = payload.Margin;
                }
            };

            api.getData = function () {
                return {
                    $type: "TOne.WhS.Sales.MainExtensions.RateCalculation.MarginRateCalculationMethod, TOne.WhS.Sales.MainExtensions",
                    Margin: ctrl.margin
                };
            };

            api.isCostColumnRequired = function () {
                return true;
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);
