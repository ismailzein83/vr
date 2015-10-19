"use strict";

app.directive("vrPstnBeSetareaprefix", [function () {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onloaded: "="
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
        return "/Client/Modules/PSTN_BusinessEntity/Directives/Templates/SetAreaPrefixDirectiveTemplate.html";
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
                    $type: "PSTN.BusinessEntity.MainExtensions.Normalization.SetArea.SetAreaPrefixSettings, PSTN.BusinessEntity.MainExtensions",
                    PrefixLength: $scope.prefixLength
                };
            }

            api.setData = function (setAreaPrefixSettings) {
                $scope.prefixLength = setAreaPrefixSettings.PrefixLength;
            }

            if (ctrl.onloaded != null)
                ctrl.onloaded(api);
        }
    }

    return directiveDefinitionObject;
}]);
