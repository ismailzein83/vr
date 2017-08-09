"use strict";

app.directive("vrWhsSalesSuppliertargetmatchmethodsLcr3", ['UtilsService', function (UtilsService) {

    return {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var method = new LCR3Method(ctrl, $scope);
            method.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/WhS_Sales/Directives/Extensions/SupplierTargetMatchMethods/Templates/SupplierTargetMatchMethodsLCR3Template.html"
    };

    function LCR3Method(ctrl, $scope) {
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
                    $type: "TOne.WhS.Sales.MainExtensions.SupplierTargetMatchCalculation.LCR3TargetMatchCalculation, TOne.WhS.Sales.MainExtensions"
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);
