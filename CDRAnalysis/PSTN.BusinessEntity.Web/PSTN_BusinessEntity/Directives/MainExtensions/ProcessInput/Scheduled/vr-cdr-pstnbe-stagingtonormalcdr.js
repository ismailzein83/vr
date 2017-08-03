"use strict";

app.directive("vrCdrPstnbeStagingtocdr", [function () {
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
        template: function (element, attrs) {
            return "";
        }
    };
    function DirectiveConstructor($scope, ctrl) {

       
        this.initializeController = initializeController;

        

        function initializeController() {
            defineAPI();
        }

        function defineAPI() {

           
            var api = {};
            api.getData = function () {
                return {
                    $type: "Vanrise.Fzero.CDRImport.BP.Arguments.StagingtoCDRProcess, Vanrise.Fzero.CDRImport.BP.Arguments"
                };
            };
            api.getExpressionsData = function () {
                return { "ScheduleTime": "ScheduleTime" };

            };

            api.load = function (payload) {

            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}]);
