(function (appControllers) {

    'use strict';

    AccountTypeEditorController.$inject = ['$scope', 'Retail_BE_AccountTypeAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService', 'Retail_BE_AccountPartAvailabilityOptionsEnum', 'Retail_BE_AccountPartRequiredOptionsEnum','Retail_BE_EntityTypeEnum'];

    function AccountTypeEditorController($scope, Retail_BE_AccountTypeAPIService, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, Retail_BE_AccountPartAvailabilityOptionsEnum, Retail_BE_AccountPartRequiredOptionsEnum, Retail_BE_EntityTypeEnum) {
        var isEditMode;

        var accountTypeId;
        var accountTypeEntity;

        var accountTypeSelectorAPI;
        var accountTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var accountTypeSelectedDeferred;

        var partDefinitionSelectorAPI;
        var partDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var statusDefinitionSelectorAPI;
        var statusDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var businessEntityDefinitionSelectorAPI;
        var businessEntityDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

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

            $scope.scopeModel.onStatusDefinitionSelectorReady = function (api) {
                statusDefinitionSelectorAPI = api;
                statusDefinitionSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.datasource = [];

            $scope.scopeModel.onSelectItem = function (dataItem) {
                addAccountPart(dataItem);
            };
            $scope.scopeModel.onBusinessEntityDefinitionSelectorReady = function (api) {
                businessEntityDefinitionSelectorAPI = api;
                businessEntityDefinitionSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onDeselectItem = function (dataItem) {
                var datasourceIndex = UtilsService.getItemIndexByVal($scope.scopeModel.datasource, dataItem.AccountPartDefinitionId, 'AccountPartDefinitionId');
                $scope.scopeModel.datasource.splice(datasourceIndex, 1);
            };
            $scope.scopeModel.removeFilter = function (dataItem) {
                var index = UtilsService.getItemIndexByVal($scope.scopeModel.selectedPartDefinitions, dataItem.AccountPartDefinitionId, 'AccountPartDefinitionId');
                $scope.scopeModel.selectedPartDefinitions.splice(index, 1);

                var datasourceIndex = $scope.scopeModel.datasource.indexOf(dataItem);
                $scope.scopeModel.datasource.splice(datasourceIndex, 1);
            };




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
            function setTitle() {
                if (isEditMode) {
                    var accountTypeName = (accountTypeEntity != undefined) ? accountTypeEntity.Name : undefined;
                    $scope.title = UtilsService.buildTitleForUpdateEditor(accountTypeName, 'Account Type');
                }
                else {
                    $scope.title = UtilsService.buildTitleForAddEditor('Account Type');
                }
            }
            function loadStatusDefinitionSelector() {
                var statusDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                statusDefinitionSelectorReadyDeferred.promise.then(function () {
                    var statusDefinitionSelectorPayload = {
                        filter: { EntityType: Retail_BE_EntityTypeEnum.Account.value },
                        selectedIds: accountTypeEntity != undefined && accountTypeEntity.Settings != undefined ? accountTypeEntity.Settings.InitialStatusId : undefined
                    };
                    VRUIUtilsService.callDirectiveLoad(statusDefinitionSelectorAPI, statusDefinitionSelectorPayload, statusDefinitionSelectorLoadDeferred);
                });
                return statusDefinitionSelectorLoadDeferred.promise;
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
            function loadStaticData() {
                if (accountTypeEntity == undefined)
                    return;
                $scope.scopeModel.name = accountTypeEntity.Name;
                $scope.scopeModel.title = accountTypeEntity.Title;

                if (accountTypeEntity.Settings != undefined) {
                    $scope.scopeModel.canBeRootAccount = accountTypeEntity.Settings.CanBeRootAccount;
                }

            }
            function loadBusinessEntityDefinitionSelector() {
                var businessEntityDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                businessEntityDefinitionSelectorReadyDeferred.promise.then(function () {
                    var payload = {
                        filter: {
                            Filters: [{
                                $type: "Retail.BusinessEntity.Entities.AccountBEDefinitionFilter, Retail.BusinessEntity.Entities",
                            }]
                        },
                        selectedIds: accountTypeEntity != undefined ? accountTypeEntity.AccountBEDefinitionId : undefined
                    };
                    VRUIUtilsService.callDirectiveLoad(businessEntityDefinitionSelectorAPI, payload, businessEntityDefinitionSelectorLoadDeferred);
                });

                return businessEntityDefinitionSelectorLoadDeferred.promise;
            }
            function loadPartDefinitionSection() {
                var partDefinitionIds;
                if (accountTypeEntity != undefined && accountTypeEntity.Settings != null && accountTypeEntity.Settings.PartDefinitionSettings != undefined) {
                    partDefinitionIds = [];
                    for (var i = 0; i < accountTypeEntity.Settings.PartDefinitionSettings.length; i++) {
                        var partDefinitionSetting = accountTypeEntity.Settings.PartDefinitionSettings[i];
                        partDefinitionIds.push(partDefinitionSetting.PartDefinitionId);
                    }
                }
                var partDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                partDefinitionSelectorReadyDeferred.promise.then(function () {
                    var partDefinitionSelectorPayload = partDefinitionIds != undefined ? { selectedIds: partDefinitionIds } : undefined;
                    VRUIUtilsService.callDirectiveLoad(partDefinitionSelectorAPI, partDefinitionSelectorPayload, partDefinitionSelectorLoadDeferred);
                });

                return partDefinitionSelectorLoadDeferred.promise.then(function () {
                    if (accountTypeEntity != undefined && accountTypeEntity.Settings != null && accountTypeEntity.Settings.PartDefinitionSettings != undefined) {
                        for (var i = 0; i < accountTypeEntity.Settings.PartDefinitionSettings.length; i++) {
                            var selectedPartDefinition = $scope.scopeModel.selectedPartDefinitions[i];
                            var partDefinitionSetting = accountTypeEntity.Settings.PartDefinitionSettings[i];
                            addAccountPart(selectedPartDefinition, partDefinitionSetting);
                        }
                    }
                });
            }


            return UtilsService.waitMultipleAsyncOperations([setTitle, loadAccountTypeSection, loadStaticData, loadPartDefinitionSection, loadStatusDefinitionSelector, loadBusinessEntityDefinitionSelector]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function addAccountPart(part, payload) {
            var dataItem = {
                AccountPartDefinitionId: part.AccountPartDefinitionId,
                title: part.Title,
                selectedAccountPartAvailability: payload != undefined ? UtilsService.getItemByVal($scope.scopeModel.accountPartAvailability, payload.AvailabilitySettings, "value") : $scope.scopeModel.accountPartAvailability[0],
                selectedAccountPartRequiredOptions: payload != undefined ? UtilsService.getItemByVal($scope.scopeModel.accountPartRequiredOptions, payload.RequiredSettings, "value") : $scope.scopeModel.accountPartRequiredOptions[0]
            };
            $scope.scopeModel.datasource.push(dataItem);
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
            var partDefinitionSettings;
            if ($scope.scopeModel.datasource.length > 0) {
                partDefinitionSettings = [];
                for (var i = 0 ; i < $scope.scopeModel.datasource.length ; i++) {
                    var dataItem = $scope.scopeModel.datasource[i];
                    partDefinitionSettings.push({
                        PartDefinitionId: dataItem.AccountPartDefinitionId,
                        AvailabilitySettings: dataItem.selectedAccountPartAvailability.value,
                        RequiredSettings: dataItem.selectedAccountPartRequiredOptions.value,
                    });
                }
            }
            var obj = {
                AccountTypeId: accountTypeId,
                Name: $scope.scopeModel.name,
                Title: $scope.scopeModel.title,
                Settings: {
                    CanBeRootAccount: $scope.scopeModel.canBeRootAccount,
                    SupportedParentAccountTypeIds: accountTypeSelectorAPI.getSelectedIds(),
                    PartDefinitionSettings: partDefinitionSettings,
                    InitialStatusId:statusDefinitionSelectorAPI.getSelectedIds()
                },
                AccountBEDefinitionId: businessEntityDefinitionSelectorAPI.getSelectedIds()
            };
            return obj;
        }
    }

    appControllers.controller('Retail_BE_AccountTypeEditorController', AccountTypeEditorController);

})(appControllers);