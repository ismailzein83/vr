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
            marginPercentageRateCalculation.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/WhS_Sales/Directives/Extensions/RateCalculation/Templates/MarginPercentageRateCalculationTemplate.html"
    };

    function MarginPercentageRateCalculation(ctrl, $scope) {
        this.initializeController = initializeController;

        ctrl.marginPercentage;

        function initializeController() {
            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                if (payload != undefined) {
                    ctrl.marginPercentage = payload.MarginPercentage;
                }
            };

            api.getData = function () {
                return {
                    $type: "TOne.WhS.Sales.MainExtensions.RateCalculation.MarginPercentageRateCalculationMethod, TOne.WhS.Sales.MainExtensions",
                    MarginPercentage: ctrl.marginPercentage
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
