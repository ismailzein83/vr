"use strict";

app.directive("vrWhsSalesPercentagecostcalculation", [function () {

    return {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var percentageCostCalculation = new PercentageCostCalculation(ctrl, $scope);
            percentageCostCalculation.initCtrl();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/WhS_Sales/Directives/Templates/PercentageCostCalculationTemplate.html"
    };

    function PercentageCostCalculation(ctrl, $scope) {
        this.initCtrl = initCtrl;

        var percentageDirectiveAPI;

        function initCtrl() {
            ctrl.title;

            $scope.optionPercentageSettingsGroupTemplates = [];

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
                        $type: "TOne.WhS.Sales.Entities.CostCalculation.Extensions.PercentageCostCalculation, TOne.WhS.Sales.Entities",
                        Title: ctrl.title,
                        FixedOptionPercentage: percentageDirectiveAPI.getData()
                    };
                };

                if (ctrl.onReady && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(api);
            }
        }
    }
}]);
