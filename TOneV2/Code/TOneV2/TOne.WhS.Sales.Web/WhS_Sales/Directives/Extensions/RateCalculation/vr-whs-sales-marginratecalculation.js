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
            mixedRateCalculation.initCtrl();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/WhS_Sales/Directives/Extensions/RateCalculation/Templates/MarginRateCalculationTemplate.html"
    };

    function MarginRateCalculation(ctrl, $scope) {
        this.initCtrl = initCtrl;

        ctrl.margin;

        function initCtrl() {
            getAPI();
        }

        function getAPI() {
            var api = {};

            api.load = function (payload) {
                if (payload) {
                    ctrl.margin = payload.Margin;
                }
            };

            api.getData = function () {
                return {
                    $type: "TOne.WhS.Sales.MainExtensions.RateCalculation.MarginRateCalculationMethod, TOne.WhS.Sales.MainExtensions",
                    Margin: ctrl.margin
                };
            };

            if (ctrl.onReady && typeof (ctrl.onReady) == "function")
                ctrl.onReady(api);
        }
    }
}]);
