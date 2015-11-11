'use strict';
app.directive('vrWhsBePricingrulesettingsExtrachargeFixed', ['$compile',
function ($compile) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;
            var ctor = new fixedExtraChargeCtor(ctrl, $scope, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/MainExtensions/PricingRules/ExtraChargeActions/Templates/PricingRuleFixedExtraChargeTemplate.html"

    };


    function fixedExtraChargeCtor(ctrl, $scope, $attrs) {

        function initializeController() {

            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.getData = function () {
                var obj = {
                    $type: "TOne.WhS.BusinessEntity.MainExtensions.PricingRules.ExtraChargeActions.FixedExtraChargeSettings, TOne.WhS.BusinessEntity.MainExtensions",
                    FromRate: ctrl.fromRate,
                    ToRate: ctrl.toRate,
                    ExtraAmount: ctrl.extraAmount
                }
                return obj;
            }
            api.load = function (payload) {
                if (payload != undefined) {
                    ctrl.fromRate = payload.FromRate;
                    ctrl.toRate = payload.ToRate
                    ctrl.extraAmount = payload.ExtraAmount
                }
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;
}]);