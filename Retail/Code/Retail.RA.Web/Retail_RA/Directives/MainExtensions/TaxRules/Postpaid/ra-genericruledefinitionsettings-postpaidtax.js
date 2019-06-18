(function (app) {
    'use strict';
    RuleDefinitionSettingsDirective.$inject = ['UtilsService', 'VRUIUtilsService'];
    function RuleDefinitionSettingsDirective(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope:
            {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new RuleDefinitionSettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_RA/Directives/MainExtensions/TaxRules/Postpaid/Templates/PostpaidTaxDefinitionSettingsTemplate.html"
        };

        function RuleDefinitionSettings($scope, ctrl, $attrs) {
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
                        $type: 'Retail.RA.Business.PostpaidTaxRuleDefinitionSettings, Retail.RA.Business'
                    };
                };

                return api;
            }
        }
    }

    app.directive('raGenericruledefinitionsettingsPostpaidtax', RuleDefinitionSettingsDirective);

})(app);