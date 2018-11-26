﻿"use strict";

app.directive('vrRulesPricingrulesettingsTariffRegular', ['$compile', 'UtilsService', 'VR_Rules_FirstPeriodRateTypeEnum',
    function ($compile, UtilsService, VR_Rules_FirstPeriodRateTypeEnum) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
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
            templateUrl: "/Client/Modules/VR_Rules/Directives/MainExtensions/PricingRules/TariffSettings/Templates/PricingRuleRegularTariffTemplate.html"
        };

        function regulartariffCtor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.datasource = UtilsService.getArrayEnum(VR_Rules_FirstPeriodRateTypeEnum);

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        ctrl.selectedFirstPeriodType = UtilsService.getItemByVal($scope.datasource, payload.FirstPeriodRateType, 'value');
                        ctrl.callFee = payload.CallFee;
                        ctrl.firstPeriod = payload.FirstPeriod;
                        ctrl.firstPeriodRate = payload.FirstPeriodRate;
                        ctrl.fractionUnit = payload.FractionUnit;
                        ctrl.pricingUnit = payload.PricingUnit;
                        ctrl.isDisabled = payload.isDisabled;
                    }
                };

                api.getData = function () {
                    var obj = {
                        $type: "Vanrise.Rules.Pricing.MainExtensions.Tariff.RegularTariffSettings, Vanrise.Rules.Pricing.MainExtensions",
                        CallFee: ctrl.callFee,
                        FirstPeriod: ctrl.firstPeriod,
                        FirstPeriodRate: ctrl.selectedFirstPeriodType.showFirstPeriodRate ? ctrl.firstPeriodRate : undefined,
                        FractionUnit: ctrl.fractionUnit,
                        PricingUnit: ctrl.pricingUnit,
                        FirstPeriodRateType: ctrl.selectedFirstPeriodType.value
                    };
                    return obj;
                };

                if (ctrl.onReady != null && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);