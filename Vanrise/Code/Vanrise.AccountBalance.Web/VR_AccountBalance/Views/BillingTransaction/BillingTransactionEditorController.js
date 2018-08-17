(function (appControllers) {

    'use strict';

    billingTransactionEditorController.$inject = ['$scope', 'VR_AccountBalance_BillingTransactionAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService', 'VR_AccountBalance_AccountTypeAPIService', 'VR_AccountBalance_AccountAPIService', 'VRDateTimeService'];

    function billingTransactionEditorController($scope, VR_AccountBalance_BillingTransactionAPIService, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, VR_AccountBalance_AccountTypeAPIService, VR_AccountBalance_AccountAPIService, VRDateTimeService) {

        var accountId;
        var accountTypeId;
        var billingTransactionId;
        var billingTransactionEntity;
        var context;
        var accountStatusSelectorAPI;
        var accountStatusSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var accountStatusSelectedDeferred;

        var accountSelectorAPI;
        var accountSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var accountSelectorContext;

        var currencySelectorAPI;
        var currencySelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var billingTransactionTypeSelectorAPI;
        var billingTransactionTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var accountBalanceInvoicesGridAPI;
        var accountBalanceInvoicesGridReadyDeferred = UtilsService.createPromiseDeferred();

        var attachmentFieldTypeManagementAPI;
        var attachmentFieldTypeManagementReadyDeferred = UtilsService.createPromiseDeferred();

        var isViewMode;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                accountId = parameters.accountId;
                accountTypeId = parameters.accountTypeId;
                billingTransactionId = parameters.billingTransactionId;
                context = parameters.context;
            }
            isViewMode = billingTransactionId != undefined;
        }
        function defineScope() {
            $scope.scopeModel = {};


            $scope.scopeModel.onAccountStatusSelectorReady = function (api) {
                accountStatusSelectorAPI = api;
                accountStatusSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onAccountStatusSelectionChanged = function (value) {
                if (value != undefined) {
                    if (accountStatusSelectedDeferred != undefined)
                        accountStatusSelectedDeferred.resolve();
                    else {
                        $scope.isLoadingDirective = true;
                        loadAccountDirective().finally(function () {
                            $scope.isLoadingDirective = false;
                        }).catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                    }
                }
            };
            $scope.scopeModel.showAccountSelector = accountId == undefined && billingTransactionId==undefined;
            $scope.scopeModel.date = VRDateTimeService.getNowDateTime();

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

            $scope.scopeModel.onAccountBalanceInvoicesGridReady = function (api) {
                accountBalanceInvoicesGridAPI = api;
                accountBalanceInvoicesGridReadyDeferred.resolve();
            };
            $scope.scopeModel.onAttachmentFieldTypeManagementReady = function (api) {
                attachmentFieldTypeManagementAPI = api;
                attachmentFieldTypeManagementReadyDeferred.resolve();
            };
            $scope.scopeModel.save = function () {
                if (attachmentFieldTypeManagementAPI != undefined) {
                    if (attachmentFieldTypeManagementAPI.getData() == undefined) {
                        VRNotificationService.showConfirmation("No attachments were added. Are you sure you want to continue?").then(function (response) {
                            if (response) {
                                return insertBillingTransaction();
                            }
                        });
                    }
                    else {
                        return insertBillingTransaction();
                    }
                }
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

            $scope.scopeModel.validateAmount = function () {
                if ($scope.scopeModel.amount == undefined)
                    return 'Amount is a required field';
                if (isNaN($scope.scopeModel.amount))
                    return 'Amount is an invalid number';
                var amount = Number($scope.scopeModel.amount);
                if (amount <= 0)
                    return 'Amount must be > 0';
                return null;
            };

            setAccountSelectorContext();
        }
        function load() {
            $scope.scopeModel.isLoading = true;
            if (isViewMode) {
                getBillingTransactionEntity().then(function () {
                    getAccountTypeSettings().then(function () {
                        if (accountId == undefined) {
                            loadAccountSection().then(function (response) {
                                loadAllControls();
                            });
                        }
                        else
                            loadAllControls();
                    });
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            }
            else {
                getAccountTypeSettings().then(function () {
                    if (accountId == undefined) {
                        loadAccountSection().then(function (response) {
                            loadAllControls();
                        });
                    }
                    else
                        loadAllControls();
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            }
        }
        function getAccountTypeSettings()
        {
            return VR_AccountBalance_AccountTypeAPIService.GetAccountTypeSettings(accountTypeId).then(function (response) {
                $scope.scopeModel.useAccountInvoicesGrid = response.InvToAccBalanceRelationId != undefined;
            });
        }

        function getBillingTransactionEntity() {
            return VR_AccountBalance_BillingTransactionAPIService.GetBillingTransactionById(billingTransactionId).then(function (billingTransaction) {
                billingTransactionEntity = billingTransaction;
                if (billingTransaction!=undefined) {
                    accountTypeId = billingTransaction.AccountTypeId;
                    accountId = billingTransaction.AccountId;
                }
            });
        }
        function loadAllControls() {

            function setTitle() {
                $scope.title = UtilsService.buildTitleForAddEditor('Financial Transactions');
            }
            function loadStaticData() {
                if (billingTransactionEntity !=undefined) {
                    $scope.scopeModel.amount = billingTransactionEntity.Amount;
                    $scope.scopeModel.reference = billingTransactionEntity.Reference;
                    $scope.scopeModel.notes = billingTransactionEntity.Notes;
                }
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
                        },
                        selectedIds: billingTransactionEntity != undefined ? billingTransactionEntity.TransactionTypeId: undefined
                    };
                    VRUIUtilsService.callDirectiveLoad(billingTransactionTypeSelectorAPI, payload, loadTransactionTypePromiseDeferred);
                });
                return loadTransactionTypePromiseDeferred.promise;
            }
            function loadAccountBalanceInvoicesByAccountParameter()
            {
                if (accountId != undefined && $scope.scopeModel.useAccountInvoicesGrid)
                {
                  return  loadAccountBalanceInvoices(accountId);
                }
            }

            function loadAttachmentFieldTypeManagement(){
                var attachmentFieldTypeManagementLoadDeferred = UtilsService.createPromiseDeferred();
                attachmentFieldTypeManagementReadyDeferred.promise.then(function () {
                    var attachmentFieldTypeManagementPayload = {
                        attachementFieldTypes: billingTransactionEntity != undefined && billingTransactionEntity.Settings != undefined ? billingTransactionEntity.Settings.Attachments : undefined
                    };
                    attachmentFieldTypeManagementAPI.loadGrid(attachmentFieldTypeManagementPayload).then(function() {
                        attachmentFieldTypeManagementLoadDeferred.resolve();
                    });
                });
                return attachmentFieldTypeManagementLoadDeferred.promise;
            }
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadCurrencySelector, loadBillingTransactionTypeSelector, loadAccountBalanceInvoicesByAccountParameter, loadAttachmentFieldTypeManagement]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
    
        function loadAccountSection() {
            var loadAccountSectionPromiseDeferred = UtilsService.createPromiseDeferred();
            accountStatusSelectedDeferred = UtilsService.createPromiseDeferred();

            var promises = [];

            promises.push(loadAccountStatusSelectorDirective());
            promises.push(accountStatusSelectedDeferred.promise);

            UtilsService.waitMultiplePromises(promises).then(function () {
                accountStatusSelectedDeferred = undefined;
                loadAccountDirective().then(function () {
                    loadAccountSectionPromiseDeferred.resolve();
                }).catch(function (error) {
                    loadAccountSectionPromiseDeferred.reject(error);
                });
            }).catch(function (error) {
                loadAccountSectionPromiseDeferred.reject(error);
            });
            return loadAccountSectionPromiseDeferred.promise;
        }

        function loadAccountStatusSelectorDirective() {
            var loadAccountStatusSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
            accountStatusSelectorReadyDeferred.promise.then(function () {
                var accountStatusSelectorPayload = { selectFirstItem: true,dontShowInActive: true };

                VRUIUtilsService.callDirectiveLoad(accountStatusSelectorAPI, accountStatusSelectorPayload, loadAccountStatusSelectorPromiseDeferred);
            });
            return loadAccountStatusSelectorPromiseDeferred.promise;
        }

        function loadAccountDirective() {
            var loadAccountSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
            accountSelectorReadyDeferred.promise.then(function () {
                var payload = {
                    accountTypeId: accountTypeId,
                    filter: accountStatusSelectorAPI.getData(),
                    context: accountSelectorContext
                };
                VRUIUtilsService.callDirectiveLoad(accountSelectorAPI, payload, loadAccountSelectorPromiseDeferred);
            });
            return loadAccountSelectorPromiseDeferred.promise;
        }

        function loadGridDirective() {
            gridReadyDeferred.promise.then(function () {
                gridAPI.loadGrid(getFilterObject());
            });
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
                Reference: $scope.scopeModel.reference,
                Settings: {
                    $type:"Vanrise.AccountBalance.Entities.BillingTransactionSettings, Vanrise.AccountBalance.Entities",
                    Attachments: attachmentFieldTypeManagementAPI.getData()
                }
            };
            return obj;
        }
        function loadAccountBalanceInvoices(selectedAccountId) {
            var accountBalanceInvoicesPromiseDeferred = UtilsService.createPromiseDeferred();
            accountBalanceInvoicesGridReadyDeferred.promise.then(function () {
                var payload = {
                    AccountTypeId: accountTypeId,
                    AccountId: selectedAccountId
                };
                VRUIUtilsService.callDirectiveLoad(accountBalanceInvoicesGridAPI, payload, accountBalanceInvoicesPromiseDeferred);
            });
            return accountBalanceInvoicesPromiseDeferred.promise;
        }
        function setAccountSelectorContext() {
            accountSelectorContext = {};

            accountSelectorContext.onAccountSelected = function (selectedAccountId) {
                $scope.scopeModel.isLoading = true;
                if ($scope.scopeModel.useAccountInvoicesGrid)
                  loadAccountBalanceInvoices(selectedAccountId);

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