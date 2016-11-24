'use strict';
app.directive('retailBeBalancealertruleThresholdFixed', ['UtilsService',
    function (UtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new FixedThresholdCtor(ctrl, $scope);
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
                return '/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/BalanceAlertRule/Templates/FixedBalanceAlertThresholdTemplate.html';
            }

        };

        function FixedThresholdCtor(ctrl, $scope) {

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        $scope.scopeModel.threshold = payload.Threshold;
                    }
                };

                api.getData = function () {
                    return {
                        $type: "Retail.BusinessEntity.MainExtensions.AccountBalanceAlertRule.FixedBalanceAlertThreshold, Retail.BusinessEntity.MainExtensions",
                        Threshold: $scope.scopeModel.threshold
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