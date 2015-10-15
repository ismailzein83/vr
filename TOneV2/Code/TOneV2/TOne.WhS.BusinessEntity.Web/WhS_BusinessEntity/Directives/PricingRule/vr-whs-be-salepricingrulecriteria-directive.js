'use strict';
app.directive('vrWhsBeSalepricingrulecriteria', ['WhS_BE_SalePricingRuleAPIService', 'UtilsService', '$compile',
function (WhS_BE_SalePricingRuleAPIService, UtilsService, $compile) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            onselectionchanged: '=',
        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;

            $scope.selectedCarrierProfiles;
            if ($attrs.ismultipleselection != undefined)
                $scope.selectedCarrierProfiles = [];

            $scope.carrierProfiles = [];
            var beSalePricingRuleObject = new beSalePricingRule(ctrl, $scope, $attrs);
            beSalePricingRuleObject.initializeController();
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
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/PricingRule/Templates/SalePricingRuleCriteriaTemplate.html"

    };


    function beSalePricingRule(ctrl, $scope, $attrs) {

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
           
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;
}]);