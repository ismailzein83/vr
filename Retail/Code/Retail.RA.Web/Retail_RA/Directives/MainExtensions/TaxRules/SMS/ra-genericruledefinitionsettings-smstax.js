(function (app) {

    'use strict';

    RuleDefinitionSettingsDirective.$inject = ['UtilsService', 'VRUIUtilsService'];

    function RuleDefinitionSettingsDirective(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope:
            {
                onReady: "=",
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
            templateUrl: "/Client/Modules/Retail_RA/Directives/Rules/SMS/Templates/EmptyDefinitionSettingsTemplate.html"
        };

        function RuleDefinitionSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {

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
                        $type: 'Retail.RA.Business.SMSTaxRuleDefinitionSettings, Retail.RA.Business'
                    };
                };

                return api;
            }
        }
    }

    app.directive('raGenericruledefinitionsettingsSmstax', RuleDefinitionSettingsDirective);

})(app);