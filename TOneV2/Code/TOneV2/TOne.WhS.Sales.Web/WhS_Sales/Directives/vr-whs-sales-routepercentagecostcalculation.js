"use strict";

app.directive("vrWhsSalesRoutepercentagecostcalculation", [function () {
    return {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var routePercentageCostCalculation = new RoutePercentageCostCalculation($scope, ctrl);
            routePercentageCostCalculation.initCtrl();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/WhS_Sales/Directives/Templates/RoutePercentageCostCalculationTemplate.html"
    };

    function RoutePercentageCostCalculation($scope, ctrl) {
        this.initCtrl = initCtrl;

        function initCtrl() {
            ctrl.title;

            if (ctrl.onReady && typeof (ctrl.onReady) == "function")
                ctrl.onReady(getAPI());

            function getAPI() {
                var api = {};
                api.load = function (payload) {
                    if (payload)
                        ctrl.title = payload.Title;
                };
                api.getData = function () {
                    return {
                        $type: "TOne.WhS.Sales.Entities.CostCalculation.Extensions.RoutePercentageCostCalculation, TOne.WhS.Sales.Entities",
                        Title: ctrl.title
                    };
                };
                return api;
            }
        }
    }
}]);
