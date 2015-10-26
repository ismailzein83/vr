'use strict';
app.directive('vrWhsBePricingrulesettingsTariffRegular', ['$compile',
function ($compile) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;
            var bePricingRuleRegulartariffObject = new bePricingRuleRegulartariff(ctrl, $scope, $attrs);
            bePricingRuleRegulartariffObject.initializeController();
            $scope.onselectionchanged = function () {

                if (ctrl.onselectionchanged != undefined) {
                    var onvaluechangedMethod = $scope.$parent.$eval(ctrl.onselectionchanged);
                    if (onvaluechangedMethod != undefined && onvaluechangedMethod != null && typeof (onvaluechangedMethod) == 'function') {
                        onvaluechangedMethod();
                    }
                }

            }
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/PricingRule/Templates/PricingRuleRegularTariffTemplate.html"

    };


    function bePricingRuleRegulartariff(ctrl, $scope, $attrs) {

        function initializeController() {

            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.getData = function () {
                var obj = {
                    $type: "TOne.WhS.BusinessEntity.Entities.PricingRules.RuleTypes.Tariff.Settings.RegularTariffSettings, TOne.WhS.BusinessEntity.Entities",
                    CallFee: $scope.callFee,
                    FirstPeriod: $scope.firstPeriod,
                    FirstPeriodRate: $scope.firstPeriodRate,
                    FractionUnit: $scope.fractionUnit
                }
                return obj;
            }

            api.setData = function (obj) {
                $scope.callFee=obj.CallFee;
                $scope.firstPeriod=obj.FirstPeriod,
                $scope.firstPeriodRate=obj.FirstPeriodRate ,
                $scope.fractionUnit=obj.FractionUnit
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