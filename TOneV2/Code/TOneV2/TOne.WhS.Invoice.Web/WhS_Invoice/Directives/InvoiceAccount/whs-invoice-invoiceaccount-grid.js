'use strict';

app.directive('whsInvoiceInvoiceaccountGrid', ['WhS_Invoice_InvoiceAccountAPIService', 'VRNotificationService', 'VRUIUtilsService', 'WhS_Invoice_InvoiceAccountService', function (WhS_Invoice_InvoiceAccountAPIService, VRNotificationService, VRUIUtilsService, WhS_Invoice_InvoiceAccountService) {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            ctrl.showAccount = true;
            var invoiceAccountGrid = new InvoiceAccountGrid($scope, ctrl, $attrs);
            invoiceAccountGrid.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/WhS_Invoice/Directives/InvoiceAccount/Templates/InvoiceAccountGridTemplate.html'
    };

    function InvoiceAccountGrid($scope, ctrl, $attrs) {

        this.initializeController = initializeController;

        var context;

        var gridAPI;

        function initializeController() {
            $scope.scopeModel = {};
            $scope.scopeModel.invoiceAccounts = [];
            $scope.scopeModel.menuActions = [];

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                defineAPI();
            };

            $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_Invoice_InvoiceAccountAPIService.GetFilteredInvoiceAccounts(dataRetrievalInput).then(function (response) {
                    if (response != undefined && response.Data != null) {
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

            api.onInvoiceAccountAdded = function (invoiceAccount) {
                return gridAPI.itemAdded(invoiceAccount);
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function defineMenuActions() {

            var menuActions = [{
                name: "Edit",
                clicked: editInvoiceAccount,
            }];

            $scope.scopeModel.gridMenuActions = function (dataItem) {
                if (dataItem.IsActive) {
                    return menuActions;
                }
                return null;
            };
        }
        function editInvoiceAccount(dataItem) {
            var onInvoiceAccountUpdated = function (invoiceAccount) {
                if (context != undefined && context.checkAllowAddInvoiceAccount != undefined)
                    context.checkAllowAddInvoiceAccount();
                gridAPI.itemUpdated(invoiceAccount);
            };
            WhS_Invoice_InvoiceAccountService.editInvoiceAccount(onInvoiceAccountUpdated, dataItem.Entity.InvoiceAccountId);
        }
    }
}]);