"use strict";

app.directive("vrPstnBeNormalizenumber", [function () {

    var directiveDefinitionObj = {
        restrict: "E",
        scope: {
            onloaded: "=",
            configid: "="
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

    function DirectiveConstructor($scope, ctrl) {
        this.initializeController = initializeController;

        $scope.message = "Mohamad";

        function initializeController() {
            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.getData = function () {
                return {
                    $type: "PSTN.BusinessEntity.Entities.Normalization.NormalizationRuleSettings, PSTN.BusinessEntity.Entities",
                    ConfigId: ctrl.configid,
                    Message: $scope.message
                };
            }

            api.setData = function (normalizationRuleSettings) {
                $scope.message = normalizationRuleSettings.Message;
            }

            if (ctrl.onloaded != null)
                ctrl.onloaded(api);
        }
    }

    function getDirectiveTemplateUrl() {
        return "/Client/Modules/PSTN_BusinessEntity/Directives/Templates/NormalizeNumberDirectiveTemplate.html";
    }

    return directiveDefinitionObj;
    
}]);
