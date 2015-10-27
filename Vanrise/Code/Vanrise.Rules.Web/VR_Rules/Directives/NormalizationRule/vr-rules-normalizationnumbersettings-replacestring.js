"use strict";

app.directive("vrRulesNormalizationnumbersettingsReplacestring", [function () {

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
        return "/Client/Modules/VR_Rules/Directives/NormalizationRule/Templates/ReplaceStringDirectiveTemplate.html";
    }

    function DirectiveConstructor($scope, ctrl) {
        this.initializeController = initializeController;

        $scope.stringToReplace = undefined;
        $scope.newString = undefined;

        function initializeController() {
            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.getData = function () {
                return {
                    $type: "Vanrise.Rules.Normalization.MainExtensions.ReplaceStringActionSettings, Vanrise.Rules.Normalization",
                    StringToReplace: $scope.stringToReplace,
                    NewString: $scope.newString
                };
            }

            api.setData = function (replaceStringActionSettings) {
                $scope.stringToReplace = replaceStringActionSettings.StringToReplace;
                $scope.newString = replaceStringActionSettings.NewString;
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}]);
