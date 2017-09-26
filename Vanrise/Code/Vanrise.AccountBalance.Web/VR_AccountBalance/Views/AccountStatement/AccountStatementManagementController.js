(function (appControllers) {

    "use strict";

    accountStatementManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VR_AccountBalance_AccountTypeAPIService', 'VRNotificationService', 'VRDateTimeService'];

    function accountStatementManagementController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VR_AccountBalance_AccountTypeAPIService, VRNotificationService, VRDateTimeService) {
        var viewId;

        var accountDirectiveAPI;
        var accountDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        var accountTypeAPI;
        var accountTypeReadyDeferred = UtilsService.createPromiseDeferred();
        var accountTypeSelectedDeferred;

        var accountStatusSelectorAPI;
        var accountStatusSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var accountStatusSelectedDeferred;

        var gridAPI;
        var gridReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
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

            var date = VRDateTimeService.getNowDateTime();

            $scope.scopeModel.fromDate = new Date(date.getFullYear(), date.getMonth() - 1, 1, 0, 0, 0, 0);

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

            $scope.scopeModel.searchClicked = function () {
                return gridAPI.loadGrid(getFilterObject());
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


        function getFilterObject() {
            var accountObject;
            if (accountDirectiveAPI != undefined)
                accountObject = accountDirectiveAPI.getData();

            var accountStatusObj = accountStatusSelectorAPI.getData();

            var query = {
                FromDate: $scope.scopeModel.fromDate,
                AccountId: accountObject != undefined ? accountObject.selectedIds : undefined,
                AccountTypeId: accountTypeAPI.getSelectedIds(),
                Status: accountStatusObj != undefined ? accountStatusObj.Status : undefined,
                EffectiveDate: accountStatusObj != undefined ? accountStatusObj.EffectiveDate : undefined,
                IsEffectiveInFuture: accountStatusObj != undefined ? accountStatusObj.IsEffectiveInFuture : undefined,
            };
            return query;

        }
    }

    appControllers.controller('VR_AccountBalance_AccountStatementManagementController', accountStatementManagementController);
})(appControllers);