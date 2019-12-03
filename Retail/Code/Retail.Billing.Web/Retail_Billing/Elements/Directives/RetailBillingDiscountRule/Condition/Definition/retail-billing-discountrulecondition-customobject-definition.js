(function (app) {

    'use strict';

    DiscountRuleConditionCustomObjectDefinition.$inject = ['UtilsService', 'VRUIUtilsService'];

    function DiscountRuleConditionCustomObjectDefinition(UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new DiscountRuleConditionCustomObjectDefinitionDirectiveCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_Billing/Elements/Directives/RetailBillingDiscountRule/Condition/Definition/Templates/DiscountRuleConditionDefinitionTemplate.html"
        };

        function DiscountRuleConditionCustomObjectDefinitionDirectiveCtor(ctrl, $scope, attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    return UtilsService.waitPromiseNode({ promises: [] });
                };

                api.getData = function () {
                    return {
                        $type: "Retail.Billing.MainExtensions.DiscountRuleCondition.DiscountRuleConditionCustomObjectTypeSettings, Retail.Billing.MainExtensions",
                    };
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }

        return directiveDefinitionObject;
    }

    app.directive('retailBillingDiscountruleconditionCustomobjectDefinition', DiscountRuleConditionCustomObjectDefinition);
})(app);