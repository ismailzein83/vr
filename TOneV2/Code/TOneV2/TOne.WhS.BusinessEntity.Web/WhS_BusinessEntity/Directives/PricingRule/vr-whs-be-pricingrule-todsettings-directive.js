'use strict';
app.directive('vrWhsBePricingruleTodsettings', ['UtilsService', '$compile','WhS_BE_PricingRuleAPIService',
function (UtilsService, $compil, WhS_BE_PricingRuleAPIService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;
            $scope.pricingRuleTODSettings = [];
            var bePricingRuleTODSettingObject = new bePricingRuleTODSetting(ctrl, $scope, $attrs);
            bePricingRuleTODSettingObject.initializeController();
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
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/PricingRule/Templates/PricingRuleTODSettings.html"

    };


    function bePricingRuleTODSetting(ctrl, $scope, $attrs) {

        function initializeController() {

            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.getData = function () {
            }

            api.setData = function (selectedIds) {


            }
            api.load = function () {
                return WhS_BE_PricingRuleAPIService.GetPricingRuleTODTemplates().then(function (response) {
                    angular.forEach(response, function (itm) {
                        $scope.pricingRuleTODSettings.push(itm);
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