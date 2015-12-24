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
            fixedRateCalculation.initCtrl();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/WhS_Sales/Directives/Extensions/RateCalculation/Templates/FixedRateCalculationTemplate.html"
    };

    function FixedRateCalculation(ctrl, $scope) {
        this.initCtrl = initCtrl;

        ctrl.fixedRate;

        function initCtrl() {
            getAPI();
        }

        function getAPI() {
            var api = {};

            api.load = function (payload) {
                if (payload) {
                    ctrl.fixedRate = payload.FixedRate;
                }
            };

            api.getData = function () {
                return {
                    $type: "TOne.WhS.Sales.MainExtensions.RateCalculation.FixedRateCalculationMethod, TOne.WhS.Sales.MainExtensions",
                    FixedRate: ctrl.fixedRate
                };
            };

            if (ctrl.onReady && typeof (ctrl.onReady) == "function")
                ctrl.onReady(api);
        }
    }
}]);
