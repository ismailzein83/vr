(function (appControllers) {

    'use strict';

    billingTransactionEditorController.$inject = ['$scope', 'VR_AccountBalance_BillingTransactionAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService', 'VR_AccountBalance_AccountTypeAPIService'];

    function billingTransactionEditorController($scope, VR_AccountBalance_BillingTransactionAPIService, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, VR_AccountBalance_AccountTypeAPIService) {
        var accountId;
        var accountTypeId;
        var currencySelectorAPI;
        var currencySelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var transactionTypeDirectiveAPI;
        var transactionTypeDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        var accountDirectiveAPI;
        var accountDirectiveReadyDeferred = UtilsService.createPromiseDeferred();


        loadParameters();
        defineScope();
        load();


        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                accountId = parameters.accountId;
                accountTypeId = parameters.accountTypeId;

            }

        }
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.showAccountSelector = accountId == undefined;
            $scope.scopeModel.date = new Date();
            $scope.scopeModel.onCurrencySelectorReady = function (api) {
                currencySelectorAPI = api;
                currencySelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onBillingTransactionTypeReady = function (api) {
                transactionTypeDirectiveAPI = api;
                transactionTypeDirectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.onAccountDirectiveReady = function (api) {
                accountDirectiveAPI = api;
                accountDirectiveReadyDeferred.resolve();
            }
            $scope.scopeModel.save = function () {
                return  insertBillingTransaction();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };
        }
        function load() {
            $scope.scopeModel.isLoading = true;
            if (accountId == undefined) {
                loadAccountDirective().then(function (response) {
                    loadAllControls();
                });
            }
            else
                loadAllControls();
        }

        function loadAccountDirective() {
            var loadAccountPromiseDeferred = UtilsService.createPromiseDeferred();
            accountDirectiveReadyDeferred.promise.then(function () {
                var payload = {
                    accountTypeId: accountTypeId
                };
                VRUIUtilsService.callDirectiveLoad(accountDirectiveAPI, payload, loadAccountPromiseDeferred);
            });
            return loadAccountPromiseDeferred.promise;
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadCurrencySelector, loadTransactionTypeSelector]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function setTitle() {
            $scope.title = UtilsService.buildTitleForAddEditor('Financial Transactions');
        }
        function loadStaticData() {
           
        }
        function loadCurrencySelector() {
            var currencyLoadDeferred = UtilsService.createPromiseDeferred();
            currencySelectorReadyDeferred.promise.then(function () {
                var currencySelectorPayload = {
                    selectSystemCurrency:true
                };
                VRUIUtilsService.callDirectiveLoad(currencySelectorAPI, currencySelectorPayload, currencyLoadDeferred);
            });
            return currencyLoadDeferred.promises;
        }
        function loadTransactionTypeSelector() {
            var loadTransactionTypePromiseDeferred = UtilsService.createPromiseDeferred();
            transactionTypeDirectiveReadyDeferred.promise.then(function () {
                var payload = {
                    filter: {
                        Filters: [{
                            $type: "Vanrise.AccountBalance.Entities.ManualAddEnabledBillingTransactionTypeFilter, Vanrise.AccountBalance.Entities"
                        }]
                    }
                }
                VRUIUtilsService.callDirectiveLoad(transactionTypeDirectiveAPI, payload, loadTransactionTypePromiseDeferred);
            });
            return loadTransactionTypePromiseDeferred.promise;
        }
        function loadFilterDirective() {
            if (accountId)
                return;
            var loadFilterPromiseDeferred = UtilsService.createPromiseDeferred();
            filterDirectiveReadyDeferred.promise.then(function () {

                VRUIUtilsService.callDirectiveLoad(filterDirectiveAPI, undefined, loadFilterPromiseDeferred);
            });
            return loadFilterPromiseDeferred.promise;
        }
        function insertBillingTransaction() {
            $scope.scopeModel.isLoading = true;

            var billingTransactionObj = buildBuillingTransactionObjFromScope();

            return VR_AccountBalance_BillingTransactionAPIService.AddBillingTransaction(billingTransactionObj).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('Financial Transactions', response, 'Name')) {
                    if ($scope.onBillingTransactionAdded != undefined)
                        $scope.onBillingTransactionAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function buildBuillingTransactionObjFromScope() {
            var obj = {
                AccountId: accountId != undefined ? accountId : accountDirectiveAPI.getData().selectedIds,
                Amount: $scope.scopeModel.amount,
                AccountTypeId:accountTypeId,
                CurrencyId: currencySelectorAPI.getSelectedIds(),
                TransactionTypeId:transactionTypeDirectiveAPI.getSelectedIds(),
                Notes: $scope.scopeModel.notes,
                TransactionTime: $scope.scopeModel.date,
                Reference: $scope.scopeModel.reference
            };
            return obj;
        }
    }

    appControllers.controller('VR_AccountBalance_BillingTransactionEditorController', billingTransactionEditorController);

})(appControllers);