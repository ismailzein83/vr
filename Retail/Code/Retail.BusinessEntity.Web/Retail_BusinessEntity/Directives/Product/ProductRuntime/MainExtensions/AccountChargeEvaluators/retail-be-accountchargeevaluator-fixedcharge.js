(function (app) {

    'use strict';

    FixedChargeDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

    function FixedChargeDirective(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new FixedChargeCtor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/Product/ProductRuntime/MainExtensions/AccountChargeEvaluators/Templates/FixedChargeAccountChargeEvaluatorTemplate.html'
        };

        function FixedChargeCtor($scope, ctrl) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var chargeEvaluator;

                    if (payload != undefined) {
                        chargeEvaluator = payload.chargeEvaluator
                    }

                    $scope.scopeModel.charge = chargeEvaluator != undefined ? chargeEvaluator.Charge : undefined;
                };

                api.getData = function () {

                    var obj = {
                        $type: "Retail.BusinessEntity.MainExtensions.AccountChargeEvaluators.FixedChargeAccountChargeEvaluator, Retail.BusinessEntity.MainExtensions",
                        Charge: $scope.scopeModel.charge
                    };
                    return obj;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('retailBeAccountchargeevaluatorFixedcharge', FixedChargeDirective);

})(app);