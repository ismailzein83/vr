(function (app) {

    'use strict';

    TextValuePropertyEvaluator.$inject = ['UtilsService', 'VRUIUtilsService'];

    function TextValuePropertyEvaluator(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                label: '@',
                customvalidate: '=',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var textValuePropertyEvaluatorSecurity = new TextValuePropertyEvaluatorSecurity($scope, ctrl, $attrs);
                textValuePropertyEvaluatorSecurity.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: '/Client/Modules/Common/Directives/MainExtensions/VRObjectTypes/Templates/TextValuePropertyEvaluatorTemplate.html'
        };

        function TextValuePropertyEvaluatorSecurity($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            function initializeController() {

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                };

                api.getData = function () {

                    var data = {
                        $type: "Vanrise.Common.MainExtensions.VRObjectTypes.TextValuePropertyEvaluator, Vanrise.Common.MainExtensions"
                    };
                    return data;
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrCommonTextvaluepropertyevaluator', TextValuePropertyEvaluator);

})(app);
