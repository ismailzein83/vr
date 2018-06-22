"use strict";

app.directive("vrWhsDealSalerateevaluatorDiscount", ['UtilsService', function (UtilsService) {

    return {
        restrict: "E",
        scope: {
            onReady: "=",
            isrequired: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var discountSaleRateEvaluator = new DiscountSaleRateEvaluator(ctrl, $scope);
            discountSaleRateEvaluator.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/WhS_Deal/Directives/Extensions/DealRateEvaluator/SaleRateEvaluator/Templates/DealSaleRateEvaluatorDiccount.html"
    };

    function DiscountSaleRateEvaluator(ctrl, $scope) {

        this.initializeController = initializeController;

        function initializeController() {
            $scope.scopeModel = {};

            defineAPI();
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {

                if (payload.evaluatedRate != undefined) {
                    $scope.scopeModel.Discount = payload.evaluatedRate.Discount;
                }
            };

            api.getDescription = function () {
                return $scope.scopeModel.Discount + '%';
            };

            api.getData = function () {
                return {
                    $type: "TOne.WhS.Deal.MainExtensions.DiscountSaleRateEvaluator,TOne.WhS.Deal.MainExtensions",
                    Discount: $scope.scopeModel.Discount
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);