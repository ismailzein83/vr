"use strict";

app.directive("vrWhsSalesAvgcostcalculation", [function () {

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
        templateUrl: "/Client/Modules/WhS_Sales/Directives/Extensions/CostCalculation/Templates/AvgCostCalculation.html"
    };

    function AvgCostCalculation(ctrl, $scope)
    {
        this.initializeController = initializeController;

        function initializeController()
        {
            ctrl.title;
            defineAPI();
        }

        function defineAPI()
        {
            var api = {};

            api.load = function (payload) {
                if (payload != undefined) {
                    ctrl.title = payload.Title;
                }
            };

            api.getData = function () {
                return {
                    $type: "TOne.WhS.Sales.MainExtensions.CostCalculation.AvgCostCalculation, TOne.WhS.Sales.MainExtensions",
                    Title: ctrl.title
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);
