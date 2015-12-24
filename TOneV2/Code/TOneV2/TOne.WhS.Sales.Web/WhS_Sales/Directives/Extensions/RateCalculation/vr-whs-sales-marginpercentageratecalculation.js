"use strict";

app.directive("vrWhsSalesMarginpercentageratecalculation", [function () {

    return {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var marginPercentageRateCalculation = new MarginPercentageRateCalculation(ctrl, $scope);
            marginPercentageRateCalculation.initCtrl();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/WhS_Sales/Directives/Extensions/RateCalculation/Templates/MarginPercentageRateCalculationTemplate.html"
    };

    function MarginPercentageRateCalculation(ctrl, $scope) {
        this.initCtrl = initCtrl;

        ctrl.marginPercentage;

        function initCtrl() {
            getAPI();
        }

        function getAPI() {
            var api = {};

            api.load = function (payload) {
                if (payload) {
                    ctrl.marginPercentage = payload.MarginPercentage;
                }
            };

            api.getData = function () {
                return {
                    $type: "TOne.WhS.Sales.MainExtensions.RateCalculation.MarginPercentageRateCalculationMethod, TOne.WhS.Sales.MainExtensions",
                    MarginPercentage: ctrl.marginPercentage
                };
            };

            if (ctrl.onReady && typeof (ctrl.onReady) == "function")
                ctrl.onReady(api);
        }
    }
}]);
