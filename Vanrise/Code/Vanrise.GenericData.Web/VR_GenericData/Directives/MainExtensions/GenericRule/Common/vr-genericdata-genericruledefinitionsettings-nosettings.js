(function (app) {

    'use strict';

    NosettingsRuleDefinitionSettingsDirective.$inject = [];

    function NosettingsRuleDefinitionSettingsDirective() {
        return {
            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var obj = new NosettingsRuleDefinitionSettings($scope, ctrl, $attrs);
                obj.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            template: ""
        };

        function NosettingsRuleDefinitionSettings($scope, ctrl, $attrs) {
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

    app.directive('vrGenericdataGenericruledefinitionsettingsNosettings', NosettingsRuleDefinitionSettingsDirective);

})(app);