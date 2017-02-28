'use strict';

app.directive('whsAccountbalanceFinancialaccountGrid', ['WhS_AccountBalance_FinancialAccountAPIService', 'VRNotificationService',
    function (WhS_AccountBalance_FinancialAccountAPIService, VRNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.showAccount = true;
                var financialAccountGrid = new FinancialAccountGrid($scope, ctrl, $attrs);
                financialAccountGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_AccountBalance/Elements/FinancialAccount/Directives/Templates/FinancialAccountGridTemplate.html'
        };

        function FinancialAccountGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.financialAccounts = [];
                $scope.scopeModel.menuActions = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return WhS_AccountBalance_FinancialAccountAPIService.GetFilteredFinancialAccounts(dataRetrievalInput).then(function (response) {
                        onResponseReady(response);
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
                };

                defineMenuActions();
            }

            function defineAPI() {
                var api = {};

                api.loadGrid = function (payload) {
                    return gridAPI.retrieveData(payload.query);
                };
                api.onFnancialAccountAdded = function (financialAccount) {
                    return gridAPI.itemAdded(financialAccount);
                };
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }


            function defineMenuActions() {
            }

        }
    }]);
