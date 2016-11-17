"use strict";

app.directive("vrCommonTextmanipulationsettingsReplacestring", [function () {

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
        return "/Client/Modules/Common/Directives/MainExtensions/TextManipulation/Templates/ReplaceStringDirectiveTemplate.html";
    }

    function DirectiveConstructor($scope, ctrl) {
        this.initializeController = initializeController;

        ctrl.stringToReplace = undefined;
        ctrl.newString = undefined;

        function initializeController() {
            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.getData = function () {
                return {
                    $type: "Vanrise.Common.MainExtensions.TextManipulationAction.ReplaceStringActionSettings, Vanrise.Common.MainExtensions",
                    StringToReplace: ctrl.stringToReplace,
                    NewString: ctrl.newString,
                    IgnoreCase: ctrl.ignoreCase
                };
            };

            api.load = function (payload) {
                if (payload != undefined) {
                    ctrl.stringToReplace = payload.StringToReplace;
                    ctrl.newString = payload.NewString;
                    ctrl.ignoreCase = payload.IgnoreCase;
                }
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}]);
