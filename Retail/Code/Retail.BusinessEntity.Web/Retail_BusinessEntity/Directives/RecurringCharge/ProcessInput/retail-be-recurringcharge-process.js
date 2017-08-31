"use strict";

app.directive("retailBeRecurringchargeProcess", ['UtilsService',
    function (UtilsService) {
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

            this.initializeController = initializeController;
            var lastDayOfMonth;

            function initializeController() {
                defineAPI();

                $scope.validateEffectiveDate = function () {
                    if (lastDayOfMonth != undefined && $scope.effectiveDate != undefined && $scope.effectiveDate > lastDayOfMonth) {
                        return "Effective Date should not exceed end of the month";
                    }
                    return null;
                };
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
                    $scope.effectiveDate = UtilsService.getDateFromDateTime(new Date());
                    lastDayOfMonth = new Date($scope.effectiveDate.getFullYear(), $scope.effectiveDate.getMonth() + 1, 0);
                    return UtilsService.waitMultiplePromises(promises);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);
