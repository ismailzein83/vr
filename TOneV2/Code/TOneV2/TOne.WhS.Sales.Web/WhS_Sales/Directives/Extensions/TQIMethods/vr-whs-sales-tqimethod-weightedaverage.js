"use strict";

app.directive("vrWhsSalesTqimethodWeightedaverage", ['UtilsService', function (UtilsService) {

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
        var context;

        function initializeController() {

                defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                if (payload != undefined) {
                    context = payload.context;
                }
            };

            api.getData = function () {
                var duration = context.getDuration();
                return {
                    $type: "TOne.WhS.Sales.MainExtensions.WeightedAverageTQIMethod, TOne.WhS.Sales.MainExtensions",
                    PeriodValue: duration.periodValue,
                    PeriodType: duration.periodType
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);
