﻿'use strict';
app.directive('vrWhsBePricingrulePercentageextracharge', ['UtilsService', '$compile',
function (UtilsService, $compil) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;
            var bePricingRulePercentageExtraChargeObject = new bePricingRulePercentageExtraCharge(ctrl, $scope, $attrs);
            bePricingRulePercentageExtraChargeObject.initializeController();
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
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/PricingRule/Templates/PricingRulePercentageExtraChargeTemplate.html"

    };


    function bePricingRulePercentageExtraCharge(ctrl, $scope, $attrs) {

        function initializeController() {

            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.getData = function () {
                var obj = {
                    $type: "TOne.WhS.BusinessEntity.Entities.PricingRules.RuleTypes.ExtraCharge.Actions.PercentageExtraChargeSettings, TOne.WhS.BusinessEntity.Entities.PricingRules.RuleTypes.ExtraCharge.Actions",
                    FromRate: $scope.fromRate,
                    ToRate: $scope.toRate,
                    ExtraPercentage: $scope.extraPercentage
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