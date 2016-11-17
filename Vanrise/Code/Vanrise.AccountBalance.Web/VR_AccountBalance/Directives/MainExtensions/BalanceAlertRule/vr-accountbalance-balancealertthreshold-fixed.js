'use strict';
app.directive('vrAccountbalanceBalancealertthresholdFixed', ['UtilsService',
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
                return '/Client/Modules/VR_AccountBalance/Directives/MainExtensions/BalanceAlertRule/Templates/FixedBalanceAlertThresholdTemplate.html';
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
                        $type: "Vanrise.AccountBalance.MainExtensions.BalanceAlertThresholds.FixedBalanceAlertThreshold, Vanrise.AccountBalance.MainExtensions",
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