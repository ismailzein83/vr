"use strict";

app.directive("vrPstnBeAddprefix", [function () {

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
        return "/Client/Modules/PSTN_BusinessEntity/Directives/Templates/AddPrefixDirectiveTemplate.html";
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
                    $type: "PSTN.BusinessEntity.MainExtensions.Normalization.AdjustNumber.AddPrefixActionSettings, PSTN.BusinessEntity.MainExtensions",
                    ConfigId: ctrl.configid,
                    Prefix: $scope.numberPrefix
                };
            }

            api.setData = function (addPrefixActionSettings) {
                $scope.numberPrefix = addPrefixActionSettings.Prefix;
            }

            if (ctrl.onloaded != null)
                ctrl.onloaded(api);
        }
    }

    return directiveDefinitionObject;
}]);
