"use strict";

app.directive("retailBeRecurringchargeProcess", ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
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
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/RecurringCharge/ProcessInput/Templates/RecurringChargProcessTemplate.html'
        };

        function DirectiveConstructor($scope, ctrl) {

            var gridAPI;
            this.initializeController = initializeController;


            function initializeController() {
                defineAPI();
            }

            function defineAPI() {

                var api = {};
                api.getData = function () {
                    return {
                        InputArguments: {
                            $type: "Retail.BusinessEntity.BP.Arguments.AccountRecurringChargeEvaluatorProcessInput, Retail.BusinessEntity.BP.Arguments",
                            EffectiveDate: $scope.effectiveDate
                        }
                    };
                };

                api.load = function (payload) {
                    var promises = [];
                    $scope.effectiveDate = new Date();
                    return UtilsService.waitMultiplePromises(promises);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);
