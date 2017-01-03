(function (appControllers) {

    'use strict';

    DynamicAccountEditorController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService', 'Retail_BE_AccountBEAPIService', 'Retail_BE_AccountTypeAPIService', 'Retail_BE_AccountPartDefinitionAPIService'];

    function DynamicAccountEditorController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, Retail_BE_AccountBEAPIService, Retail_BE_AccountTypeAPIService, Retail_BE_AccountPartDefinitionAPIService) {

        var isEditMode;
        var accountBEDefinitionId;
        var accountId;
        var accountEntity;
        var parentAccountId;

        var accountEditorDirectiveAPI;
        var accountEditorDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                accountBEDefinitionId = parameters.accountBEDefinitionId
                accountId = parameters.accountId;
                parentAccountId = parameters.parentAccountId;
            }

            isEditMode = (accountId != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onAccountEditorDirectiveReady = function (api) {
                accountEditorDirectiveAPI = api;
                accountEditorDirectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateAccount() : insertAccount();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };
        }
        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadAccountEditorDirective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function setTitle() {
            var title;
            if (isEditMode) {
                var accountName = (accountEntity != undefined) ? accountEntity.Name : undefined;
                title = UtilsService.buildTitleForUpdateEditor(accountName, 'Account');
            }
            else {
                title = UtilsService.buildTitleForAddEditor('Account');
            }

            if (parentAccountId != undefined) {
                return Retail_BE_AccountBEAPIService.GetAccountName(accountBEDefinitionId, parentAccountId).then(function (response) {
                    var titleExtension = ' for ' + response;
                    $scope.title = title += titleExtension;
                });
            }
            else {
                $scope.title = title;
            }
        }
        function loadAccountEditorDirective() {
            var accountEditorLoadDeferred = UtilsService.createPromiseDeferred();

            accountEditorDirectiveReadyDeferred.promise.then(function () {

                var accountEditorDirectivePayload = {
                    accountBEDefinitionId: accountBEDefinitionId,
                    accountId: accountId,
                    parentAccountId: parentAccountId,
                    onAccountLoaded: buildOnAccountLoaded()
                };
                VRUIUtilsService.callDirectiveLoad(accountEditorDirectiveAPI, accountEditorDirectivePayload, accountEditorLoadDeferred);
            });

            return accountEditorLoadDeferred.promise;
        }

        function insertAccount() {
            $scope.scopeModel.isLoading = true;

            var accountObj = buildAccountObjFromScope();

            return Retail_BE_AccountBEAPIService.AddAccount(accountObj).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('Account', response, 'Name')) {
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
        function updateAccount() {
            $scope.scopeModel.isLoading = true;

            var accountObj = buildAccountObjFromScope();

            return Retail_BE_AccountBEAPIService.UpdateAccount(accountObj).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('Account', response, 'Name')) {
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

        function buildAccountObjFromScope() {
            return accountEditorDirectiveAPI.getData();
        }
        function buildOnAccountLoaded() {
            var onAccountLoaded = function (accountEntityFromAccountEditor) {
                accountEntity = accountEntityFromAccountEditor;
                setTitle();
            }
            return onAccountLoaded;
        }
    }

    appControllers.controller('Retail_BE_DynamicAccountEditorController', DynamicAccountEditorController);

})(appControllers);