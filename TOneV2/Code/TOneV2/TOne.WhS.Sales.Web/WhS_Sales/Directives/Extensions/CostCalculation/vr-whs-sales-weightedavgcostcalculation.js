"use strict";

app.directive("vrWhsSalesWeightedavgcostcalculation", ['WhS_Sales_PeriodTypesEnum', 'UtilsService', function (WhS_Sales_PeriodTypesEnum, UtilsService) {

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
        var periodTypesReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        function initializeController() {
            ctrl.title;
            
            $scope.onPeriodTypeSelectorReady = function (api) {
                $scope.periodTypes = UtilsService.getArrayEnum(WhS_Sales_PeriodTypesEnum)
                periodTypesReadyPromiseDeferred.resolve();
                defineAPI();
                }
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                if (payload != undefined) {
                    $scope.title = payload.Title;
                    $scope.periodValue = payload.PeriodValue;
                    periodTypesReadyPromiseDeferred.promise.then(function () {
                        $scope.periodTypeSelectedValue = UtilsService.getItemByVal($scope.periodTypes, payload.PeriodType, "value");
                    });
                }
            };

            api.getData = function () {
                return {
                    $type: "TOne.WhS.Sales.MainExtensions.CostCalculation.WeightedAvgCostCalculation, TOne.WhS.Sales.MainExtensions",
                    Title: $scope.title,
                    PeriodValue: $scope.periodValue,
                    PeriodType: $scope.periodTypeSelectedValue.value
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);
