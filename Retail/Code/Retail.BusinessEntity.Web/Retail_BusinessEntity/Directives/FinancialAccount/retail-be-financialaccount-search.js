"use strict";

app.directive("retailBeFinancialaccountSearch", ['VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'VRValidationService', 'Retail_BE_FinancialAccountService','Retail_BE_FinancialAccountAPIService',
function (VRNotificationService, UtilsService, VRUIUtilsService, VRValidationService, Retail_BE_FinancialAccountService, Retail_BE_FinancialAccountAPIService) {

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
        templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/FinancialAccount/Templates/FinancialAccountSearchTemplate.html"

    };

    function BillingTransactionSearch($scope, ctrl, $attrs) {

        var gridAPI;
        var gridPromiseDeferred = UtilsService.createPromiseDeferred();

        var accountId;
        var accountBEDefinitionId;
        this.initializeController = initializeController;
        var context;

        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.addFinancialAccount = function () {
                var onFinancialAccountAdded = function (obj) {
                    if (context != undefined) {
                        context.checkAllowAddFinancialAccount();
                    }
                    gridAPI.onFinancialAccountAdded(obj);
                };
                Retail_BE_FinancialAccountService.addFinancialAccount(onFinancialAccountAdded,accountBEDefinitionId, accountId);
            };

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridPromiseDeferred.resolve();
            };
            defineContext();
            defineAPI();
        }

        function defineAPI() {
            var api = {};
            api.loadDirective = function (payload) {
                $scope.scopeModel.isLoading = true;

                if (payload != undefined) {
                    accountId = payload.accountId;
                    accountBEDefinitionId = payload.accountBEDefinitionId;
                }
                var promises = [];
                promises.push(checkAllowAddFinancialAccount());
                promises.push(loadFinancialAccountsGrid());
                return UtilsService.waitMultiplePromises(promises).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
               
            };

            if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                ctrl.onReady(api);
        }

        function getFilterObject() {
            var filter = {
                AccountBEDefinitionId: accountBEDefinitionId,
                AccountId: accountId,
            };
            return filter;
        }
        function loadFinancialAccountsGrid() {
            return gridPromiseDeferred.promise.then(function () {
                var payload = {
                    query: getFilterObject(),
                    accountBEDefinitionId: accountBEDefinitionId,
                    accountId: accountId,
                    context: context
                };
                gridAPI.loadGrid(payload);
            });
        }
        function checkAllowAddFinancialAccount() {
            return Retail_BE_FinancialAccountAPIService.CheckAllowAddFinancialAccounts(accountBEDefinitionId, accountId).then(function (response) {
                $scope.scopeModel.showAddButton = response;
            });
        }
        function defineContext() {
            context = {
                checkAllowAddFinancialAccount: function () {
                    $scope.scopeModel.isLoading = true;
                    return checkAllowAddFinancialAccount().finally(function () {
                        $scope.scopeModel.isLoading = false;
                    });
                }
            };
        }
    }

    return directiveDefinitionObject;

}]);
