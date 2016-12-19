(function (appControllers) {

    'use strict';

    BillingTransactionController.$inject = ['$scope', 'VR_AccountBalance_BillingTransactionAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService', 'VR_AccountBalance_AccountTypeAPIService'];

    function BillingTransactionController($scope, VR_AccountBalance_BillingTransactionAPIService, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, VR_AccountBalance_AccountTypeAPIService) {
        var accountId;
        var accountTypeId;
        var currencySelectorAPI;
        var currencySelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var transactionTypeDirectiveAPI;
        var transactionTypeDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        var filterDirectiveAPI;
        var filterDirectiveReadyDeferred = UtilsService.createPromiseDeferred();


        loadParameters();
        defineScope();
        load();


        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                accountId = parameters.accountId;
                accountTypeId = parameters.accountTypeId;

            }
            $scope.showFilter = accountId == undefined;

        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onCurrencySelectorReady = function (api) {
                currencySelectorAPI = api;
                currencySelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onBillingTransactionTypeReady = function (api) {
                transactionTypeDirectiveAPI = api;
                transactionTypeDirectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.onFilterDirectiveReady = function (api) {
                filterDirectiveAPI = api;
                filterDirectiveReadyDeferred.resolve();
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
            if (accountTypeId != undefined) {
                VR_AccountBalance_AccountTypeAPIService.GetAccountSelector(accountTypeId).then(function (response) {
                    $scope.filterEditor = response;
                    loadAllControls();
                });
            }
            else
                loadAllControls();
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
                var currencySelectorPayload;
                VRUIUtilsService.callDirectiveLoad(currencySelectorAPI, currencySelectorPayload, currencyLoadDeferred);
            });
            return currencyLoadDeferred.promises;
        }
        function loadTransactionTypeSelector() {
            var loadTransactionTypePromiseDeferred = UtilsService.createPromiseDeferred();
            transactionTypeDirectiveReadyDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(transactionTypeDirectiveAPI, undefined, loadTransactionTypePromiseDeferred);
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
                AccountId: accountId != undefined ? accountId : filterDirectiveAPI.getData().selectedIds,
                Amount: $scope.scopeModel.amount,
                AccountTypeId:accountTypeId,
                CurrencyId: currencySelectorAPI.getSelectedIds(),
                TransactionTypeId:transactionTypeDirectiveAPI.getSelectedIds(),
                Notes: $scope.scopeModel.notes,
                Reference: $scope.scopeModel.reference
            };
            return obj;
        }
    }

    appControllers.controller('VR_AccountBalance_BillingTransactionController', BillingTransactionController);

})(appControllers);