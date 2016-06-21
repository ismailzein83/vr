(function (appControllers) {

    'use strict';

    AccountTypeEditorController.$inject = ['$scope', 'Retail_BE_AccountTypeAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService','Retail_BE_AccountPartAvailabilityOptionsEnum','Retail_BE_AccountPartRequiredOptionsEnum'];

    function AccountTypeEditorController($scope, Retail_BE_AccountTypeAPIService, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, Retail_BE_AccountPartAvailabilityOptionsEnum, Retail_BE_AccountPartRequiredOptionsEnum) {
        var isEditMode;

        var accountTypeId;
        var accountTypeEntity;

        var accountTypeSelectorAPI;
        var accountTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var accountTypeSelectedDeferred;

        var partDefinitionSelectorAPI;
        var partDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                accountTypeId = parameters.accountTypeId;
            }
            isEditMode = (accountTypeId != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.accountPartAvailability = UtilsService.getArrayEnum(Retail_BE_AccountPartAvailabilityOptionsEnum);

            $scope.scopeModel.accountPartRequiredOptions = UtilsService.getArrayEnum(Retail_BE_AccountPartRequiredOptionsEnum);


            $scope.scopeModel.onAccountTypeSelectorReady = function (api) {
                accountTypeSelectorAPI = api;
                accountTypeSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onAccountTypePartDefinitionReady = function (api) {
                accountTypePartDefinitionAPI = api;
                accountTypePartDefinitionReadyDeferred.resolve();
            };

            $scope.scopeModel.onPartDefinitionSelectorReady = function (api) {
                partDefinitionSelectorAPI = api;
                partDefinitionSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateAccountType() : insertAccountType();
            };

            $scope.scopeModel.hasSaveAccountTypePermission = function () {
                return (isEditMode) ? Retail_BE_AccountTypeAPIService.HasUpdateAccountTypePermission() : Retail_BE_AccountTypeAPIService.HasAddAccountTypePermission();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };
        }
        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                getAccountType().then(function () {
                    loadAllControls().finally(function () {
                        accountTypeEntity = undefined;
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

        function getAccountType() {
            return Retail_BE_AccountTypeAPIService.GetAccountType(accountTypeId).then(function (response) {
                accountTypeEntity = response;
            });
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadAccountTypeSection, loadStaticData, loadPartDefinitionSelector]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function setTitle() {
            if (isEditMode) {
                var accountTypeName = (accountTypeEntity != undefined) ? accountTypeEntity.Name : undefined;
                $scope.title = UtilsService.buildTitleForUpdateEditor(accountTypeName, 'Account Type');
            }
            else {
                $scope.title = UtilsService.buildTitleForAddEditor('Account Type');
            }
        }
        function loadStaticData() {
            if (accountTypeEntity == undefined)
                return;
            $scope.scopeModel.name = accountTypeEntity.Name;
            $scope.scopeModel.title = accountTypeEntity.Title;

            if (accountTypeEntity.Settings != undefined)
            {
                $scope.scopeModel.canBeRootAccount = accountTypeEntity.Settings.CanBeRootAccount;
                if (accountTypeEntity.Settings.AccountTypePartSettings != undefined) {
                    $scope.scopeModel.selectedAccountPartAvailability = UtilsService.getItemByVal($scope.scopeModel.accountPartAvailability, accountTypeEntity.Settings.AccountTypePartSettings.AvailabilitySettings, "value");
                    $scope.scopeModel.selectedAccountPartRequiredOptions = UtilsService.getItemByVal($scope.scopeModel.accountPartRequiredOptions, accountTypeEntity.Settings.AccountTypePartSettings.RequiredSettings, "value");
                }
            }
           
        }
        function loadAccountTypeSection() {
            var accountTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            accountTypeSelectorReadyDeferred.promise.then(function () {
                var accountTypeSelectorPayload;
                if (accountTypeEntity != undefined && accountTypeEntity.Settings != null) {
                    accountTypeSelectorPayload = {
                        selectedIds: accountTypeEntity.Settings.SupportedParentAccountTypeIds
                    };
                }
                VRUIUtilsService.callDirectiveLoad(accountTypeSelectorAPI, accountTypeSelectorPayload, accountTypeSelectorLoadDeferred);
            });

            return accountTypeSelectorLoadDeferred.promise;
        }
        function loadPartDefinitionSelector()
        {
            var partDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            partDefinitionSelectorReadyDeferred.promise.then(function () {
                var partDefinitionSelectorPayload = (accountTypeEntity != undefined && accountTypeEntity.Settings != null && accountTypeEntity.Settings.AccountTypePartSettings !=undefined) ? {
                    selectedIds: accountTypeEntity.Settings.AccountTypePartSettings.PartDefinitionIds
                } : undefined;
                VRUIUtilsService.callDirectiveLoad(partDefinitionSelectorAPI, partDefinitionSelectorPayload, partDefinitionSelectorLoadDeferred);
            });

            return partDefinitionSelectorLoadDeferred.promise;
        }

        function insertAccountType() {
            $scope.scopeModel.isLoading = true;

            var accountTypeObj = buildAccountTypeObjFromScope();

            return Retail_BE_AccountTypeAPIService.AddAccountType(accountTypeObj).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('Account Type', response, 'Name')) {
                    if ($scope.onAccountTypeAdded != undefined)
                        $scope.onAccountTypeAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function updateAccountType() {
            $scope.scopeModel.isLoading = true;

            var accountTypeObj = buildAccountTypeObjFromScope();

            return Retail_BE_AccountTypeAPIService.UpdateAccountType(accountTypeObj).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('Account Type', response, 'Name')) {
                    if ($scope.onAccountTypeUpdated != undefined) {
                        $scope.onAccountTypeUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function buildAccountTypeObjFromScope() {
            var obj = {
                AccountTypeId: accountTypeId,
                Name: $scope.scopeModel.name,
                Title: $scope.scopeModel.title,
                Settings:{
                    CanBeRootAccount: $scope.scopeModel.canBeRootAccount,
                    SupportedParentAccountTypeIds: accountTypeSelectorAPI.getSelectedIds(),
                    AccountTypePartSettings: {
                        PartDefinitionIds: partDefinitionSelectorAPI.getSelectedIds(),
                        AvailabilitySettings: $scope.scopeModel.selectedAccountPartAvailability.value,
                        RequiredSettings: $scope.scopeModel.selectedAccountPartRequiredOptions.value,
                    }
                }
               
            };
            return obj;
        }
    }

    appControllers.controller('Retail_BE_AccountTypeEditorController', AccountTypeEditorController);

})(appControllers);