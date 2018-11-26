"use strict";

app.directive('vrRulesPricingrulesettingsTariffSixtyone', ['$compile', 'VR_Rules_FirstPeriodRateTypeEnum',
    function ($compile, VR_Rules_FirstPeriodRateTypeEnum) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new sixtyOneTariffCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Rules/Directives/MainExtensions/PricingRules/TariffSettings/Templates/PricingRuleSixtyoneTariffTemplate.html"
        };

        function sixtyOneTariffCtor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var regularDirectiveReadyAPI;

            function initializeController() {
                $scope.onRegularTariffDirectiveReady = function (api) {
                    regularDirectiveReadyAPI = api;
                    defineAPI();
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    if (payload == undefined) {
                        payload = {
                            FirstPeriodRateType: VR_Rules_FirstPeriodRateTypeEnum.EffectiveRate.value,
                            PricingUnit: 60,
                            FractionUnit: 1,
                            FirstPeriod: 60
                        };
                    }

                    payload.isDisabled = true;

                    regularDirectiveReadyAPI.load(payload);
                };

                api.getData = function () {
                    var obj = regularDirectiveReadyAPI.getData();
                    obj.$type = "Vanrise.Rules.Pricing.MainExtensions.Tariff.SixtyOneTariffSettings, Vanrise.Rules.Pricing.MainExtensions";
                    return obj;
                };

                if (ctrl.onReady != null && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);