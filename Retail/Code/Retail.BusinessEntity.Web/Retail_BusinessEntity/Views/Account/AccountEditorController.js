(function (appControllers) {

    'use strict';

    AccountEditorController.$inject = ['$scope', 'Retail_BE_AccountAPIService', 'Retail_BE_AccountTypeAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function AccountEditorController($scope, Retail_BE_AccountAPIService, Retail_BE_AccountTypeAPIService, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService)
    {
        var isEditMode;

        var accountId;
        var accountEntity;
        var parentAccountId;

        var accountTypeSelectorAPI;
        var accountTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters()
        {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined)
            {
                accountId = parameters.accountId;
                parentAccountId = parameters.parentAccountId;
            }

            isEditMode = (accountId != undefined);
        }
        function defineScope()
        {
            $scope.scopeModel = {};

            $scope.scopeModel.onAccountTypeSelectorReady = function (api) {
                accountTypeSelectorAPI = api;
                accountTypeSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.save = function ()
            {
                return (isEditMode) ? updateAccount() : insertAccount();
            };

            $scope.scopeModel.hasSaveAccountPermission = function ()
            {
                return (isEditMode) ? Retail_BE_AccountAPIService.HasUpdateAccountPermission() : Retail_BE_AccountAPIService.HasAddAccountPermission();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };
        }
        function load()
        {
            $scope.scopeModel.isLoading = true;

            if (isEditMode)
            {
                getAccount().then(function () {
                    loadAllControls().finally(function () {
                        accountEntity = undefined;
                    });
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }
        }

        function getAccount() {
            return Retail_BE_AccountAPIService.GetAccount(accountId).then(function (response) {
                accountEntity = response;
            });
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadAccountTypeSelector]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                   $scope.scopeModel.isLoading = false;
              });
        }
        function setTitle()
        {
            var title;
            if (isEditMode) {
                var accountName = (accountEntity != undefined) ? accountEntity.Name : undefined;
                title = UtilsService.buildTitleForUpdateEditor(accountName, 'Account');
            }
            else {
                title = UtilsService.buildTitleForAddEditor('Account');
            }

            if (parentAccountId != undefined) {
                return Retail_BE_AccountAPIService.GetAccountName(parentAccountId).then(function (response) {
                    var titleExtension = ' for ' + response;
                    $scope.title = title += titleExtension;
                });
            }
            else {
                $scope.title = title;
            }
        }
        function loadStaticData()
        {
            if (accountEntity == undefined)
                return;
            $scope.scopeModel.name = accountEntity.Name;
        }
        function loadAccountTypeSelector() {
            var accountTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            accountTypeSelectorReadyDeferred.promise.then(function () {
                var accountTypeSelectorPayload = {
                    selectedIds: (accountEntity != undefined) ? accountEntity.TypeId : undefined,
                    filter: { CanBeRootAccount: (parentAccountId == undefined) }
                };
                VRUIUtilsService.callDirectiveLoad(accountTypeSelectorAPI, accountTypeSelectorPayload, accountTypeSelectorLoadDeferred);
            });

            return accountTypeSelectorLoadDeferred.promise;
        }

        function insertAccount()
        {
            $scope.scopeModel.isLoading = true;

            var accountObj = buildAccountObjFromScope();

            return Retail_BE_AccountAPIService.AddAccount(accountObj).then(function (response)
            {
                if (VRNotificationService.notifyOnItemAdded('Account', response, 'Name'))
                {
                    if ($scope.onAccountAdded != undefined)
                        $scope.onAccountAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function updateAccount()
        {
            $scope.scopeModel.isLoading = true;

            var accountObj = buildAccountObjFromScope();

            return Retail_BE_AccountAPIService.UpdateAccount(accountObj).then(function (response)
            {
                if (VRNotificationService.notifyOnItemUpdated('Account', response, 'Name'))
                {
                    if ($scope.onAccountUpdated != undefined) {
                        $scope.onAccountUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function buildAccountObjFromScope()
        {
            var obj = {
                AccountId: accountId,
                Name: $scope.scopeModel.name,
                TypeId: accountTypeSelectorAPI.getSelectedIds()
            };

            obj.Settings = {};
            obj.Settings.Parts = {}; // Call getData

            if (!isEditMode) {
                obj.ParentAccountId = parentAccountId;
            }

            return obj;
        }
    }

    appControllers.controller('Retail_BE_AccountEditorController', AccountEditorController);

})(appControllers);