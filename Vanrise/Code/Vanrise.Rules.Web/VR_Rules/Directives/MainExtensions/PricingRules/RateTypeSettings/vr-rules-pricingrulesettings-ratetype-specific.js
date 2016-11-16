'use strict';
app.directive('vrRulesPricingrulesettingsRatetypeSpecific', ['$compile',
function ($compile) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;
            var ctor = new specificRateTypeCtor(ctrl, $scope, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/VR_Rules/Directives/MainExtensions/PricingRules/RateTypeSettings/Templates/PricingRuleSpecificRateTypeTemplate.html"

    };


    function specificRateTypeCtor(ctrl, $scope, $attrs) {

        function initializeController() {

            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.getData = function () {
                var obj = {
                    $type: "Vanrise.Rules.Pricing.MainExtensions.RateType.SpecificDayRateTypeSettings, Vanrise.Rules.Pricing.MainExtensions",
                    Date: ctrl.date,
                };
                return obj;
            };
            api.load = function (payload) {
                if (payload != undefined) {
                    ctrl.date = payload.Date;
                }
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;
}]);