'use strict';
app.directive('vrWhsBePricingrulesettingsExtrachargePercentage', ['$compile',
function ( $compile) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;
            var ctor = new percentageExtraChargeCtor(ctrl, $scope, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/PricingRule/Settings/Templates/PricingRulePercentageExtraChargeTemplate.html"

    };


    function percentageExtraChargeCtor(ctrl, $scope, $attrs) {

        function initializeController() {

            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.getData = function () {
                var obj = {
                    $type: "TOne.WhS.BusinessEntity.Entities.PricingRules.RuleTypes.ExtraCharge.Actions.PercentageExtraChargeSettings, TOne.WhS.BusinessEntity.Entities",
                    FromRate: ctrl.fromRate,
                    ToRate: ctrl.toRate,
                    ExtraPercentage: ctrl.extraPercentage
                }
                return obj;
            }

            api.load = function (payload) {
                if (payload != undefined) {
                    ctrl.fromRate = payload.FromRate;
                    ctrl.toRate = payload.ToRate
                    ctrl.extraPercentage = payload.ExtraPercentage
                }
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;
}]);