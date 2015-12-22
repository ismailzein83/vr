"use strict";

app.directive("vrRuntimeTaskactionGroup", ['UtilsService', 'VRUIUtilsService',
    function (UtilsService,VRUIUtilsService) {

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
        return "/Client/Modules/Runtime/Directives/TaskAction/Templates/TaskActionGroup.html";
    }

    function DirectiveConstructor($scope, ctrl) {
        this.initializeController = initializeController;

       
        function initializeController() {
            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.getData = function () {
                
            };


            api.load = function (payload) {
                
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}]);
