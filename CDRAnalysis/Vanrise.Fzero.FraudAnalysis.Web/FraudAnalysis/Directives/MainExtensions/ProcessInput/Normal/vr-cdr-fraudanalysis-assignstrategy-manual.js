"use strict";

app.directive("vrCdrFraudanalysisAssignstrategyManual", [ function () {
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
                var createProcessInputObjects = [];

                createProcessInputObjects.push({
                    InputArguments: {
                        $type: "Vanrise.Fzero.FraudAnalysis.BP.Arguments.AssignStrategyCasesProcessInput, Vanrise.Fzero.FraudAnalysis.BP.Arguments"
                    }
                });

                return createProcessInputObjects;
            };
           

            api.load = function (payload) {

            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}]);
