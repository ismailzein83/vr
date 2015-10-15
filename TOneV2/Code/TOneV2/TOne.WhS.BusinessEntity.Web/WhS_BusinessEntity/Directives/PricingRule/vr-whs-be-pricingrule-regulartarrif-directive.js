'use strict';
app.directive('vrWhsBePricingruleRegulartariff', ['UtilsService', '$compile',
function (UtilsService, $compil) {

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