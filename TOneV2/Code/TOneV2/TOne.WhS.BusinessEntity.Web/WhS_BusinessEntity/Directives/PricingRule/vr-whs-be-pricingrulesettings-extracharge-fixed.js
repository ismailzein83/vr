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
            var bePricingRuleFixedExtraChargeObject = new bePricingRuleFixedExtraCharge(ctrl, $scope, $attrs);
            bePricingRuleFixedExtraChargeObject.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/PricingRule/Templates/PricingRuleFixedExtraChargeTemplate.html"

    };


    function bePricingRuleFixedExtraCharge(ctrl, $scope, $attrs) {

        function initializeController() {

            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.getData = function () {
                var obj = {
                    $type: "TOne.WhS.BusinessEntity.Entities.PricingRules.RuleTypes.ExtraCharge.Actions.FixedExtraChargeSettings, TOne.WhS.BusinessEntity.Entities",
                    FromRate: $scope.fromRate,
                    ToRate: $scope.toRate,
                    ExtraAmount: $scope.extraAmount
                }
                return obj;
            }

            api.setData = function (selectedobj) {
              
                $scope.fromRate = selectedobj.FromRate;
                $scope.toRate = selectedobj.ToRate
                $scope.extraAmount = selectedobj.ExtraAmount
            }
            api.load = function () {
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;
}]);