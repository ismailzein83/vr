(function (appControllers) {

    'use strict';

    AccountEditorController.$inject = ['$scope', 'Retail_BE_AccountAPIService', 'Retail_BE_AccountTypeAPIService', 'Retail_BE_AccountPartDefinitionAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function AccountEditorController($scope, Retail_BE_AccountAPIService, Retail_BE_AccountTypeAPIService, Retail_BE_AccountPartDefinitionAPIService, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {
        var isEditMode;
        var accountId;
        var accountEntity;
        var parentAccountId;

        var accountTypeSelectorAPI;
        var accountTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var accountTypeSelectedDeferred;

        var accountEditorRuntime;
        var requiredPartsDirectiveAPI;
        var requiredPartsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        var partDefinitionExtensionConfigs;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                accountId = parameters.accountId;
                parentAccountId = parameters.parentAccountId;
            }

            isEditMode = (accountId != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.notRequiredParts = [];

            $scope.scopeModel.onAccountTypeSelectorReady = function (api) {
                accountTypeSelectorAPI = api;
                accountTypeSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onAccountTypeSelectionChanged = function () {
                var selectedId = accountTypeSelectorAPI.getSelectedIds();
                if (selectedId == undefined)
                    return;
                if (accountTypeSelectedDeferred != undefined) {
                    accountTypeSelectedDeferred.resolve();
                    return;
                }
                $scope.scopeModel.isLoading = true;
                loadRuntime().catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
            };

            $scope.scopeModel.onRequiredPartsDirectiveReady = function (api) {
                requiredPartsDirectiveAPI = api;
                requiredPartsDirectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateAccount() : insertAccount();
            };

            $scope.scopeModel.hasSaveAccountPermission = function () {
                return (isEditMode) ? Retail_BE_AccountAPIService.HasUpdateAccountPermission() : Retail_BE_AccountAPIService.HasAddAccountPermission();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };
        }
        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
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
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadAccountTypeSelectorWithRuntime]).catch(function (error) {
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
                return Retail_BE_AccountAPIService.GetAccountName(parentAccountId).then(function (response) {
                    var titleExtension = ' for ' + response;
                    $scope.title = title += titleExtension;
                });
            }
            else {
                $scope.title = title;
            }
        }
        function loadStaticData() {
            if (accountEntity == undefined)
                return;
            $scope.scopeModel.name = accountEntity.Name;
        }
        function loadAccountTypeSelectorWithRuntime() {
            var promises = [];

            var accountTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            promises.push(accountTypeSelectorLoadDeferred.promise);

            var accountTypeSelectorPayload = {
                filter: { ParentAccountId: parentAccountId }
            };

            if (accountEntity != undefined) {
                accountTypeSelectorPayload.selectedIds = accountEntity.TypeId;
                accountTypeSelectedDeferred = UtilsService.createPromiseDeferred();
            }

            accountTypeSelectorReadyDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(accountTypeSelectorAPI, accountTypeSelectorPayload, accountTypeSelectorLoadDeferred);
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });

            if (accountEntity != undefined) {
                var runtimeLoadDeferred = UtilsService.createPromiseDeferred();
                promises.push(runtimeLoadDeferred.promise);

                accountTypeSelectedDeferred.promise.then(function () {
                    accountTypeSelectedDeferred = undefined;

                    loadRuntime().then(function () {
                        runtimeLoadDeferred.resolve();
                    }).catch(function (error) {
                        runtimeLoadDeferred.reject(error);
                    });
                });
            }

            return UtilsService.waitMultiplePromises(promises);
        }
        function loadRuntime() {
            var promises = [];
            var accountTypeId = accountTypeSelectorAPI.getSelectedIds();

            var getAccountEditorRuntimePromise = getAccountEditorRuntime();
            promises.push(getAccountEditorRuntimePromise);

            var loadAccountPartDefinitionExtensionConfigsPromise = loadAccountPartDefinitionExtensionConfigs();
            promises.push(loadAccountPartDefinitionExtensionConfigsPromise);

            var loadPartsDeferred = UtilsService.createPromiseDeferred();
            promises.push(loadPartsDeferred.promise);

            var requiredPartDefinitions = [];
            $scope.scopeModel.notRequiredParts.length = 0;

            UtilsService.waitMultiplePromises([getAccountEditorRuntimePromise, loadAccountPartDefinitionExtensionConfigsPromise]).then(function () {
                if (accountEditorRuntime != undefined && accountEditorRuntime.Parts != null) {
                    for (var i = 0; i < accountEditorRuntime.Parts.length; i++) {
                        var part = accountEditorRuntime.Parts[i];
                        if (part.IsRequired) {
                            requiredPartDefinitions.push(part.PartDefinition);
                        }
                        else {
                            var notRequiredPart = buildNotRequiredPart(part.PartDefinition);
                            $scope.scopeModel.notRequiredParts.push(notRequiredPart);
                        }
                    }
                }

                UtilsService.waitMultipleAsyncOperations([loadRequiredPartsDirective, loadNotRequiredParts]).then(function () {
                    loadPartsDeferred.resolve();
                }).catch(function (error) {
                    loadPartsDeferred.reject(error, $scope);
                });
            });

            function getAccountEditorRuntime() {
                return Retail_BE_AccountAPIService.GetAccountEditorRuntime(accountTypeId, parentAccountId).then(function (response) {
                    accountEditorRuntime = response;
                });
            }
            function loadAccountPartDefinitionExtensionConfigs() {
                return Retail_BE_AccountPartDefinitionAPIService.GetAccountPartDefinitionExtensionConfigs().then(function (response) {
                    if (response != null) {
                        partDefinitionExtensionConfigs = [];
                        for (var i = 0; i < response.length; i++) {
                            partDefinitionExtensionConfigs.push(response[i]);
                        }
                    }
                });
            }
            function loadRequiredPartsDirective() {
                var requiredPartsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                requiredPartsDirectiveReadyDeferred.promise.then(function () {
                    var requiredPartsDirectivePayload = {};
                    requiredPartsDirectivePayload.context = {};
                    requiredPartsDirectivePayload.context.getPartDefinitionRuntimeEditor = getPartDefinitionRuntimeEditor;
                    requiredPartsDirectivePayload.partDefinitions = requiredPartDefinitions;

                    if (accountEntity != undefined && accountEntity.Settings != null)
                        requiredPartsDirectivePayload.parts = accountEntity.Settings.Parts;

                    VRUIUtilsService.callDirectiveLoad(requiredPartsDirectiveAPI, requiredPartsDirectivePayload, requiredPartsDirectiveLoadDeferred);
                });

                return requiredPartsDirectiveLoadDeferred.promise;
            }
            function loadNotRequiredParts() {
                var loadPromises = [];
                for (var i = 0; i < $scope.scopeModel.notRequiredParts.length; i++) {
                    var notRequiredPart = $scope.scopeModel.notRequiredParts[i];
                    if (notRequiredPart.isSelected)
                        loadPromises.push(notRequiredPart.directiveLoadDeferred.promise);
                }
                return UtilsService.waitMultiplePromises(loadPromises);
            }
            function buildNotRequiredPart(partDefinition) {
                var notRequiredPart = {};

                notRequiredPart.definition = partDefinition;
                notRequiredPart.runtimeEditor = getPartDefinitionRuntimeEditor(partDefinition.Settings.ConfigId);
                notRequiredPart.directiveLoadDeferred = UtilsService.createPromiseDeferred();
                notRequiredPart.isSelected = isNotRequiredPartSelected(partDefinition.AccountPartDefinitionId);

                notRequiredPart.onDirectiveReady = function (api) {
                    notRequiredPart.directiveAPI = api;
                    var directivePayload = {
                        partDefinition: partDefinition,
                        partSettings: (accountEntity != undefined && accountEntity.Settings != null) ?
                            accountEntity.Settings.Parts[partDefinition.AccountPartDefinitionId].Settings : undefined
                    };
                    notRequiredPart.isLoading = true;
                    VRUIUtilsService.callDirectiveLoad(notRequiredPart.directiveAPI, directivePayload, notRequiredPart.directiveLoadDeferred);
                    notRequiredPart.directiveLoadDeferred.promise.finally(function () {
                        notRequiredPart.isLoading = false;
                    });
                };

                notRequiredPart.directiveLoadDeferred.promise;

                return notRequiredPart;
            }
            function isNotRequiredPartSelected(partDefinitionId) {
                if (accountEntity == undefined || accountEntity.Settings == null || accountEntity.Settings.Parts == null)
                    return false;
                return (accountEntity.Settings.Parts[partDefinitionId] != null);
            }

            return UtilsService.waitMultiplePromises(promises);
        }
        function getPartDefinitionRuntimeEditor(partDefinitionId) {
            var partExtensionConfig = UtilsService.getItemByVal(partDefinitionExtensionConfigs, partDefinitionId, 'ExtensionConfigurationId');
            return (partExtensionConfig != undefined) ? partExtensionConfig.RuntimeEditor : null;
        }

        function insertAccount() {
            $scope.scopeModel.isLoading = true;

            var accountObj = buildAccountObjFromScope();

            return Retail_BE_AccountAPIService.AddAccount(accountObj).then(function (response) {
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

            return Retail_BE_AccountAPIService.UpdateAccount(accountObj).then(function (response) {
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
            var obj = {
                AccountId: accountId,
                Name: $scope.scopeModel.name,
                TypeId: accountTypeSelectorAPI.getSelectedIds(),
            };

            obj.Settings = {};
            obj.Settings.Parts = buildAccountParts();

            if (!isEditMode) {
                obj.ParentAccountId = parentAccountId;
            }

            return obj;
        }
        function buildAccountParts() {
            var accountParts = {};

            var requiredParts = requiredPartsDirectiveAPI.getData();
            if (requiredParts != undefined) {
                for (var key in requiredParts)
                    accountParts[key] = requiredParts[key];
            }

            for (var i = 0; i < $scope.scopeModel.notRequiredParts.length; i++) {
                var notRequiredPart = $scope.scopeModel.notRequiredParts[i];
                if (notRequiredPart.isSelected) {
                    var key = notRequiredPart.definition.AccountPartDefinitionId;
                    var partSettings = notRequiredPart.directiveAPI.getData();
                    accountParts[key] = { Settings: partSettings };
                }
            }

            return accountParts;
        }
    }

    appControllers.controller('Retail_BE_AccountEditorController', AccountEditorController);

})(appControllers);