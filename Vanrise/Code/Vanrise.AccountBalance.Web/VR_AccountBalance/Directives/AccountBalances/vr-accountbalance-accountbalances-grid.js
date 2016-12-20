'use strict';

app.directive('vrAccountbalanceAccountbalancesGrid', ['VR_AccountBalance_LiveBalanceAPIService', 'VRNotificationService',
    function (VR_AccountBalance_BillingTransactionAPIService, VRNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.showAccount = true;
                var grid = new AccountBalancesTransactionGrid($scope, ctrl, $attrs);
                grid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_AccountBalance/Directives/AccountBalances/Templates/AccountBalancesGridTemplate.html'
        };

        function AccountBalancesTransactionGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.accountBalances = [];
                $scope.scopeModel.menuActions = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VR_AccountBalance_BillingTransactionAPIService.GetFilteredBillingTransactions(dataRetrievalInput).then(function (response) {
                        onResponseReady(response);
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
                };

                defineMenuActions();
            }

            function defineAPI() {
                var api = {};
                api.loadGrid = function (query) {
                    return gridAPI.retrieveData(query);
                };
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }


            function defineMenuActions() {
            }

        }
    }]);
