(function (app) {

    'use strict';

    NormalizationRuleDefinitionSettingsDirective.$inject = [];

    function NormalizationRuleDefinitionSettingsDirective() {
        return {
            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var obj = new NormalizationRuleDefinitionSettings($scope, ctrl, $attrs);
                obj.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            template: ""
        };

        function NormalizationRuleDefinitionSettings($scope, ctrl, $attrs) {
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
                    return {};
                };

                return api;
            }
        }
    }

    app.directive('vrGenericdataGenericruledefinitionsettingsNormalization', NormalizationRuleDefinitionSettingsDirective);

})(app);