(function (app) {

    'use strict';

    FinancialTransactionsViewDirective.$inject = ['UtilsService', 'VRNotificationService','Retail_BE_AccountBalanceTypeAPIService'];

    function FinancialTransactionsViewDirective(UtilsService, VRNotificationService, Retail_BE_AccountBalanceTypeAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new FinancialTransactionsViewCtor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/AccountViews/MainExtensions/Templates/FinancialTransactionsViewTemplate.html'
        };

        function FinancialTransactionsViewCtor($scope, ctrl) {
            this.initializeController = initializeController;

            var accountBEDefinitionId;
            var parentAccountId;

            var accountTypeId;

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    if (payload != undefined) {
                        accountBEDefinitionId = payload.accountBEDefinitionId;
                        parentAccountId = payload.parentAccountId;
                    }
                     
                    function getAccountTypeId()
                    {
                        return Retail_BE_AccountBalanceTypeAPIService.GetAccountBalanceTypeId(accountBEDefinitionId).then(function (response) {
                            accountTypeId = response;
                        });
                    }

                    var promises = [];

                    var promiseDeferred = UtilsService.createPromiseDeferred();
                    promises.push(promiseDeferred.promise);
                    getAccountTypeId().then(function () {
                        var promise =  gridAPI.loadDirective(buildGridPayload());
                        if (promise != undefined)
                        {
                            promise.then(function () {
                                promiseDeferred.resolve();
                            }).catch(function (error) {
                                promiseDeferred.reject(error);
                            });
                        }else
                        {
                            promiseDeferred.resolve();
                        }
                          
                    });
                    return UtilsService.waitMultiplePromises(promises);
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function buildGridPayload() {

                var billingTransactionGridPayload = {
                    accountBEDefinitionId: accountBEDefinitionId,
                    AccountsIds: [parentAccountId],
                    AccountTypeId: accountTypeId
                };
                return billingTransactionGridPayload;
            }
        }
    }

    app.directive('retailBeFinancialtransactionsView', FinancialTransactionsViewDirective);

})(app);