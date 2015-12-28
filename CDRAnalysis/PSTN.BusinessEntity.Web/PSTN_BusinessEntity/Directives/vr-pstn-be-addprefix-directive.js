"use strict";

app.directive("vrPstnBeAddprefix", [function () {

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

            api.validateData = function () {
                return true;
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}]);
