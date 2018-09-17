'use strict';

app.directive('vrAccountbalanceAccountstatementGrid', ['VR_AccountBalance_AccountStatementAPIService', 'VRNotificationService','DataGridRetrieveDataEventType',
    function (VR_AccountBalance_AccountStatementAPIService, VRNotificationService, DataGridRetrieveDataEventType) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var accountStatementGrid = new AccountStatementGrid($scope, ctrl, $attrs);
                accountStatementGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_AccountBalance/Directives/AccountStatement/Templates/AccountStatementGridTemplate.html'
        };

        function AccountStatementGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;
            var accountTypeId;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.acountStatements = [];
                $scope.scopeModel.menuActions = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady,gridContext) {
                    return VR_AccountBalance_AccountStatementAPIService.GetFilteredAccountStatments(dataRetrievalInput).then(function (response) {
                        if (response)
                        {
                            if (gridContext != undefined && gridContext.eventType == DataGridRetrieveDataEventType.ExternalTrigger)
                            {
                                ctrl.currency = response.Currency;
                                ctrl.currentBalance = response.CurrentBalance;
                                ctrl.totalDebit = response.TotalDebit;
                                ctrl.totalCredit = response.TotalCredit;
                                ctrl.balanceFlagDescription = response.BalanceFlagDescription;

                            }
                            
                        }
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
                    if (accountTypeId != undefined && accountTypeId != query.AccountTypeId)
                        gridAPI.clearUpdatedItems();
                    accountTypeId = query.AccountTypeId;
                    return gridAPI.retrieveData(query);
                };
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }


            function defineMenuActions() {
            }

        }
    }]);
