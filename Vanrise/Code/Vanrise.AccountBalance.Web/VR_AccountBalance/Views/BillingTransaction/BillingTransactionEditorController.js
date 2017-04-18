(function (appControllers) {

    'use strict';

    billingTransactionEditorController.$inject = ['$scope', 'VR_AccountBalance_BillingTransactionAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService', 'VR_AccountBalance_AccountTypeAPIService', 'VR_AccountBalance_AccountAPIService'];

    function billingTransactionEditorController($scope, VR_AccountBalance_BillingTransactionAPIService, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, VR_AccountBalance_AccountTypeAPIService, VR_AccountBalance_AccountAPIService) {

        var accountId;
        var accountTypeId;

        var accountSelectorAPI;
        var accountSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var accountSelectorContext;

        var currencySelectorAPI;
        var currencySelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var billingTransactionTypeSelectorAPI;
        var billingTransactionTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

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

            $scope.scopeModel.onAccountSelectorReady = function (api) {
                accountSelectorAPI = api;
                accountSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onCurrencySelectorReady = function (api) {
                currencySelectorAPI = api;
                currencySelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onBillingTransactionTypeSelectorReady = function (api) {
                billingTransactionTypeSelectorAPI = api;
                billingTransactionTypeSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.save = function () {
                return insertBillingTransaction();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };

            setAccountSelectorContext();
        }
        function load() {
            $scope.scopeModel.isLoading = true;
            if (accountId == undefined) {
                loadAccountSelector().then(function (response) {
                    loadAllControls();
                });
            }
            else
                loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadCurrencySelector, loadBillingTransactionTypeSelector]).catch(function (error) {
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
        function loadAccountSelector() {
            var loadAccountPromiseDeferred = UtilsService.createPromiseDeferred();
            accountSelectorReadyDeferred.promise.then(function () {
                var payload = {
                    accountTypeId: accountTypeId,
                    context: accountSelectorContext
                };
                VRUIUtilsService.callDirectiveLoad(accountSelectorAPI, payload, loadAccountPromiseDeferred);
            });
            return loadAccountPromiseDeferred.promise;
        }
        function loadCurrencySelector() {
            var promises = [];

            var getAccountInfoDeferred = UtilsService.createPromiseDeferred();
            promises.push(getAccountInfoDeferred.promise);

            var currencyLoadDeferred = UtilsService.createPromiseDeferred();
            promises.push(currencyLoadDeferred.promise);

            var selectedCurrencyId;

            if (accountId == undefined)
                getAccountInfoDeferred.resolve();
            else {
                VR_AccountBalance_AccountAPIService.GetAccountInfo(accountTypeId, accountId).then(function (response) {
                    if (response != undefined)
                        selectedCurrencyId = response.CurrencyId;
                    getAccountInfoDeferred.resolve();
                }).catch(function (error) {
                    getAccountInfoDeferred.reject(error);
                });
            }

            getAccountInfoDeferred.promise.then(function () {
                currencySelectorReadyDeferred.promise.then(function () {
                    var currencySelectorPayload = { selectedIds: selectedCurrencyId };
                    VRUIUtilsService.callDirectiveLoad(currencySelectorAPI, currencySelectorPayload, currencyLoadDeferred);
                });
            });

            return UtilsService.waitMultiplePromises(promises);
        }
        function loadBillingTransactionTypeSelector() {
            var loadTransactionTypePromiseDeferred = UtilsService.createPromiseDeferred();
            billingTransactionTypeSelectorReadyDeferred.promise.then(function () {
                var payload = {
                    filter: {
                        AccountTypeId: accountTypeId,
                        Filters: [{
                            $type: "Vanrise.AccountBalance.Entities.ManualAddEnabledBillingTransactionTypeFilter, Vanrise.AccountBalance.Entities"
                        }]
                    }
                };
                VRUIUtilsService.callDirectiveLoad(billingTransactionTypeSelectorAPI, payload, loadTransactionTypePromiseDeferred);
            });
            return loadTransactionTypePromiseDeferred.promise;
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
                AccountId: accountId != undefined ? accountId : accountSelectorAPI.getData().selectedIds,
                Amount: $scope.scopeModel.amount,
                AccountTypeId: accountTypeId,
                CurrencyId: currencySelectorAPI.getSelectedIds(),
                TransactionTypeId: billingTransactionTypeSelectorAPI.getSelectedIds(),
                Notes: $scope.scopeModel.notes,
                TransactionTime: $scope.scopeModel.date,
                Reference: $scope.scopeModel.reference
            };
            return obj;
        }

        function setAccountSelectorContext() {
            accountSelectorContext = {};

            accountSelectorContext.onAccountSelected = function (selectedAccountId) {
                $scope.scopeModel.isLoading = true;
                VR_AccountBalance_AccountAPIService.GetAccountInfo(accountTypeId, selectedAccountId).then(function (response) {
                    if (response != undefined) {
                        currencySelectorAPI.selectedCurrency(response.CurrencyId);
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
            };
        }
    }

    appControllers.controller('VR_AccountBalance_BillingTransactionEditorController', billingTransactionEditorController);

})(appControllers);