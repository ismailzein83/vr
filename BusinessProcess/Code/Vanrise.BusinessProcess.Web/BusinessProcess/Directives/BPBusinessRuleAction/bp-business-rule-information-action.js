"use strict";

app.directive("businessprocessBpBusinessRuleInformationAction", [function () {
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
            };
        },
        templateUrl: "/Client/Modules/BusinessProcess/Directives/BPBusinessRuleAction/Templates/EmptyTemplate.html"
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
                    action: {
                        $type: "Vanrise.BusinessProcess.InformationAction, Vanrise.BusinessProcess",
                    }
                };
            };

            api.load = function (payload) {

            };

            if (ctrl.onReady != null) {
                ctrl.onReady(api);
            }
        }
    }

    return directiveDefinitionObject;
}]);





