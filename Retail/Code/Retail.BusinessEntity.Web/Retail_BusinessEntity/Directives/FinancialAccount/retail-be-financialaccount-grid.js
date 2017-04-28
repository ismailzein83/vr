'use strict';

app.directive('retailBeFinancialaccountGrid', ['Retail_BE_FinancialAccountService','Retail_BE_FinancialAccountAPIService', 'VRNotificationService', function (Retail_BE_FinancialAccountService,Retail_BE_FinancialAccountAPIService, VRNotificationService) {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var financialAccountGrid = new FinancialAccountGrid($scope, ctrl, $attrs);
            financialAccountGrid.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/FinancialAccount/Templates/FinancialAccountGridTemplate.html'
    };

    function FinancialAccountGrid($scope, ctrl, $attrs) {
        this.initializeController = initializeController;

        var gridAPI;
        var accountBEDefinitionId;
        var accountId;
        function initializeController() {
            $scope.scopeModel = {};
            $scope.scopeModel.financialAccounts = [];
            $scope.scopeModel.menuActions = [];

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                defineAPI();
            };

            $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return Retail_BE_FinancialAccountAPIService.GetFilteredFinancialAccounts(dataRetrievalInput).then(function (response) {
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
                accountBEDefinitionId = payload.accountBEDefinitionId;
                accountId = payload.accountId;
                return gridAPI.retrieveData(payload.query);
            };

            api.onFinancialAccountAdded = function (addedFinancialAccount) {
                gridAPI.itemAdded(addedFinancialAccount);
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function defineMenuActions() {
            $scope.scopeModel.menuActions.push({
                name: 'Edit',
                clicked: editFinancialAccount,
            });
        }

        function editFinancialAccount(financialAccount) {
            var onFinancialAccountUpdated = function (updatedFinancialAccount) {
                gridAPI.itemUpdated(updatedFinancialAccount);
            };
            Retail_BE_FinancialAccountService.editFinancialAccount(onFinancialAccountUpdated,accountBEDefinitionId,accountId, financialAccount.SequenceNumber);
        }

    }
}]);
