(function (appControllers) {

    "use strict";

    accountManagerAssignmentEditor.$inject = ["$scope", "UtilsService", "VRNotificationService", "VRNavigationService", "VRUIUtilsService",  "Retail_BE_AccountManagerAssignmentAPIService"];

    function accountManagerAssignmentEditor($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, Retail_BE_AccountManagerAssignmentAPIService) {
        var isEditMode;
        var subViewDefinitionEntity

        var accountManagerDefinitionId;
        var accountManagerAssignementDefinitionId;
        var accountManagerId;

        var accountManagerAssignmentRuntime;

        var accountManagerAssignmentId;

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
                accountManagerId = parameters.accountManagerId;
                accountManagerAssignmentId = parameters.accountManagerAssignmentId;
            };
            isEditMode = (accountManagerAssignmentId != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
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
            }
        }
        function load() {
            if (isEditMode)
                 $scope.scopeModel.disable = true;
            console.log($scope.scopeModel.disabel);
            $scope.scopeModel.isLoading = true;
                getAccountManagerAssignmentRuntimeEditor().then(function () {
                    loadAllControls();
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
        }
        function loadAllControls() {
            function setTitle() {
                if (!isEditMode)
                    $scope.title = UtilsService.buildTitleForAddEditor('Account Manager');
                else
                    $scope.title = UtilsService.buildTitleForUpdateEditor('Account Manager');
            }
            function loadAccountSelector() {
                var accountSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                accountSelectorReadyDeferred.promise.then(function () {
                        var payload = {
                            AccountBEDefinitionId: accountManagerAssignmentRuntime.AccountManagrAssignmentDefinition.Settings.AccountBEDefinitionId
                        };
                        if (accountManagerAssignmentRuntime != undefined && accountManagerAssignmentRuntime.AccountManagerAssignment != undefined) {
                            var selectedIds = [];
                            selectedIds.push(accountManagerAssignmentRuntime.AccountManagerAssignment.AccountId);
                            payload.selectedIds = selectedIds;
                        }
                        VRUIUtilsService.callDirectiveLoad(accountSelectorAPI, payload, accountSelectorLoadDeferred);
                    });

                return accountSelectorLoadDeferred.promise;
            }
            function loadStaticData() {
                if (accountManagerAssignmentRuntime != undefined)
                var accountManagerAssignment = accountManagerAssignmentRuntime.AccountManagerAssignment;
                if (accountManagerAssignment != undefined) {

                    $scope.scopeModel.bed = accountManagerAssignment.BED;
                    $scope.scopeModel.eed = accountManagerAssignment.EED;
                }
            }
            return UtilsService.waitMultipleAsyncOperations([loadStaticData,loadAccountSelector, setTitle ]).catch(function (error) {
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
                        AssignementSettings: {}
                    }
                    accounts.push(account);
                }
                var accountmanagerAssignment = {
                    AccountManagerAssignementDefinitionId: accountManagerAssignementDefinitionId,
                    AccountManagerId: accountManagerId,
                    Accounts: accounts,
                    BED: $scope.scopeModel.bed,
                    EED: $scope.scopeModel.eed
                }
            }
            else {
                var accountmanagerAssignment = {
                    AccountManagerAssignmentId: accountManagerAssignmentId,
                    BED: $scope.scopeModel.bed,
                    EED: $scope.scopeModel.eed,
                    AssignementSettings: {}
                }
            }
            console.log(accountmanagerAssignment)
            return accountmanagerAssignment;
        }
        function getAccountManagerAssignmentRuntimeEditor() {
            var accountManagerRuntimeInput = {
                AccountManagerAssignementId: accountManagerAssignmentId,
                AccountManagerDefinitionId: accountManagerDefinitionId,
                AssignmentDefinitionId: accountManagerAssignementDefinitionId
            }
            return Retail_BE_AccountManagerAssignmentAPIService.GetAccountManagerAssignmentRuntimeEditor(accountManagerRuntimeInput).then(function (response) {
                accountManagerAssignmentRuntime = response;
            });
        }
        function addAccountManagerAssignment() {
          
            $scope.scopeModel.isLoading = true;
            var accountmanagerAssignment = buildObjectFromScope();
            return Retail_BE_AccountManagerAssignmentAPIService.AddAccountManagerAssignment(accountmanagerAssignment).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Account Manager Assignment", response)) {
                    if ($scope.onAccountManagerAssignmentAdded != undefined)
                    $scope.modalContext.closeModal();
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
                if (VRNotificationService.notifyOnItemUpdated("Account Manager Assignment", response)) {
                    if ($scope.onAccountManagerAssignmentUpdated != undefined)
                    $scope.modalContext.closeModal();
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
