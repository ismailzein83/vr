(function (app) {

    "use strict";

    accountBalancesManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'VRNavigationService', 'VR_AccountBalance_AccountTypeAPIService', 'VR_AccountBalance_BillingTransactionService', 'VR_AccountBalance_BalanceOrderByEnum'];

    function accountBalancesManagementController($scope, UtilsService, VRUIUtilsService, VRNotificationService, VRNavigationService, VR_AccountBalance_AccountTypeAPIService, VR_AccountBalance_BillingTransactionService, VR_AccountBalance_BalanceOrderByEnum) {
        var gridAPI;
        var gridReadyDeferred = UtilsService.createPromiseDeferred();

        var viewId;

        var accountDirectiveAPI;
        var accountDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        var accountTypeAPI;
        var accountTypeReadyDeferred = UtilsService.createPromiseDeferred();
        var accountTypeSelectedDeferred;

        var accountStatusSelectorAPI;
        var accountStatusSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var accountStatusSelectedDeferred;

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

            $scope.scopeModel.onAccountStatusSelectorReady = function (api) {
                accountStatusSelectorAPI = api;
                accountStatusSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onAccountStatusSelectionChanged = function (value) {
                if (value != undefined) {
                    if (accountStatusSelectedDeferred != undefined)
                        accountStatusSelectedDeferred.resolve();
                    else
                    {
                        $scope.isLoadingDirective = true;
                        loadAccountDirective().finally(function () {
                            $scope.isLoadingDirective = false;
                        }).catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                    }
                }
            };

            $scope.scopeModel.gridloadded = false;

            $scope.scopeModel.top = 1000;

            $scope.scopeModel.signs = [{ text: '>', value: '>', signValue: 0 }, { text: '=>', value: '>=', signValue: 1 }, { text: '<', value: '<', signValue: 2 }, { text: '<=', value: '<=', signValue: 3}];

            $scope.scopeModel.orderByOptions = UtilsService.getArrayEnum(VR_AccountBalance_BalanceOrderByEnum);

            $scope.scopeModel.orderBy = $scope.scopeModel.orderByOptions[0];

            $scope.scopeModel.onAccountTypeSelectorReady = function (api) {
                accountTypeAPI = api;
                accountTypeReadyDeferred.resolve();
            };

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridReadyDeferred.resolve();
            };

            $scope.scopeModel.searchClicked = function (api) {
               return  gridAPI.loadGrid(getFilterObject());
            };

            $scope.scopeModel.onSignSelectionChanged = function () {
                if (!$scope.scopeModel.sign)
                    $scope.scopeModel.balance = null;
            };

            $scope.scopeModel.onAccountDirectiveReady = function (api) {
                accountDirectiveAPI = api;
                accountDirectiveReadyDeferred.resolve();
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

        //Load Account Type And Subsections
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

            return UtilsService.waitMultipleAsyncOperations([loadAccountSection]).then(function () {
                loadGridDirective();
            });
        }

        //Load Account Section
        function loadAccountSection()
        {
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


        //Load Grid Section

        function loadGridDirective() {
            gridReadyDeferred.promise.then(function () {
                gridAPI.loadGrid(getFilterObject());
            });
        }

        function getFilterObject() {
            var accountData = (accountDirectiveAPI != undefined) ? accountDirectiveAPI.getData() : undefined;
            var accountsIds;
            if(accountData != undefined)
                accountsIds = accountData.selectedIds;
            var accountStatusObj = accountStatusSelectorAPI.getData();


            return {
                query:{
                    AccountTypeId: accountTypeAPI.getSelectedIds(),
                    Top: $scope.scopeModel.top,
                    AccountsIds: accountsIds,
                    Sign: $scope.scopeModel.sign != undefined ? $scope.scopeModel.sign.value : undefined,
                    Balance: $scope.scopeModel.sign != undefined ? $scope.scopeModel.balance : undefined,
                    OrderBy: $scope.scopeModel.orderBy.value,
                    Status: accountStatusObj != undefined ? accountStatusObj.Status : undefined,
                    EffectiveDate: accountStatusObj != undefined ? accountStatusObj.EffectiveDate : undefined,
                    IsEffectiveInFuture: accountStatusObj != undefined ? accountStatusObj.IsEffectiveInFuture : undefined,
                },
                accountTypeId : accountTypeAPI.getSelectedIds()
            };
        }
    }

    app.controller('VR_AccountBalance_AccountBalancesManagementController', accountBalancesManagementController);
})(app);