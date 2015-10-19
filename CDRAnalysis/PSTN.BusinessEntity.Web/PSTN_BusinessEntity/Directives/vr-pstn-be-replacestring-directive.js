"use strict";

app.directive("vrPstnBeReplacestring", [function () {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            configid: "=",
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
        return "/Client/Modules/PSTN_BusinessEntity/Directives/Templates/ReplaceStringDirectiveTemplate.html";
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
                    $type: "PSTN.BusinessEntity.Entities.Normalization.RuleTypes.NormalizeNumber.Actions.ReplaceStringActionSettings, PSTN.BusinessEntity.Entities",
                    configid: ctrl.configid,
                    StringToReplace: $scope.stringToReplace,
                    NewString: $scope.newString
                };
            }

            api.setData = function (replaceStringActionSettings) {
                $scope.stringToReplace = replaceStringActionSettings.StringToReplace;
                $scope.newString = replaceStringActionSettings.NewString;
            }

            if (ctrl.onloaded != null)
                ctrl.onloaded(api);
        }
    }

    return directiveDefinitionObject;
}]);
