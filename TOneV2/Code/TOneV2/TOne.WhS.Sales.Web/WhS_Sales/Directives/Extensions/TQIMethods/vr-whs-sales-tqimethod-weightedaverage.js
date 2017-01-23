"use strict";

app.directive("vrWhsSalesTqimethodWeightedaverage", ['WhS_Sales_PeriodTypesEnum', 'UtilsService', function (WhS_Sales_PeriodTypesEnum, UtilsService) {

    return {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var weightedAvgTQIMethod = new WeightedAvgTQIMethod(ctrl, $scope);
            weightedAvgTQIMethod.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/WhS_Sales/Directives/Extensions/TQIMethods/Templates/WeightedAverageTQIMethodTemplate.html"
    };

    function WeightedAvgTQIMethod(ctrl, $scope) {
        this.initializeController = initializeController;
        var periodTypesReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        function initializeController() {

            $scope.onPeriodTypeSelectorReady = function (api) {
                $scope.periodTypes = UtilsService.getArrayEnum(WhS_Sales_PeriodTypesEnum)
                periodTypesReadyPromiseDeferred.resolve();
                defineAPI();
            };
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                if (payload != undefined) {
                    $scope.periodValue = payload.PeriodValue;
                    periodTypesReadyPromiseDeferred.promise.then(function () {
                        $scope.periodTypeSelectedValue = UtilsService.getItemByVal($scope.periodTypes, payload.PeriodType, "value");
                    });
                }
            };

            api.getData = function () {
                return {
                    $type: "TOne.WhS.Sales.MainExtensions.WeightedAverageTQIMethod, TOne.WhS.Sales.MainExtensions",
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
