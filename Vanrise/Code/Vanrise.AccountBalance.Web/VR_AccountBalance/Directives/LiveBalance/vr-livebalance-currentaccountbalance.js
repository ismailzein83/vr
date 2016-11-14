'use strict';

app.directive('vrLivebalanceCurrentaccountbalance', ['VR_AccountBalance_LiveBalanceAPIService',
    function (VR_AccountBalance_LiveBalanceAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var liveBalanceGrid = new LiveBalance($scope, ctrl, $attrs);
                liveBalanceGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_AccountBalance/Directives/LiveBalance/Templates/LiveBalanceTemplate.html'
        };

        function LiveBalance($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined && payload.accountId != undefined) {
                        return VR_AccountBalance_LiveBalanceAPIService.GetCurrentAccountBalance(payload.accountId, payload.accountTypeId).then(function (response) {
                            if (response) {
                                $scope.scopeModel.balance = response.CurrentBalance;
                                $scope.scopeModel.currency = response.CurrencyDescription;
                            }

                        });
                    }
                    return gridAPI.retrieveData(query);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);
