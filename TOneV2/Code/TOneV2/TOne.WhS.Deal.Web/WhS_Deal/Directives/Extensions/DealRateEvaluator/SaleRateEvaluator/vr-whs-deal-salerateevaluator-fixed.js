"use strict";

app.directive("vrWhsDealSalerateevaluatorFixed", ['UtilsService', function (UtilsService) {

    return {
        restrict: "E",
        scope: {
            onReady: "=",
            isrequired: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var fixedSaleRateEvaluator = new FixedSaleRateEvaluator(ctrl, $scope);
            fixedSaleRateEvaluator.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/WhS_Deal/Directives/Extensions/DealRateEvaluator/SaleRateEvaluator/Templates/DealSaleRateEvaluatorFixed.html"
    };

    function FixedSaleRateEvaluator(ctrl, $scope) {

        this.initializeController = initializeController;

        function initializeController() {
            $scope.scopeModel = {};

            defineAPI();
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                if (payload != undefined) {
                    $scope.scopeModel.Rate = payload.evaluatedRate.Rate;
                }
            };

            api.getDescription = function () {
                return $scope.scopeModel.Rate;
            };

            api.getData = function () {
                return {
                    $type: "TOne.WhS.Deal.MainExtensions.FixedSaleRateEvaluator,TOne.WhS.Deal.MainExtensions",
                    Rate: $scope.scopeModel.Rate
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);