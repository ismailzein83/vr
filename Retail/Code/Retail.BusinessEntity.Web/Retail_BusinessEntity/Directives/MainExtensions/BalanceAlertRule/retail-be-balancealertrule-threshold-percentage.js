'use strict';
app.directive('retailBeBalancealertruleThresholdPercentage', ['UtilsService',
    function (UtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new PercentageThresholdCtor(ctrl, $scope);
                ctor.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: function (element, attrs) {
                return '/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/BalanceAlertRule/Templates/PercentageBalanceAlertThresholdTemplate.html';
            }

        };

        function PercentageThresholdCtor(ctrl, $scope) {

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        $scope.scopeModel.percentage = payload.Percentage;
                    }
                };

                api.getData = function () {
                    return {
                        $type: "Retail.BusinessEntity.MainExtensions.AccountBalanceAlertRule.PercentageBalanceAlertThreshold, Retail.BusinessEntity.MainExtensions",
                        Percentage: $scope.scopeModel.percentage,
                        ThresholdDescription: $scope.scopeModel.percentage + " %"
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }
]);