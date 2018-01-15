(function (appControllers) {

    'use strict';

    AccountManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'VRNavigationService', 'Retail_BE_AccountBEService', 'Retail_BE_AccountBEAPIService', 'Retail_BE_AccountTypeAPIService', 'VR_Sec_ViewAPIService','Retail_BE_AccountBEDefinitionAPIService'];

    function AccountManagementController($scope, UtilsService, VRUIUtilsService, VRNotificationService, VRNavigationService, Retail_BE_AccountBEService, Retail_BE_AccountBEAPIService, Retail_BE_AccountTypeAPIService, VR_Sec_ViewAPIService, Retail_BE_AccountBEDefinitionAPIService) {

        var viewId;
        var accountBEDefinitionId;
        var accountFields;

        var businessEntityDefinitionSelectorAPI;
        var businessEntityDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var accountRootTypeSelectorAPI;
        var accountRootTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var accountTypeSelectorAPI;
        var accountTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var recordFilterDirectiveAPI;
        var recordFilterDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        var statusSelectorAPI;
        var statusSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();


        var gridAPI;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != null) {
                viewId = parameters.viewId;
            }
        }
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.isGridLoaded = false;
            $scope.scopeModel.isAccountBEDefinitionSelected = false;
            $scope.scopeModel.showBusinessEntityDefinitionSelector = false;
            $scope.scopeModel.showAddAccount = false;
            $scope.scopeModel.onlyRootAccount = true;
            $scope.scopeModel.onStatusSelectorReady = function (api) {
                statusSelectorAPI = api;
                statusSelectorReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.onBusinessEntityDefinitionSelectorReady = function (api) {
                businessEntityDefinitionSelectorAPI = api;
                businessEntityDefinitionSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onAccountRootTypeSelectorReady = function (api) {
                accountRootTypeSelectorAPI = api;
                accountRootTypeSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onAccountTypeSelectorReady = function (api) {
                accountTypeSelectorAPI = api;
                accountTypeSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onRecordFilterDirectiveReady = function (api) {
                recordFilterDirectiveAPI = api;
                recordFilterDirectiveReadyDeferred.resolve();
            };
            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                var gridPayload = {
                    accountBEDefinitionId: accountBEDefinitionId,
                    query: {
                        OnlyRootAccount: $scope.scopeModel.onlyRootAccount,
                        StatusIds: statusSelectorAPI.getSelectedIds()
                    }
                };
                gridAPI.load(gridPayload);
            };

            $scope.scopeModel.onBusinessEntityDefinitionSelectionChanged = function (selectedBusinessEntityDefinition) {

                if (selectedBusinessEntityDefinition != undefined) {
                    $scope.scopeModel.isLoading = true;
                    $scope.scopeModel.isGridLoaded = false;
                    $scope.scopeModel.isAccountBEDefinitionSelected = true;

                    accountBEDefinitionId = selectedBusinessEntityDefinition.BusinessEntityDefinitionId;

                    Retail_BE_AccountBEAPIService.DoesUserHaveAddAccess(accountBEDefinitionId).then(function (response) {
                        $scope.scopeModel.showAddAccount = response;
                    });
                    loadAllControls().then(function () {
                        $scope.scopeModel.isGridLoaded = true;
                    });
                }
            };
            $scope.scopeModel.onlyRootAccountValueChanged = function () {
                if (accountRootTypeSelectorAPI.getSelectedIds() != undefined) {
                    var payload = {
                        filter: {
                            AccountBEDefinitionId: accountBEDefinitionId,
                            RootAccountTypeOnly: accountRootTypeSelectorAPI.getSelectedIds()
                        }
                    };
                    var setLoader = function (value) {
                        $scope.scopeModel.isAccountTypeSelectorLoading = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, accountTypeSelectorAPI, payload, setLoader, undefined);
                }

            };

            $scope.scopeModel.search = function () {
                var gridPayload = {
                    accountBEDefinitionId: accountBEDefinitionId,
                    query: buildGridQuery()
                };

                return gridAPI.load(gridPayload);
            };
            $scope.scopeModel.add = function () {
                var onAccountAdded = function (addedAccount) {
                    gridAPI.onAccountAdded(addedAccount);
                };

                Retail_BE_AccountBEService.addAccount(accountBEDefinitionId, undefined, onAccountAdded);
            };

            //$scope.scopeModel.hasAddAccountPermission = function () {
            //    return Retail_BE_AccountBEAPIService.HasAddAccountPermission();
            //};
        }
        function load() {
            $scope.scopeModel.isLoading = true;
            loadBEDefinitionSelectorLabel().then(function () {
                loadBusinessEntityDefinitionSelector();
            });
        }

        function loadBEDefinitionSelectorLabel() {
            return VR_Sec_ViewAPIService.GetView(viewId).then(function (response) {
                if (response != undefined && response.Settings != undefined) {
                    $scope.scopeModel.BEDefinitionSelectorLabel = response.Settings.AccountDefinitionSelectorLabel;
                }
            });
        }
        function loadBusinessEntityDefinitionSelector() {
            var businessEntityDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            businessEntityDefinitionSelectorReadyDeferred.promise.then(function () {

                var payload = {
                    filter: {
                        Filters: [{
                            $type: "Retail.BusinessEntity.Business.AccountBEDefinitionViewFilter, Retail.BusinessEntity.Business",
                            ViewId: viewId
                        }]
                    },
                    selectFirstItem: true
                };
                VRUIUtilsService.callDirectiveLoad(businessEntityDefinitionSelectorAPI, payload, businessEntityDefinitionSelectorLoadDeferred);
            });

            return businessEntityDefinitionSelectorLoadDeferred.promise.then(function () {
                setTimeout(function () {
                    $scope.scopeModel.showBusinessEntityDefinitionSelector = !businessEntityDefinitionSelectorAPI.hasSingleItem();
                });
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadRootAccountTypeSelector, loadRecordFilterDirective, loadStatusSelector]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        
        function loadRootAccountTypeSelector() {
            var rootAccountTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            accountRootTypeSelectorReadyDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(accountRootTypeSelectorAPI, undefined, rootAccountTypeSelectorLoadDeferred);
            });

            return rootAccountTypeSelectorLoadDeferred.promise;
        }
        function loadStatusSelector() {
            var statusSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            Retail_BE_AccountBEDefinitionAPIService.GetAccountBEStatusDefinitionId(accountBEDefinitionId).then(function (response) {
                statusSelectorReadyPromiseDeferred.promise.then(function () {
                    var selectorPayload = {
                        businessEntityDefinitionId: response
                    }
                    VRUIUtilsService.callDirectiveLoad(statusSelectorAPI, selectorPayload, statusSelectorLoadDeferred);
                });
            }).catch(function (error) {
                 statusSelectorLoadDeferred.reject(error);
            });

            return statusSelectorLoadDeferred.promise;
        }
        function loadRecordFilterDirective() {
            var recordFilterDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

            recordFilterDirectiveReadyDeferred.promise.then(function () {

                loadAccountFields().then(function () {

                    var recordFilterDirectivePayload = {
                        context: buildContext()
                    };
                    VRUIUtilsService.callDirectiveLoad(recordFilterDirectiveAPI, recordFilterDirectivePayload, recordFilterDirectiveLoadDeferred);
                });
            });

            return recordFilterDirectiveLoadDeferred.promise;
        }

        function loadAccountFields() {
            return Retail_BE_AccountTypeAPIService.GetGenericFieldDefinitionsInfo(accountBEDefinitionId).then(function (response) {
                accountFields = response;
            });
        }
        function buildContext() {
            var context = {
                getFields: function () {
                    var fields = [];
                    if (accountFields != undefined) {
                        for (var i = 0; i < accountFields.length; i++) {
                            var accountField = accountFields[i];

                            fields.push({
                                FieldName: accountField.Name,
                                FieldTitle: accountField.Title,
                                Type: accountField.FieldType
                            });
                        }
                    }
                    return fields;
                }
            };
            return context;
        };
        function buildGridQuery() {
            return {
                Name: $scope.scopeModel.name,
                OnlyRootAccount: accountRootTypeSelectorAPI.getSelectedIds(),
                AccountTypeIds: accountTypeSelectorAPI.getSelectedIds(),
                FilterGroup: recordFilterDirectiveAPI.getData().filterObj,
                StatusIds: statusSelectorAPI.getSelectedIds()
            };
        }
    }

    appControllers.controller('Retail_BE_AccountManagementController', AccountManagementController);

})(appControllers);