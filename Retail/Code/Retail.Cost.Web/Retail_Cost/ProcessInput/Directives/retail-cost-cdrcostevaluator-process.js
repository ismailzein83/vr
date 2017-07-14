"use strict";

app.directive("retailCostCdrcostevaluatorProcess", ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new CostEvaluatorProcessDirectiveConstructor($scope, ctrl);
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
            templateUrl: '/Client/Modules/Retail_Cost/ProcessInput/Directives/Templates/CostEvaluatorProcessTemplate.html'
        };

        function CostEvaluatorProcessDirectiveConstructor($scope, ctrl) {
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
                            $type: "Retail.Cost.BP.Arguments.CostEvaluatorProcessInput, Retail.Cost.BP.Arguments"
                        }
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);
