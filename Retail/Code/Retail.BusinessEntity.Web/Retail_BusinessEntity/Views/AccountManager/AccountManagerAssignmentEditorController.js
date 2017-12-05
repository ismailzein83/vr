(function (appControllers) {

    "use strict";

    accountManagerAssignmentEditor.$inject = ["$scope", "UtilsService", "VRNotificationService", "VRNavigationService", "VRUIUtilsService", "Retail_BE_AccountManagerAssignmentAPIService", "VRValidationService", "InsertOperationResultEnum", "UpdateOperationResultEnum"];

    function accountManagerAssignmentEditor($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, Retail_BE_AccountManagerAssignmentAPIService, VRValidationService, InsertOperationResultEnum, UpdateOperationResultEnum) {
        var isEditMode;
        var subViewDefinitionEntity;

        var accountManagerDefinitionId;
        var accountManagerAssignementDefinitionId;
        var accountManagerId;

        var accountManagerSelectorAPI;
        var accountManagerReadyDeferred = UtilsService.createPromiseDeferred();

        var runtimeDirectiveAPI;
        var runtimeDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        var accountManagerAssignmentRuntime;

        var accountManagerAssignmentId;

        var accountBEDefinitionId;

        var accountId;
        var accountActionDefinition;

        var selectorPayload;

        var accountSelectorAPI;
        var accountSelectorReadyDeferred = UtilsService.createPromiseDeferred();
       
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                accountManagerDefinitionId = parameters.accountManagerDefinitionId;
                accountManagerAssignementDefinitionId = parameters.accountManagerAssignementDefinitionId;
                accountManagerAssignmentId = parameters.accountManagerAssignmentId;
                accountBEDefinitionId = parameters.accountBEDefinitionId;
                accountId = parameters.accountId;
                accountActionDefinition = parameters.accountActionDefinition;
                accountManagerId = parameters.accountManagerId;
            }
            isEditMode = (accountManagerAssignmentId != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
            $scope.scopeModel.beginEffectiveDate = new Date();
            $scope.scopeModel.validateEffectiveDate = function () {
                return VRValidationService.validateTimeRange($scope.scopeModel.beginEffectiveDate, $scope.scopeModel.endEffectiveDate);
            };
            $scope.scopeModel.save = function () {
                if (!isEditMode) {
                    addAccountManagerAssignment();
                }
                else {
                    updateAccountManagerAssignment();
                }
            };
            $scope.scopeModel.onAccountSelectorReady = function (api) {
                accountSelectorAPI = api;
                accountSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onRuntimeDirectiveReady = function (api) {
                runtimeDirectiveAPI = api;
                runtimeDirectiveReadyDeferred.resolve();
            };
            $scope.scopeModel.onAccountManagerSelectorReady = function (api) {
                accountManagerSelectorAPI = api;
                accountManagerReadyDeferred.resolve();
            };
        }
        function load() {
            if (isEditMode || accountId != undefined) {
                $scope.scopeModel.accountSelectorDisable = true;
                $scope.scopeModel.accountSelectorLabel = "Account";
            }
            else
                $scope.scopeModel.accountSelectorLabel = "Accounts";
            $scope.scopeModel.isLoading = true;
            if (accountBEDefinitionId != undefined) {
                Retail_BE_AccountManagerAssignmentAPIService.GetAccountManagerDefInfo(accountBEDefinitionId).then(function (response) {
                    if (response != undefined) {
                        accountManagerDefinitionId = response.AccountManagerDefinitionId;
                        if (response.AccountManagerAssignmentDefinition != undefined)
                        accountManagerAssignementDefinitionId = response.AccountManagerAssignmentDefinition.AccountManagerAssignementDefinitionId;
                    }
                        getAccountManagerAssignmentRuntimeEditor().then(function () {
                            loadAllControls();
                        }).catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        }).finally(function () {
                            $scope.scopeModel.isLoading = false;
                        });
                    });
            }
            else {
                getAccountManagerAssignmentRuntimeEditor().then(function () {
                   
                    loadAllControls();
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
            }
        }
        function loadAllControls() {
            function setTitle() {
                if (!isEditMode)
                    $scope.title = UtilsService.buildTitleForAddEditor('Account Manager Assignment');
                else
                    $scope.title = UtilsService.buildTitleForUpdateEditor(accountManagerAssignmentRuntime.AccountName, 'Account Manager Assignment');
            }
            function loadRuntimeDirective() {
                var runtimeDirectiveLoadDeferred = UtilsService.createPromiseDeferred();
                runtimeDirectiveReadyDeferred.promise.then(function () {
                    var directivePayload;
                    VRUIUtilsService.callDirectiveLoad(runtimeDirectiveAPI, directivePayload, runtimeDirectiveLoadDeferred);
                });
                return runtimeDirectiveLoadDeferred.promise;
            }
            function loadAccountManagerSelector() {
                var accountManagerSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                accountManagerReadyDeferred.promise.then(function () {
                  
                    if (accountManagerId != undefined) {
                        var payload = {
                            selectedIds: accountManagerId
                        };

                        $scope.scopeModel.accountManagerDisable = true;
                    }
                    VRUIUtilsService.callDirectiveLoad(accountManagerSelectorAPI, payload, accountManagerSelectorLoadDeferred);
                });
                return accountManagerSelectorLoadDeferred.promise;
            }
            function loadAccountSelector() {
                var accountSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                accountSelectorReadyDeferred.promise.then(function () {
                    var editedAccountId;
                        var payload = {
                            AccountBEDefinitionId: accountManagerAssignmentRuntime.AccountManagrAssignmentDefinition.Settings.AccountBEDefinitionId
                        };
                        if (accountManagerAssignmentRuntime != undefined && accountManagerAssignmentRuntime.AccountManagerAssignment != undefined) {
                            payload.selectedIds = [accountManagerAssignmentRuntime.AccountManagerAssignment.AccountId];
                            editedAccountId = accountManagerAssignmentRuntime.AccountManagerAssignment.AccountId;
                        }
                        if (accountId != undefined) {
                            payload.selectedIds = [accountId];
                        }
                      
                            payload.filter = {};
                            payload.filter.Filters = [];
                            payload.filter.Filters.push({
                                $type: "Retail.BusinessEntity.Business.AssignedAccountToAccountManagerFilter,Retail.BusinessEntity.Business",
                                EditedAccountId: editedAccountId
                            });

                        VRUIUtilsService.callDirectiveLoad(accountSelectorAPI, payload, accountSelectorLoadDeferred);
                    });

                return accountSelectorLoadDeferred.promise;
            }
            function loadStaticData() {
                if (accountManagerAssignmentRuntime != undefined)
                var accountManagerAssignment = accountManagerAssignmentRuntime.AccountManagerAssignment;
                if (accountManagerAssignment != undefined) {

                    $scope.scopeModel.beginEffectiveDate = accountManagerAssignment.BED;
                    $scope.scopeModel.endEffectiveDate = accountManagerAssignment.EED;
                }
            }
            return UtilsService.waitMultipleAsyncOperations([loadStaticData, loadAccountSelector, setTitle, loadRuntimeDirective, loadAccountManagerSelector]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function buildObjectFromScope() {
            if (!isEditMode) {
                var accounts = [];
                var accountIds = accountSelectorAPI.getSelectedIds();
                for (var i = 0; i < accountIds.length; i++) {
                    var account = {
                        AccountId: accountIds[i],
                        AssignementSettings: {
                            ExtendedSettings: runtimeDirectiveAPI.getData()
                        }
                    };
                    accounts.push(account);
                }
                if (accountManagerId == undefined) {
                    accountManagerId = accountManagerSelectorAPI.getSelectedIds();
                }
                var accountmanagerAssignment = {
                    AccountManagerAssignementDefinitionId: accountManagerAssignementDefinitionId,
                    AccountManagerId: accountManagerId,
                    Accounts: accounts,
                    BED: $scope.scopeModel.beginEffectiveDate,
                    EED: $scope.scopeModel.endEffectiveDate,
                    AccountManagerDefinitionId: accountManagerDefinitionId
                };
            }
            else {
               
                var accountmanagerAssignment = {
                    AccountManagerAssignmentDefinitionId: accountManagerAssignementDefinitionId,
                    AccountManagerAssignmentId: accountManagerAssignmentId,
                    BED: $scope.scopeModel.beginEffectiveDate,
                    EED: $scope.scopeModel.endEffectiveDate,
                    AssignementSettings: {
                        ExtendedSettings: runtimeDirectiveAPI.getData()
                    },
                    AccountManagerDefinitionId: accountManagerDefinitionId
                };
            }
            return accountmanagerAssignment;
        }
        function getAccountManagerAssignmentRuntimeEditor() {
            var accountManagerRuntimeInput = {
                AccountManagerAssignementId: accountManagerAssignmentId,
                AccountManagerDefinitionId: accountManagerDefinitionId,
                AssignmentDefinitionId: accountManagerAssignementDefinitionId
            };
            return Retail_BE_AccountManagerAssignmentAPIService.GetAccountManagerAssignmentRuntimeEditor(accountManagerRuntimeInput).then(function (response) {
                accountManagerAssignmentRuntime = response;
                if (accountManagerAssignmentRuntime != undefined && accountManagerAssignmentRuntime.AccountManagerAssignment != undefined)
                    accountManagerId = accountManagerAssignmentRuntime.AccountManagerAssignment.AccountManagerId;
                if (accountManagerAssignmentRuntime != undefined && accountManagerAssignmentRuntime.AccountManagrAssignmentDefinition != undefined && accountManagerAssignmentRuntime.AccountManagrAssignmentDefinition.Settings != undefined) {
                    $scope.scopeModel.assignmentRuntime = accountManagerAssignmentRuntime.AccountManagrAssignmentDefinition.Settings.RuntimeEditor;
                }
            });
        }
        function addAccountManagerAssignment() {
          
            $scope.scopeModel.isLoading = true;
            var accountmanagerAssignment = buildObjectFromScope();
            return Retail_BE_AccountManagerAssignmentAPIService.AddAccountManagerAssignment(accountmanagerAssignment).then(function (response) {
                if (response.Result == InsertOperationResultEnum.Succeeded.value) {
                    if (VRNotificationService.notifyOnItemAdded("Account Manager Assignment", response)) {
                        $scope.modalContext.closeModal();
                    }
                }
                else {
                    $scope.scopeModel.errorMessage = "* " + response.Message;
                    VRNotificationService.showError("An error has occured");
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function updateAccountManagerAssignment() {
            $scope.scopeModel.isLoading = true;
            var accountmanagerAssignment = buildObjectFromScope();
            return Retail_BE_AccountManagerAssignmentAPIService.UpdateAccountManagerAssignment(accountmanagerAssignment).then(function (response) {
                if (response.Result == UpdateOperationResultEnum.Succeeded.value) {
                    if (VRNotificationService.notifyOnItemUpdated("Account Manager Assignment", response)) {

                        if ($scope.onAccountManagerAssignmentUpdated != undefined)
                            $scope.onAccountManagerAssignmentUpdated(response.UpdatedObject);
                        $scope.modalContext.closeModal();
                    }
                }
                else {
                    $scope.scopeModel.errorMessage = "* " + response.Message;
                    VRNotificationService.showError("An error has occured");
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
      
    }
    appControllers.controller("Retail_BE_AccountManagerAssignmentEditorController", accountManagerAssignmentEditor);
})(appControllers);
