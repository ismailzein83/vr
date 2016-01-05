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
            avgCostCalculation.initCtrl();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/WhS_Sales/Directives/Extensions/CostCalculation/Templates/AvgCostCalculation.html"
    };

    function AvgCostCalculation(ctrl, $scope) {
        this.initCtrl = initCtrl;

        function initCtrl() {
            ctrl.title;

            getAPI();

            function getAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload) {
                        ctrl.title = payload.Title;
                    }
                };

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.Sales.Entities.CostCalculation.Extensions.AvgCostCalculation, TOne.WhS.Sales.Entities",
                        Title: ctrl.title
                    };
                };

                if (ctrl.onReady && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(api);
            }
        }
    }
}]);
