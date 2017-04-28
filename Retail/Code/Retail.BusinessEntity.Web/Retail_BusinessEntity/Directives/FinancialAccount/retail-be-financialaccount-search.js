"use strict";

app.directive("retailBeFinancialaccountSearch", ['VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'VRValidationService', 'Retail_BE_FinancialAccountService',
function (VRNotificationService, UtilsService, VRUIUtilsService, VRValidationService, Retail_BE_FinancialAccountService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var billingTransactionSearch = new BillingTransactionSearch($scope, ctrl, $attrs);
            billingTransactionSearch.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/VR_AccountBalance/Directives/BillingTransaction/Templates/BillingTransactionSearch.html"

    };

    function BillingTransactionSearch($scope, ctrl, $attrs) {

        var gridAPI;
        var gridPromiseDeferred = UtilsService.createPromiseDeferred();

        var accountsIds;
     
        this.initializeController = initializeController;

        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.addFinancialAccount = function () {
                var onFinancialAccountAdded = function (obj) {
                    gridAPI.onFinancialAccountAdded(obj);
                };
                Retail_BE_FinancialAccountService.addFinancialAccount(onFinancialAccountAdded, accountsIds[0]);
            };

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridPromiseDeferred.resolve();
            };

            defineAPI();
        }

        function defineAPI() {
            var api = {};
            api.loadDirective = function (payload) {
                $scope.scopeModel.isLoading = true;

                if (payload != undefined) {
                    accountsIds = payload.AccountsIds;
                }

                return loadFinancialAccountsGrid().finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
            };

            if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                ctrl.onReady(api);
        }

        function getFilterObject() {
            var filter = {
                TransactionTypeIds: transactionTypeSelectorAPI.getSelectedIds(),
                AccountTypeId: accountTypeId,
                AccountsIds: accountsIds,
                FromTime: $scope.scopeModel.fromTime,
                ToTime: $scope.scopeModel.toTime,
            };
            return filter;
        }
        function loadFinancialAccountsGrid() {
            return gridPromiseDeferred.promise.then(function () {
                var payload = {
                    query: getFilterObject(),
                };
                gridAPI.loadGrid(payload);
            });
        }
       
    }

    return directiveDefinitionObject;

}]);
