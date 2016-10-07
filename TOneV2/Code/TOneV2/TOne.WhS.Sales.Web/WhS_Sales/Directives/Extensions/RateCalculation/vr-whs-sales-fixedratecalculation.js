"use strict";

app.directive("vrWhsSalesFixedratecalculation", [function () {

    return {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var fixedRateCalculation = new FixedRateCalculation(ctrl, $scope);
            fixedRateCalculation.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/WhS_Sales/Directives/Extensions/RateCalculation/Templates/FixedRateCalculationTemplate.html"
    };

    function FixedRateCalculation(ctrl, $scope) {
        this.initializeController = initializeController;

        ctrl.fixedRate;

        function initializeController() {
            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                if (payload != undefined) {
                    ctrl.fixedRate = payload.FixedRate;
                }
            };

            api.getData = function () {
                return {
                    $type: "TOne.WhS.Sales.MainExtensions.RateCalculation.FixedRateCalculationMethod, TOne.WhS.Sales.MainExtensions",
                    FixedRate: ctrl.fixedRate
                };
            };

            api.isCostColumnRequired = function () {
                return false;
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);
