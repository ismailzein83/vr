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
            var ctor = new regulartariffCtor(ctrl, $scope, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/PricingRule/Templates/PricingRuleRegularTariffTemplate.html"

    };


    function regulartariffCtor(ctrl, $scope, $attrs) {

        function initializeController() {

            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.getData = function () {
                var obj = {
                    $type: "TOne.WhS.BusinessEntity.Entities.PricingRules.RuleTypes.Tariff.Settings.RegularTariffSettings, TOne.WhS.BusinessEntity.Entities",
                    CallFee: ctrl.callFee,
                    FirstPeriod: ctrl.firstPeriod,
                    FirstPeriodRate: ctrl.firstPeriodRate,
                    FractionUnit: ctrl.fractionUnit
                }
                return obj;
            }
            api.load = function (payload) {
                if (payload != undefined) {
                    ctrl.callFee = payload.CallFee;
                    ctrl.firstPeriod = payload.FirstPeriod,
                    ctrl.firstPeriodRate = payload.FirstPeriodRate,
                    ctrl.fractionUnit = payload.FractionUnit
                }
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;
}]);