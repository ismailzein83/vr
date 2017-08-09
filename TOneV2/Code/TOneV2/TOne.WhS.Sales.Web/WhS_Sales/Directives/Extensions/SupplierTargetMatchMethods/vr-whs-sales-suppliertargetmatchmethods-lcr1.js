"use strict";

app.directive("vrWhsSalesSuppliertargetmatchmethodsLcr1", ['UtilsService', function (UtilsService) {

    return {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var lcr1Method = new LCR1Method(ctrl, $scope);
            lcr1Method.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/WhS_Sales/Directives/Extensions/SupplierTargetMatchMethods/Templates/SupplierTargetMatchMethodsLCR1Template.html"
    };

    function LCR1Method(ctrl, $scope) {
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
                    $type: "TOne.WhS.Sales.MainExtensions.SupplierTargetMatchCalculation.LCR1TargetMatchCalculation, TOne.WhS.Sales.MainExtensions"
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);
