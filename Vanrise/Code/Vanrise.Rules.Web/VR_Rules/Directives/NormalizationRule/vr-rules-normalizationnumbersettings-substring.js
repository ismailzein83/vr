"use strict";

app.directive("vrRulesNormalizationnumbersettingsSubstring", [function () {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var ctor = new DirectiveConstructor($scope, ctrl);
            ctor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {

                }
            };
        },
        templateUrl: function (element, attrs) {
            return getDirectiveTemplateUrl();
        }
    };

    function getDirectiveTemplateUrl() {
        return "/Client/Modules/VR_Rules/Directives/NormalizationRule/Templates/SubstringDirectiveTemplate.html";
    }

    function DirectiveConstructor($scope, ctrl) {
        this.initializeController = initializeController;

        ctrl.startIndex = undefined;
        ctrl.length = undefined;

        function initializeController() {
            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.getData = function () {
                return {
                    $type: "Vanrise.Rules.Normalization.MainExtensions.SubstringActionSettings, Vanrise.Rules.Normalization",
                    StartIndex: ctrl.startIndex,
                    Length: ctrl.length
                };
            };
            api.load = function (payload) {
                if (payload != undefined) {
                    ctrl.startIndex = payload.StartIndex;
                    ctrl.length = payload.Length;
                }
            };
            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}]);
