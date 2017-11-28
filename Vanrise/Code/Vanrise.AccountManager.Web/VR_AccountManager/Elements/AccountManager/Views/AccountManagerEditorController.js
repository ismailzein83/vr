(function (appControllers) {

    "use strict";

    accountManagerEditorController.$inject = ["$scope", "UtilsService", "VRNotificationService", "VRNavigationService", "VRUIUtilsService", "VR_AccountManager_AccountManagerAPIService", "VR_AccountManager_AccountManagerDefinitionAPIService"];

    function accountManagerEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, VR_AccountManager_AccountManagerAPIService, VR_AccountManager_AccountManagerDefinitionAPIService) {
        var accountDefinitionId;
        var isEditMode;
        var accountManagerId;
        var userSelectorAPI;
        var accountManagerDefinitionId;
        var accountManagerDefintionEntity;
        var accountManagerEntity;
        var userSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var runtimeAPI;
        var runtimeReadyPromise = UtilsService.createPromiseDeferred();
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                accountManagerId = parameters.accountManagerId;
                accountManagerDefinitionId = parameters.accountManagerDefinitionId;
            };
            isEditMode = (accountManagerId != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
            $scope.scopeModel.save = function () {
                if (!isEditMode) {
                    addAccountManager();
                }
                else {
                    updateAccountManager();
                }
            };
            $scope.scopeModel.onUserSelectorReady = function (api) {
                userSelectorAPI = api;
                userSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onDirectiveReady = function (api) {
                runtimeAPI = api;
                runtimeReadyPromise.resolve();
            };
        }
        function load() {
            $scope.scopeModel.isLoading = true;
            getAccountManagerDefinitionId().then(function () {
                if (isEditMode) {
                    getAccountManager().then(function () {
                        loadAllControls();
                    });
                }
                else
                    loadAllControls();
            });
        }

        function loadAllControls() {
            function setTitle() {
                if (!isEditMode)
                    $scope.title = UtilsService.buildTitleForAddEditor('Account Manager');
                else
                    $scope.title = UtilsService.buildTitleForUpdateEditor('Account Manager');

            }
            function loadRuntimeDirective() {
                var directiveLoaddDeferred = UtilsService.createPromiseDeferred();
                if ($scope.scopeModel.runtimeEditor != undefined) {
                    runtimeReadyPromise.promise.then(function () {
                        var payload;
                        VRUIUtilsService.callDirectiveLoad(runtimeAPI, payload, directiveLoaddDeferred);
                    });
                }
                else
                    directiveLoaddDeferred.resolve();
                return directiveLoaddDeferred.promise;
            }
            function loadUserSelector() {
                var editedUserId;
                var payload;
                var userSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                userSelectorReadyDeferred.promise.then(function () {
                    if (accountManagerEntity != undefined) {
                        payload = {
                            selectedIds: accountManagerEntity.UserId
                        };
                        editedUserId = accountManagerEntity.UserId;
                    }
                    if (payload == undefined)
                        payload = {};
                    payload.filter = {};
                    payload.filter.Filters = [];
                    payload.filter.Filters.push({
                        $type: "Vanrise.AccountManager.Business.AssignedUsersToAccountManagerFilter,Vanrise.AccountManager.Business",
                        EditedUserId: editedUserId
                    });
                    console.log(payload);
                    VRUIUtilsService.callDirectiveLoad(userSelectorAPI, payload, userSelectorLoadDeferred);
                });
                return userSelectorLoadDeferred.promise;
            }
            return UtilsService.waitMultipleAsyncOperations([loadUserSelector, setTitle, loadRuntimeDirective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function addAccountManager() {
            var accountManager = buildObjectFromScope();
            VR_AccountManager_AccountManagerAPIService.AddAccountManager(accountManager).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Account Manager", response)) {
                    if ($scope.onAccountManagerAdded != undefined)
                        $scope.onAccountManagerAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                };
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });

        }
        function updateAccountManager() {
            var accountManager = buildObjectFromScope();
            VR_AccountManager_AccountManagerAPIService.UpdateAccountManager(accountManager).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Account Manager", response)) {
                    if ($scope.onAccountManagerUpdated != undefined)
                        $scope.onAccountManagerUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                };
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });

        }
        function buildObjectFromScope() {
            var accountManagerObject = {
                UserId: userSelectorAPI.getSelectedIds(),
                AccountManagerDefinitionId: accountManagerDefinitionId
            };
            if (isEditMode) {
                accountManagerObject.AccountManagerId = accountManagerId
            };
            return accountManagerObject;
        }
       
        function getAccountManager() {
            return VR_AccountManager_AccountManagerAPIService.GetAccountManager(accountManagerId).then(function (response) {
                accountManagerEntity = response;
            });
        }
      
        function getAccountManagerDefinitionId() {
            return VR_AccountManager_AccountManagerDefinitionAPIService.GetAccountManagerDefinition(accountManagerDefinitionId).then(function (response) {
                accountManagerDefintionEntity = response;
                var extendedSettings = accountManagerDefintionEntity.Settings.ExtendedSettings;
                if (extendedSettings != null) {
                    if (extendedSettings.RuntimeEditor != undefined)
                        $scope.scopeModel.runtimeEditor = accountManagerDefintionEntity.Settings.ExtendedSettings.RuntimeEditor;
                }
            });
        }
    }
    appControllers.controller("VR_AccountManager_AccountManagerEditorController", accountManagerEditorController);
})(appControllers);
