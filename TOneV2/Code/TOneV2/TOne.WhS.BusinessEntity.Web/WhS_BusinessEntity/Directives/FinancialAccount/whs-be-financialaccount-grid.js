'use strict';

app.directive('whsBeFinancialaccountGrid', ['WhS_BE_FinancialAccountAPIService', 'VRNotificationService', 'WhS_BE_FinancialAccountService', 'VRUIUtilsService', function (WhS_BE_FinancialAccountAPIService, VRNotificationService, WhS_BE_FinancialAccountService, VRUIUtilsService) {
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
        templateUrl: '/Client/Modules/WhS_BusinessEntity/Directives/FinancialAccount/Templates/FinancialAccountGridTemplate.html'
    };

    function FinancialAccountGrid($scope, ctrl, $attrs) {

        this.initializeController = initializeController;

        var context;

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
                return WhS_BE_FinancialAccountAPIService.GetFilteredFinancialAccounts(dataRetrievalInput).then(function (response) {
                    if (response != undefined && response.Data != null) {
                        for (var i = 0; i < response.Data.length; i++) {
                            var financialAccount = response.Data[i];
                            WhS_BE_FinancialAccountService.defineFinancialAccountDrillDownTabs(financialAccount, gridAPI);
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

            api.loadGrid = function (payload) {

                var query;

                if (payload != undefined) {
                    context = payload.context;
                    query = payload.query;
                }

                return gridAPI.retrieveData(query);
            };

            api.onFinancialAccountAdded = function (financialAccount) {
                WhS_BE_FinancialAccountService.defineFinancialAccountDrillDownTabs(financialAccount, gridAPI);
                return gridAPI.itemAdded(financialAccount);
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
       
        function defineMenuActions() {

            var menuActions = [{
                name: "Edit",
                clicked: editFinancialAccount,
                haspermission: hasUpdateFinancialAccountPermission
            }];

            $scope.scopeModel.gridMenuActions = function (dataItem) {
                if (dataItem.IsActive) {
                    return menuActions;
                }
                return null;
            };
        }

        function editFinancialAccount(dataItem) {
            var onFinancialAccountUpdated = function (financialAccount) {
                if (context != undefined && context.checkAllowAddFinancialAccount != undefined)
                    context.checkAllowAddFinancialAccount();
                WhS_BE_FinancialAccountService.defineFinancialAccountDrillDownTabs(financialAccount, gridAPI);
                gridAPI.itemUpdated(financialAccount);
            };
            WhS_BE_FinancialAccountService.editFinancialAccount(onFinancialAccountUpdated, dataItem.Entity.FinancialAccountId);
        }

        function hasUpdateFinancialAccountPermission() {
            return WhS_BE_FinancialAccountAPIService.HasUpdateFinancialAccountPermission();
        }
    }
}]);