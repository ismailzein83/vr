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
                    var styleFormating;

                    if (payload != undefined) {
                        styleFormating = payload.switchIntegration;
                    }

                    if (styleFormating != undefined) {
                        $scope.scopeModel.connectionString = styleFormating.ClassName;
                    }
                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var data = {
                        $type: "Retail.BusinessEntity.MainExtensions.SwitchIntegrations.Radius,Retail.BusinessEntity.MainExtensions",
                        ConnectionString: $scope.scopeModel.className
                    }
                    return data;
                }
            }
        }
    }

    app.directive('vrCommonStyleFormatingCSSClass', StyleFormatingCSSClass);

})(app);