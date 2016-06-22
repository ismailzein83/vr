(function (appControllers) {

    'use strict';

    AccountEditorController.$inject = ['$scope', 'Retail_BE_AccountAPIService', 'Retail_BE_AccountTypeAPIService', 'Retail_BE_AccountPartDefinitionAPIService', 'Retail_BE_AccountPartRequiredOptionsEnum', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function AccountEditorController($scope, Retail_BE_AccountAPIService, Retail_BE_AccountTypeAPIService, Retail_BE_AccountPartDefinitionAPIService, Retail_BE_AccountPartRequiredOptionsEnum, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService)
    {
        var isEditMode;

        var accountId;
        var accountEntity;
        var parentAccountId;

        var accountTypeSelectorAPI;
        var accountTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var accountEditorRuntime;
        var requiredPartsDirectiveAPI;
        var requiredPartsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        var partDefinitionExtensionConfigs;

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

            $scope.scopeModel.notRequiredParts = [];

            $scope.scopeModel.onAccountTypeSelectorReady = function (api) {
                accountTypeSelectorAPI = api;
                accountTypeSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onAccountTypeSelectionChanged = function ()
            {
                var selectedId = accountTypeSelectorAPI.getSelectedIds();
                if (selectedId == undefined)
                    return;
                $scope.scopeModel.isLoading = true;
                loadAccountPartsSection().catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
            };

            $scope.scopeModel.onRequiredPartsDirectiveReady = function (api) {
                requiredPartsDirectiveAPI = api;
                requiredPartsDirectiveReadyDeferred.resolve();
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
                        console.log('2');
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

            //var p1 = setTitle();
            //var p2 = loadStaticData();
            //var p3 = loadAccountTypeSelector();
            //var p4 = loadAccountPartsSection();

            //UtilsService.convertToPromiseIfUndefined(p1).finally(function () { console.log('p1'); });
            //UtilsService.convertToPromiseIfUndefined(p2).finally(function () { console.log('p2'); });
            //UtilsService.convertToPromiseIfUndefined(p3).finally(function () { console.log('p3'); });
            //UtilsService.convertToPromiseIfUndefined(p4).finally(function () { console.log('p4'); });

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadAccountTypeSelector, loadAccountPartsSection]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                console.log('1');
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
        function loadAccountTypeSelector()
        {
            var accountTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            accountTypeSelectorReadyDeferred.promise.then(function ()
            {
                var accountTypeSelectorPayload = {
                    filter: { CanBeRootAccount: (parentAccountId == undefined) }
                };
                if (accountEntity != undefined) {
                    accountTypeSelectorPayload.selectedIds = (accountEntity != undefined) ? accountEntity.TypeId : undefined;
                }
                VRUIUtilsService.callDirectiveLoad(accountTypeSelectorAPI, accountTypeSelectorPayload, accountTypeSelectorLoadDeferred);
            });

            return accountTypeSelectorLoadDeferred.promise;
        }
        function loadAccountPartsSection()
        {
            if (!isEditMode && accountTypeSelectorAPI == undefined)
                return;

            var promises = [];
            var accountTypeId = (accountEntity != undefined) ? accountEntity.TypeId : accountTypeSelectorAPI.getSelectedIds();

            var requiredPartDefinitions = [];
            //$scope.scopeModel.notRequiredParts.length = 0;

            var getAccountEditorRuntimePromise = getAccountEditorRuntime();
            promises.push(getAccountEditorRuntimePromise);

            var loadAccountPartDefinitionExtensionConfigsPromise = loadAccountPartDefinitionExtensionConfigs();
            promises.push(loadAccountPartDefinitionExtensionConfigsPromise);

            var loadPartsDeferred = UtilsService.createPromiseDeferred();
            promises.push(loadPartsDeferred.promise);

            UtilsService.waitMultiplePromises([getAccountEditorRuntimePromise, loadAccountPartDefinitionExtensionConfigsPromise]).then(function ()
            {
                $scope.scopeModel.notRequiredParts.length = 0;

                if (accountEditorRuntime != undefined && accountEditorRuntime.Parts != null)
                {
                    for (var i = 0; i < accountEditorRuntime.Parts.length; i++) {
                        var part = accountEditorRuntime.Parts[i];
                        if (part.RequiredSettings == Retail_BE_AccountPartRequiredOptionsEnum.Required.value) {
                            requiredPartDefinitions.push(part.PartDefinition);
                        }
                        else if (part.RequiredSettings == Retail_BE_AccountPartRequiredOptionsEnum.NotRequired.value) {
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
            function loadRequiredPartsDirective()
            {
                var requiredPartsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                requiredPartsDirectiveReadyDeferred.promise.then(function ()
                {
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
            function loadNotRequiredParts()
            {
                var loadPromises = [];
                for (var i = 0; i < $scope.scopeModel.notRequiredParts.length; i++) {
                    var notRequiredPart = $scope.scopeModel.notRequiredParts[i];
                    if (notRequiredPart.isSelected)
                        loadPromises.push(notRequiredPart.directiveLoadDeferred.promise);
                }
                return UtilsService.waitMultiplePromises(loadPromises);
            }
            function buildNotRequiredPart(partDefinition)
            {
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
            function isNotRequiredPartSelected(partDefinitionId)
            {
                if (accountEntity == undefined || accountEntity.Settings == null || accountEntity.Settings.Parts == null)
                    return false;
                return (accountEntity.Settings.Parts[partDefinitionId] != null);
            }
            function getPartDefinitionRuntimeEditor(partDefinitionId) {
                var partExtensionConfig = UtilsService.getItemByVal(partDefinitionExtensionConfigs, partDefinitionId, 'ExtensionConfigurationId');
                return (partExtensionConfig != undefined) ? partExtensionConfig.RuntimeEditor : null;
            }

            return UtilsService.waitMultiplePromises(promises);
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
            obj.Settings.Parts = buildAccountParts();

            if (!isEditMode) {
                obj.ParentAccountId = parentAccountId;
            }

            console.log(obj);
            return obj;
        }
        function buildAccountParts()
        {
            var accountParts = {};

            var requiredParts = requiredPartsDirectiveAPI.getData();
            if (requiredParts != undefined)
            {
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