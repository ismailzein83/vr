(function (app) {

    "use strict";

    billingTransactionManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'VRNavigationService', 'VR_AccountBalance_AccountTypeAPIService', 'VR_AccountBalance_BillingTransactionService', 'VR_AccountBalance_BillingTransactionAPIService', 'VRValidationService', 'VRDateTimeService'];

    function billingTransactionManagementController($scope, UtilsService, VRUIUtilsService, VRNotificationService, VRNavigationService, VR_AccountBalance_AccountTypeAPIService, VR_AccountBalance_BillingTransactionService, VR_AccountBalance_BillingTransactionAPIService, VRValidationService, VRDateTimeService) {
        var gridAPI;
        var gridReadyDeferred = UtilsService.createPromiseDeferred();

        var viewId;

    
        var accountTypeAPI;
        var accountTypeReadyDeferred = UtilsService.createPromiseDeferred();
        var accountTypeSelectedDeferred;

        var accountDirectiveAPI;
        var accountDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        var accountStatusSelectorAPI;
        var accountStatusSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var accountStatusSelectedDeferred;

        var transactionTypeDirectiveAPI;
        var transactionTypeDirectiveReadyDeferred = UtilsService.createPromiseDeferred();


        loadParameters();
        defineScope();

        var filter = {};
        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != null) {
                viewId = parameters.viewId;
            }
        }
        
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.showAddButton = false;

            var now = VRDateTimeService.getNowDateTime();
            $scope.scopeModel.fromTime = new Date(now.getFullYear(), now.getMonth(), 1);

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

            $scope.scopeModel.onAccountTypeSelectorReady = function (api) {
                accountTypeAPI = api;
                accountTypeReadyDeferred.resolve();
            };


            $scope.scopeModel.onAccountTypeSelectorSelectionChange = function () {
                if (accountTypeAPI.getSelectedIds() != undefined) {
                    if (accountTypeSelectedDeferred != undefined)
                        accountTypeSelectedDeferred.resolve();
                    else {
                        $scope.scopeModel.isLoading = true;
                       loadSearchCriteria().finally(function () {
                            $scope.scopeModel.isLoading = false;
                        }).catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                    }
                }
            };

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridReadyDeferred.resolve();

            };
           
            $scope.scopeModel.onAccountDirectiveReady = function (api) {
                accountDirectiveAPI = api;
                accountDirectiveReadyDeferred.resolve();
            };
            $scope.scopeModel.searchClicked = function (api) {
             
                return gridAPI.loadGrid( getFilterObject());
            };
            $scope.validateDateTime = function () {
                return VRValidationService.validateTimeRange($scope.scopeModel.fromTime, $scope.scopeModel.toTime);
            };
            $scope.scopeModel.addClicked = function (api) {
                var onBillingTransactionAdded = function (transactionObj) {
                    if (gridAPI != undefined) {
                        gridAPI.onBillingTransactionAdded(transactionObj);
                    }
                };
                VR_AccountBalance_BillingTransactionService.addBillingTransaction(undefined, accountTypeAPI.getSelectedIds(), onBillingTransactionAdded);
            };
           
            $scope.scopeModel.onBillingTransactionTypeReady = function (api) {
                transactionTypeDirectiveAPI = api;
                transactionTypeDirectiveReadyDeferred.resolve();
            };
            UtilsService.waitMultiplePromises([accountTypeReadyDeferred.promise]).then(function () {
                load();
            });
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAccountTypeAndSubsections().finally(function () {
                $scope.scopeModel.isLoading = false;
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }

        function checkHasAddBillingTransactionPermission() {
            VR_AccountBalance_BillingTransactionAPIService.HasAddBillingTransactionPermission(accountTypeAPI.getSelectedIds()).then(function (response) {
                $scope.scopeModel.showAddButton = response;
            });
        }

        function loadAccountTypeAndSubsections() {
            var loadAccountTypeAndSubsectionsPromiseDeferred = UtilsService.createPromiseDeferred();

            var promises = [];
            accountTypeSelectedDeferred = UtilsService.createPromiseDeferred();
            promises.push(accountTypeSelectedDeferred.promise);
            promises.push(loadAccountType());

            UtilsService.waitMultiplePromises(promises).then(function () {
                accountTypeSelectedDeferred = undefined;
                $scope.scopeModel.hideAccountType = accountTypeAPI.hasSingleItem();
                loadSearchCriteria().then(function () {
                    loadAccountTypeAndSubsectionsPromiseDeferred.resolve();
                }).catch(function (error) {
                    loadAccountTypeAndSubsectionsPromiseDeferred.reject(error);
                });
            }).catch(function (error) {
                loadAccountTypeAndSubsectionsPromiseDeferred.reject(error);
            });

            return loadAccountTypeAndSubsectionsPromiseDeferred.promise;
        }

        function loadAccountType() {
            var payLoad = {
                filter: {
                    Filters: [{
                        $type: "Vanrise.AccountBalance.Business.AccountTypeViewFilter, Vanrise.AccountBalance.Business",
                        ViewId: viewId
                    }]
                },
                selectfirstitem: true
            };
            return accountTypeAPI.load(payLoad);
        }
   
        function loadSearchCriteria() {

            return UtilsService.waitMultipleAsyncOperations([loadAccountSection, checkHasAddBillingTransactionPermission, loadTransactionTypeSelector]).then(function () {
                loadGridDirective();
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
                var accountStatusSelectorPayload = { selectFirstItem: true };

                VRUIUtilsService.callDirectiveLoad(accountStatusSelectorAPI, accountStatusSelectorPayload, loadAccountStatusSelectorPromiseDeferred);
            });
            return loadAccountStatusSelectorPromiseDeferred.promise;
        }

        function loadAccountDirective() {
            var loadAccountSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
            accountDirectiveReadyDeferred.promise.then(function () {
                var payload = {
                    accountTypeId: accountTypeAPI.getSelectedIds(),
                    filter: accountStatusSelectorAPI.getData()
                };
                VRUIUtilsService.callDirectiveLoad(accountDirectiveAPI, payload, loadAccountSelectorPromiseDeferred);
            });
            return loadAccountSelectorPromiseDeferred.promise
        }

        function loadGridDirective() {
            gridReadyDeferred.promise.then(function () {
                gridAPI.loadGrid(getFilterObject());
            });
        }
    
        function loadTransactionTypeSelector() {
            var loadTransactionTypePromiseDeferred = UtilsService.createPromiseDeferred();
            transactionTypeDirectiveReadyDeferred.promise.then(function () {
                var transactionTypePayload = {
                    filter:{
                        AccountTypeId: accountTypeAPI.getSelectedIds()
                    }
                };
                VRUIUtilsService.callDirectiveLoad(transactionTypeDirectiveAPI, transactionTypePayload, loadTransactionTypePromiseDeferred);
            });
            return loadTransactionTypePromiseDeferred.promise;
        }

        function getFilterObject() {
            var accountStatusObj = accountStatusSelectorAPI.getData();
            var payload = {
                query: {
                    FromTime: $scope.scopeModel.fromTime,
                    ToTime: $scope.scopeModel.toTime,
                    AccountTypeId: accountTypeAPI.getSelectedIds(),
                    TransactionTypeIds: transactionTypeDirectiveAPI.getSelectedIds(),
                    AccountsIds: (accountDirectiveAPI != undefined) ? accountDirectiveAPI.getData().selectedIds : null,
                    Status: accountStatusObj != undefined ? accountStatusObj.Status : undefined,
                    EffectiveDate: accountStatusObj != undefined ? accountStatusObj.EffectiveDate : undefined,
                    IsEffectiveInFuture: accountStatusObj != undefined ? accountStatusObj.IsEffectiveInFuture : undefined,
                }
            };
            return payload;
        }
    }

    app.controller('VR_AccountBalance_BillingTransactionManagementController', billingTransactionManagementController);
})(app);