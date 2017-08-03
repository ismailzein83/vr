﻿"use strict";

app.directive("vrPstnBeReplacestring", [function () {

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
                    $type: "PSTN.BusinessEntity.MainExtensions.Normalization.AdjustNumber.ReplaceStringActionSettings, PSTN.BusinessEntity.MainExtensions",
                    StringToReplace: $scope.stringToReplace,
                    NewString: $scope.newString
                };
            };

            api.load = function (payload) {
                if (payload != undefined) {
                    $scope.stringToReplace = payload.StringToReplace;
                    $scope.newString = payload.NewString;
                }
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);



            api.validateData = function () {
                return $scope.stringToReplace != undefined && $scope.stringToReplace != null && $scope.newString != undefined && $scope.newString != null;
            };

           
        }
    }

    return directiveDefinitionObject;
}]);
