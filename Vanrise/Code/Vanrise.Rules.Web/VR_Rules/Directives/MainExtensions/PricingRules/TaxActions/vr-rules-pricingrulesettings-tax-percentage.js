'use strict';
app.directive('vrRulesPricingrulesettingsTaxPercentage', ['$compile',
function ($compile) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;
            var ctor = new percentageTaxCtor(ctrl, $scope, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/VR_Rules/Directives/MainExtensions/PricingRules/TaxActions/Templates/PricingRulePercentageTaxTemplate.html"

    };


    function percentageTaxCtor(ctrl, $scope, $attrs) {
        function initializeController() {
            defineAPI();
        }
        function defineAPI() {
            var api = {};

            api.getData = function () {
                var obj = {
                    $type: "Vanrise.Rules.Pricing.MainExtensions.Tax.PercentageTaxSettings,Vanrise.Rules.Pricing.MainExtensions",
                    ExtraPercentage: ctrl.extraPercentage
                };
                return obj;
            };

            api.load = function (payload) {
                if (payload != undefined) {
                    ctrl.extraPercentage = payload.ExtraPercentage;
                }
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;
}]);