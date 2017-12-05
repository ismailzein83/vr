(function (appControllers) {
    "use strict";
    accountManagerManagementController.$inject = ["$scope", "UtilsService", "VRUIUtilsService", "VRNotificationService", "VR_AccountManager_AccountManagerService", "VR_AccountManager_AccountManagerAPIService"];

    function accountManagerManagementController($scope, UtilsService, VRUIUtilsService, VRNotificationService, VR_AccountManager_AccountManagerService,VR_AccountManager_AccountManagerAPIService) {
        var gridAPI;
        var userSelectorAPI;
        var userSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var genericAccountManagerDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
        var genericAccountmanagerDefinitionSelectorApi;
        var gridReadyPromise = UtilsService.createPromiseDeferred();
        var accountManagerDefinitionId;
        var selectorChangePromise = UtilsService.createPromiseDeferred();
        loadParameters();
        defineScope();
        load();
        function loadParameters() {

        };
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridReadyPromise.resolve();
            };
            $scope.scopeModel.onUserSelectorReady = function (api) {
                userSelectorAPI = api;
                userSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.searchClicked = function () {
                gridAPI.loadGrid(getGridFilter());
            };
            $scope.scopeModel.onAccountManagerDefinitionSelectorReady = function (api) {
                genericAccountmanagerDefinitionSelectorApi = api;
                genericAccountManagerDefinitionSelectorPromiseDeferred.resolve();
            };
            $scope.scopeModel.onAccountManagerDefinitionSelectorSelectionChange = function (value) {
                if (value != undefined) {
                    if (selectorChangePromise != undefined) {
                        accountManagerDefinitionId = genericAccountmanagerDefinitionSelectorApi.getSelectedIds();
                        selectorChangePromise.resolve();
                    }
                    else {
                        accountManagerDefinitionId = genericAccountmanagerDefinitionSelectorApi.getSelectedIds();
                        loadAllControls();
                    }
                }
                if (accountManagerDefinitionId != undefined) {
                    VR_AccountManager_AccountManagerAPIService.DoesUserHaveAddAccess(accountManagerDefinitionId).then(function (response) {
                        $scope.scopeModel.showAddAccountManager = response;
                    });
                }
              
            };
            $scope.scopeModel.addNewAccountManager = function () {
                var onAccountManagerAdded = function (addedItem) {
                    gridAPI.onAccountManagerAdded(addedItem); 
                };
                VR_AccountManager_AccountManagerService.addAccountManager(onAccountManagerAdded, accountManagerDefinitionId);
            };
        };
        function load() {
            $scope.scopeModel.isLoading = true;
            loadGenericAccountManagerDefinitionSelector().then(function () {
                accountManagerDefinitionId = genericAccountmanagerDefinitionSelectorApi.getSelectedIds();
                $scope.scopeModel.hideAccountDefinition = genericAccountmanagerDefinitionSelectorApi.hasSingleItem();

                loadAllControls();
            });
           
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadUserSelector]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).then(function () {
                gridReadyPromise.promise.then(function () {
                    gridAPI.loadGrid(getGridFilter());
                    selectorChangePromise = undefined;
                });
            });
          
        }
        function getGridFilter() {
            var gridPayload = {
                query: {
                    UserIds: userSelectorAPI.getSelectedIds(),
                    AccountManagerDefinitionId: accountManagerDefinitionId
                },
                accountManagerDefinitionId: accountManagerDefinitionId
            };
            return gridPayload;
        }
        function loadUserSelector() {
            var userSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            userSelectorReadyDeferred.promise.then(function () {
                var payload;
                VRUIUtilsService.callDirectiveLoad(userSelectorAPI, payload, userSelectorLoadDeferred);
            });
            return userSelectorLoadDeferred.promise;
        }
        function loadGenericAccountManagerDefinitionSelector() {
            var genericAccountManagerDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            genericAccountManagerDefinitionSelectorPromiseDeferred.promise.then(function () {
                var payloadSelector = {
                    selectFirstItem:true
                };
                VRUIUtilsService.callDirectiveLoad(genericAccountmanagerDefinitionSelectorApi, payloadSelector, genericAccountManagerDefinitionSelectorLoadDeferred);
            });
            return genericAccountManagerDefinitionSelectorLoadDeferred.promise;
        }
    }
    appControllers.controller("VR_AccountManager_AccountManagerManagementController", accountManagerManagementController);
})(appControllers);