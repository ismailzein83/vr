(function (app) {

    'use strict';

    StyleFormatingCSSClass.$inject = ["UtilsService", 'VRUIUtilsService'];

    function StyleFormatingCSSClass(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var cssClassStyleFormating = new CSSClassStyleFormating($scope, ctrl, $attrs);
                cssClassStyleFormating.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Common/Directives/MainExtensions/StyleFormating/Templates/VRCommonStyleFormatingTemplate.html"

        };
        function CSSClassStyleFormating($scope, ctrl, $attrs) {

            this.initializeController = initializeController;
            var mainPayload;

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var styleFormatingSettings;

                    if (payload != undefined) {
                        styleFormatingSettings = payload.styleFormatingSettings;
                    }

                    if (styleFormatingSettings != undefined) {
                        $scope.scopeModel.connectionString = styleFormatingSettings.ClassName;
                    }
                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var data = {
                        $type: "Vanrise.Common.MainExtensions.StyleFormatingSettings.CSSClass, Vanrise.Common.MainExtensions",
                        ClassName: $scope.scopeModel.className
                    }
                    return data;
                }
            }
        }
    }

    app.directive('vrCommonStyleformatingCssclass', StyleFormatingCSSClass);

})(app);