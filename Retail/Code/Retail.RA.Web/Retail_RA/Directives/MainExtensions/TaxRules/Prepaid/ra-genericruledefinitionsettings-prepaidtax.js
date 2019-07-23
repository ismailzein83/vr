(function (app) {
    'use strict';
    PrepaidTaxRuleDefinitionSettingsDirective.$inject = ['UtilsService', 'VRUIUtilsService'];
    function PrepaidTaxRuleDefinitionSettingsDirective(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope:
            {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new PrepaidTaxRuleDefinitionSettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_RA/Directives/MainExtensions/TaxRules/Prepaid/Templates/PrepaidTaxDefinitionSettingsTemplate.html"
        };

        function PrepaidTaxRuleDefinitionSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {

                $scope.scopeModel = {};

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(getDirectiveAPI());
                }
            }

            function getDirectiveAPI() {
                var api = {};

                api.load = function (payload) {

                };

                api.getData = function () {
                    return {
                        $type: 'Retail.RA.Business.PrepaidTaxRuleDefinitionSettings, Retail.RA.Business'
                    };
                };

                return api;
            }
        }
    }

    app.directive('raGenericruledefinitionsettingsPrepaidtax', PrepaidTaxRuleDefinitionSettingsDirective);

})(app);