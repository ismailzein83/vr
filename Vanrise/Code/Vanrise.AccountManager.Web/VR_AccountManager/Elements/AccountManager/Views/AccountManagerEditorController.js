(function (appControllers) {

    "use strict";

    accountManagerEditorController.$inject = ["$scope", "UtilsService", "VRNotificationService", "VRNavigationService", "VRUIUtilsService", "VR_AccountManager_AccountManagerAPIService"];

    function accountManagerEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, VR_AccountManager_AccountManagerAPIService) {
        var accountDefinitionId;
        var isEditMode;
        var userId;
        var userName;
        var accountManagerId;
        var userSelectorAPI;
        var accountManagerDefinitionId;
        var userSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                userId = parameters.UserID;
                userName = parameters.UserName;
                accountManagerId = parameters.AccountManagerId;
                accountManagerDefinitionId = parameters.AccountManagerDefinitionId;
            };
            isEditMode = (userId != undefined);
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
            }
        }
        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            function setTitle() {
                if (!isEditMode)
                    $scope.title = UtilsService.buildTitleForAddEditor('Account Manager');
                else
                    $scope.title = UtilsService.buildTitleForUpdateEditor(userName, 'Account Manager');

            }
            return UtilsService.waitMultipleAsyncOperations([loadUserSelector, setTitle]).catch(function (error) {
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
            if (!isEditMode) {
                var accountManagerObject = {
                    UserId: userSelectorAPI.getSelectedIds(),
                    AccountManagerDefinitionId: accountManagerDefinitionId

                };
            }
            else {
                var accountManagerObject = {
                    AccountManagerId: accountManagerId,
                    UserId: userSelectorAPI.getSelectedIds(),
                    AccountManagerDefinitionId: accountManagerDefinitionId
                }
            }
            return accountManagerObject;
        }
        function loadUserSelector() {
            var userSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            userSelectorReadyDeferred.promise.then(function () {
                var payload = (userName != undefined) ? { selectedIds: userId } : undefined;
                VRUIUtilsService.callDirectiveLoad(userSelectorAPI, payload, userSelectorLoadDeferred);
            });
            return userSelectorLoadDeferred.promise;
        }
    }
    appControllers.controller("VR_AccountManager_AccountManagerEditorController", accountManagerEditorController);
})(appControllers);
