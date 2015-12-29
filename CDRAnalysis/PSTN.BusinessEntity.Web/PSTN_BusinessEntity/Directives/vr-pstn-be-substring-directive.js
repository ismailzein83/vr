"use strict";

app.directive("vrPstnBeSubstring", [function () {

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
        return "/Client/Modules/PSTN_BusinessEntity/Directives/Templates/SubstringDirectiveTemplate.html";
    }

    function DirectiveConstructor($scope, ctrl) {
        this.initializeController = initializeController;

        $scope.startIndex = undefined;
        $scope.length = undefined;

        function initializeController() {
            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.getData = function () {
                return {
                    $type: "PSTN.BusinessEntity.MainExtensions.Normalization.AdjustNumber.SubstringActionSettings, PSTN.BusinessEntity.MainExtensions",
                    StartIndex: $scope.startIndex,
                    Length: $scope.length
                };
            };


            api.load = function (payload) {
                if (payload != undefined) {
                    $scope.startIndex = payload.StartIndex;
                    $scope.length = payload.Length;
                }
            }

            api.validateData = function () {
                return $scope.startIndex != undefined && $scope.startIndex != null && $scope.length != undefined && $scope.length != null;
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}]);
