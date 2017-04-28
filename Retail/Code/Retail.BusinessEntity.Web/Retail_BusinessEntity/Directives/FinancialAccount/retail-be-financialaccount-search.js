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
        templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/FinancialAccount/Templates/FinancialAccountSearchTemplate.html"

    };

    function BillingTransactionSearch($scope, ctrl, $attrs) {

        var gridAPI;
        var gridPromiseDeferred = UtilsService.createPromiseDeferred();

        var accountId;
        var accountBEDefinitionId;
        this.initializeController = initializeController;

        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.addFinancialAccount = function () {
                var onFinancialAccountAdded = function (obj) {
                    gridAPI.onFinancialAccountAdded(obj);
                };
                Retail_BE_FinancialAccountService.addFinancialAccount(onFinancialAccountAdded,accountBEDefinitionId, accountId);
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
                    accountId = payload.accountId;
                    accountBEDefinitionId = payload.accountBEDefinitionId;
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
                    accountId: accountId
                };
                gridAPI.loadGrid(payload);
            });
        }
       
    }

    return directiveDefinitionObject;

}]);
