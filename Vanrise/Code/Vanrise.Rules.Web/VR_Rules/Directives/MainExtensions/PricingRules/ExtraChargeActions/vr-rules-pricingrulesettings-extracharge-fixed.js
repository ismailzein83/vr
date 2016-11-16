'use strict';
app.directive('vrRulesPricingrulesettingsExtrachargeFixed', ['$compile',
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
        templateUrl: "/Client/Modules/VR_Rules/Directives/MainExtensions/PricingRules/ExtraChargeActions/Templates/PricingRuleFixedExtraChargeTemplate.html"

    };


    function fixedExtraChargeCtor(ctrl, $scope, $attrs) {
        ctrl.validate = function () {
            if (ctrl.fromRate != undefined && ctrl.toRate != undefined && parseFloat(ctrl.fromRate) > parseFloat(ctrl.toRate))
                return 'From Rate should be less than To Rate';
            return undefined;
        };
        function initializeController() {

            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.getData = function () {
                var obj = {
                    $type: "Vanrise.Rules.Pricing.MainExtensions.ExtraCharge.FixedExtraChargeSettings, Vanrise.Rules.Pricing.MainExtensions",
                    FromRate: ctrl.fromRate,
                    ToRate: ctrl.toRate,
                    ExtraAmount: ctrl.extraAmount
                };
                return obj;
            };
            api.load = function (payload) {
                if (payload != undefined) {
                    ctrl.fromRate = payload.FromRate;
                    ctrl.toRate = payload.ToRate;
                    ctrl.extraAmount = payload.ExtraAmount;
                }
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;
}]);