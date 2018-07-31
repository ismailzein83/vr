"use strict";

app.directive("vrWhsDealSupplierrateevaluatorDiscount", ['UtilsService', function (UtilsService) {

    return {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var discountSupplierRateEvaluator = new DiscountSupplierRateEvaluator(ctrl, $scope);
            discountSupplierRateEvaluator.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/WhS_Deal/Directives/Extensions/DealRateEvaluator/SupplierRateEvaluator/Templates/DealSupplierRateEvaluatorDiccount.html"
    };

    function DiscountSupplierRateEvaluator(ctrl, $scope) {

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
                    $type: "TOne.WhS.Deal.MainExtensions.DiscountSupplierRateEvaluator,TOne.WhS.Deal.MainExtensions",
                    Discount: $scope.scopeModel.Discount
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);