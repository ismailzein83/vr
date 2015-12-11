"use strict";

app.directive("vrQmClitesterProfilesettings", [function () {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var directiveConstructor = new DirectiveConstructor($scope, ctrl);
            directiveConstructor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {

                }
            }
        },
        templateUrl: function (element, attrs) {
            return getDirectiveTemplateUrl();
        }
    };

    function getDirectiveTemplateUrl() {
        return "/Client/Modules/QM_CLITester/Directives/Profile/Templates/AddPrefixDirectiveTemplate.html";
    }

    function DirectiveConstructor($scope, ctrl) {
        this.initializeController = initializeController;

        $scope.numberPrefix = undefined;
        
        function initializeController() {
            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.getData = function () {
                return {
                    $type: "QM.CLITester.iTestIntegration.ProfileExtensionSettings, QM.CLITester.iTestIntegration",
                    Prefix: $scope.numberPrefix
                };
            };

            api.load = function (payload) {
                if (payload != undefined) {
                    $scope.numberPrefix = payload.Prefix;
                }
            }


            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}]);
