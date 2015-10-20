'use strict';
app.directive('vrWhsBePricingruleTariffsettings', ['UtilsService', '$compile', 'WhS_BE_PricingRuleAPIService',
function (UtilsService, $compil, WhS_BE_PricingRuleAPIService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;
            $scope.pricingRuleTariffSettings = [];
            var bePricingRuleTariffSettingObject = new bePricingRuleTariffSetting(ctrl, $scope, $attrs);
            bePricingRuleTariffSettingObject.initializeController();
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
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/PricingRule/Templates/PricingRuleTariffSettings.html"

    };


    function bePricingRuleTariffSetting(ctrl, $scope, $attrs) {
        var pricingRuleTariffTemplateDirectiveAPI;
        function initializeController() {
            $scope.onPricingRuleTariffTemplateDirectiveReady = function (api) {
                pricingRuleTariffTemplateDirectiveAPI = api;
            }
            defineAPI();
        }


        function defineAPI() {
            var api = {};

            api.getData = function () {
                var obj = pricingRuleTariffTemplateDirectiveAPI.getData();
                obj.ConfigId = $scope.selectedPricingRuleTariffSettings.TemplateConfigID;
                return obj;
            }

            api.setData = function (settings) {
                for (var j = 0; j < $scope.pricingRuleTariffSettings.length; j++)
                    if (settings.ConfigId == $scope.pricingRuleTariffSettings[j].TemplateConfigID)
                        $scope.selectedPricingRuleTariffSettings = $scope.pricingRuleTariffSettings[j];
                $scope.onPricingRuleTariffTemplateDirectiveReady = function (api) {
                    pricingRuleTariffTemplateDirectiveAPI = api;
                    pricingRuleTariffTemplateDirectiveAPI.setData(settings);
                }

            }
            api.load = function () {
                return WhS_BE_PricingRuleAPIService.GetPricingRuleTariffTemplates().then(function (response) {
                    angular.forEach(response, function (itm) {
                        $scope.pricingRuleTariffSettings.push(itm);
                    });
                })
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;
}]);