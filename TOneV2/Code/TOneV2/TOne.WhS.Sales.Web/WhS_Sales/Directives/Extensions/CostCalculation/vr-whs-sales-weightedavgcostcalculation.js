"use strict";

app.directive("vrWhsSalesWeightedavgcostcalculation", [function () {

    return {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var avgCostCalculation = new AvgCostCalculation(ctrl, $scope);
            avgCostCalculation.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/WhS_Sales/Directives/Extensions/CostCalculation/Templates/WeightedAvgCostCalculation.html"
    };

    function AvgCostCalculation(ctrl, $scope) {
        this.initializeController = initializeController;

        function initializeController() {
            ctrl.title;
            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                if (payload != undefined) {
                    $scope.title = payload.Title;
                    $scope.days = payload.Days
                }
            };

            api.getData = function () {
                return {
                    $type: "TOne.WhS.Sales.MainExtensions.CostCalculation.WeightedAvgCostCalculation, TOne.WhS.Sales.MainExtensions",
                    Title: $scope.title,
                    Days: $scope.days
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);
