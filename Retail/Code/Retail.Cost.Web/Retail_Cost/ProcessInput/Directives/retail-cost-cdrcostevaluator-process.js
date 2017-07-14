"use strict";

app.directive("vrWhsDealDealevaluatorProcess", ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new DealEvaluatorProcessDirectiveConstructor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: "/Client/Modules/WhS_Deal/Directives/ProcessInput/Templates/DealEvaluatorProcessTemplate.html"
        };

        function DealEvaluatorProcessDirectiveConstructor($scope, ctrl) {
            this.initializeController = initializeController;

            function initializeController() {
                defineAPI();
            }
            function defineAPI() {

                var api = {};

                api.load = function (payload) {

                };

                api.getData = function () {

                    return {
                        InputArguments: {
                            $type: "TOne.WhS.Deal.BP.Arguments.DealEvaluatorProcessInput, TOne.WhS.Deal.BP.Arguments"
                        }
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);
