"use strict";

app.directive("vrWhsSalesSuppliertargetmatchmethodsWeightedaverage", ['UtilsService', function (UtilsService) {

    return {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var method = new WeightedAverageMethod(ctrl, $scope);
            method.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/WhS_Sales/Directives/Extensions/SupplierTargetMatchMethods/Templates/SupplierTargetMatchMethodsLCR1Template.html"
    };

    function WeightedAverageMethod(ctrl, $scope) {
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
                return {
                    $type: "TOne.WhS.Sales.MainExtensions.SupplierTargetMatchCalculation.WeightedAverageTargetMatchCalculation, TOne.WhS.Sales.MainExtensions"
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);
