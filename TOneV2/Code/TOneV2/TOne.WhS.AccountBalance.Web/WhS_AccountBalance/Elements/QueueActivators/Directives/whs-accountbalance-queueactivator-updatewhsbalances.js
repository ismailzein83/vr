(function (app) {

    'use strict';

    QueueActivatorUpdateWhSBalances.$inject = ['UtilsService', 'VRUIUtilsService'];

    function QueueActivatorUpdateWhSBalances(UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new QueueActivatorUpdateWhSBalancesCtor(ctrl, $scope);
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
                return '/Client/Modules/WhS_AccountBalance/Elements/QueueActivators/Directives/Templates/QueueActivatorUpdateWhSBalancesTemplate.html';
            }
        };

        function QueueActivatorUpdateWhSBalancesCtor(ctrl, $scope) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                };

                api.getData = function () {

                    return {
                        $type: 'TOne.WhS.AccountBalance.MainExtensions.QueueActivators.UpdateWhSBalancesQueueActivator, TOne.WhS.AccountBalance.MainExtensions'
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }

    app.directive('whsAccountbalanceQueueactivatorUpdatewhsbalances', QueueActivatorUpdateWhSBalances);

})(app);