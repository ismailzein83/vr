"use strict";

app.directive("vrWhsDealSalerateevaluatorFixed", ['UtilsService', 'UISettingsService', function (UtilsService, UISettingsService) {

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
        var context;

        this.initializeController = initializeController;

        function initializeController() {
         
            $scope.scopeModel = {};
            $scope.longPrecision = UISettingsService.getLongPrecision();
            defineAPI();
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                var promises = [];
                if (payload != undefined) {
                    context = payload.context;
                    var currency = context.getVolumeCommitmentCurrency();
                    if(currency != undefined)
                        $scope.scopeModel.currency = currency.Symbol;
                    if (payload.evaluatedRate != undefined) {
                        $scope.scopeModel.Rate = payload.evaluatedRate.Rate;
                    }
                }
                return UtilsService.waitMultiplePromises(promises);
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