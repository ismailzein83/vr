(function (appControllers) {

    'use strict';

    OperatorDeclaredInfoEditorController.$inject = ['$scope', 'Retail_BE_OperatorDeclaredInfoAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService', 'Retail_BE_AccountPartAvailabilityOptionsEnum', 'Retail_BE_AccountPartRequiredOptionsEnum','Retail_BE_EntityTypeEnum'];

    function OperatorDeclaredInfoEditorController($scope, Retail_BE_OperatorDeclaredInfoAPIService, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, Retail_BE_AccountPartAvailabilityOptionsEnum, Retail_BE_AccountPartRequiredOptionsEnum, Retail_BE_EntityTypeEnum) {
        var isEditMode;

        var OperatorDeclaredInfoId;
        var OperatorDeclaredInfoEntity;

        var OperatorDeclaredInfoSelectorAPI;
        var OperatorDeclaredInfoSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var OperatorDeclaredInfoSelectedDeferred;

        var partDefinitionSelectorAPI;
        var partDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var statusDefinitionSelectorAPI;
        var statusDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                OperatorDeclaredInfoId = parameters.OperatorDeclaredInfoId;
            }
            isEditMode = (OperatorDeclaredInfoId != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onStatusDefinitionSelectorReady = function (api) {
                statusDefinitionSelectorAPI = api;
                statusDefinitionSelectorReadyDeferred.resolve();
            }
            $scope.scopeModel.datasource = [];

            $scope.scopeModel.onSelectItem = function (dataItem) {
                addAccountPart(dataItem);
            }
            $scope.scopeModel.onDeselectItem = function (dataItem) {
                var datasourceIndex = UtilsService.getItemIndexByVal($scope.scopeModel.datasource, dataItem.AccountPartDefinitionId, 'AccountPartDefinitionId');
                $scope.scopeModel.datasource.splice(datasourceIndex, 1);
            }
            $scope.scopeModel.removeFilter = function (dataItem) {
                var index = UtilsService.getItemIndexByVal($scope.scopeModel.selectedPartDefinitions, dataItem.AccountPartDefinitionId, 'AccountPartDefinitionId');
                $scope.scopeModel.selectedPartDefinitions.splice(index, 1);

                var datasourceIndex = $scope.scopeModel.datasource.indexOf(dataItem);
                $scope.scopeModel.datasource.splice(datasourceIndex, 1);
            };




            $scope.scopeModel.accountPartAvailability = UtilsService.getArrayEnum(Retail_BE_AccountPartAvailabilityOptionsEnum);

            $scope.scopeModel.accountPartRequiredOptions = UtilsService.getArrayEnum(Retail_BE_AccountPartRequiredOptionsEnum);


            $scope.scopeModel.onOperatorDeclaredInfoSelectorReady = function (api) {
                OperatorDeclaredInfoSelectorAPI = api;
                OperatorDeclaredInfoSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onOperatorDeclaredInfoPartDefinitionReady = function (api) {
                OperatorDeclaredInfoPartDefinitionAPI = api;
                OperatorDeclaredInfoPartDefinitionReadyDeferred.resolve();
            };

            $scope.scopeModel.onPartDefinitionSelectorReady = function (api) {
                partDefinitionSelectorAPI = api;
                partDefinitionSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateOperatorDeclaredInfo() : insertOperatorDeclaredInfo();
            };

            $scope.scopeModel.hasSaveOperatorDeclaredInfoPermission = function () {
                return (isEditMode) ? Retail_BE_OperatorDeclaredInfoAPIService.HasUpdateOperatorDeclaredInfoPermission() : Retail_BE_OperatorDeclaredInfoAPIService.HasAddOperatorDeclaredInfoPermission();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };
        }
        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                getOperatorDeclaredInfo().then(function () {
                    loadAllControls().finally(function () {
                        OperatorDeclaredInfoEntity = undefined;
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

        function getOperatorDeclaredInfo() {
            return Retail_BE_OperatorDeclaredInfoAPIService.GetOperatorDeclaredInfo(OperatorDeclaredInfoId).then(function (response) {
                OperatorDeclaredInfoEntity = response;
            });
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadOperatorDeclaredInfoSection, loadStaticData, loadPartDefinitionSection,loadStatusDefinitionSelector]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function setTitle() {
            if (isEditMode) {
                var OperatorDeclaredInfoName = (OperatorDeclaredInfoEntity != undefined) ? OperatorDeclaredInfoEntity.Name : undefined;
                $scope.title = UtilsService.buildTitleForUpdateEditor(OperatorDeclaredInfoName, 'Account Type');
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
                        selectedIds: OperatorDeclaredInfoEntity != undefined && OperatorDeclaredInfoEntity.Settings != undefined? OperatorDeclaredInfoEntity.Settings.InitialStatusId : undefined
                    };
                    VRUIUtilsService.callDirectiveLoad(statusDefinitionSelectorAPI, statusDefinitionSelectorPayload, statusDefinitionSelectorLoadDeferred);
                });
                return statusDefinitionSelectorLoadDeferred.promise;
        }

        function loadStaticData() {
            if (OperatorDeclaredInfoEntity == undefined)
                return;
            $scope.scopeModel.name = OperatorDeclaredInfoEntity.Name;
            $scope.scopeModel.title = OperatorDeclaredInfoEntity.Title;

            if (OperatorDeclaredInfoEntity.Settings != undefined) {
                $scope.scopeModel.canBeRootAccount = OperatorDeclaredInfoEntity.Settings.CanBeRootAccount;
            }

        }

        function loadOperatorDeclaredInfoSection() {
            var OperatorDeclaredInfoSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            OperatorDeclaredInfoSelectorReadyDeferred.promise.then(function () {
                var OperatorDeclaredInfoSelectorPayload;
                if (OperatorDeclaredInfoEntity != undefined && OperatorDeclaredInfoEntity.Settings != null) {
                    OperatorDeclaredInfoSelectorPayload = {
                        selectedIds: OperatorDeclaredInfoEntity.Settings.SupportedParentOperatorDeclaredInfoIds
                    };
                }
                VRUIUtilsService.callDirectiveLoad(OperatorDeclaredInfoSelectorAPI, OperatorDeclaredInfoSelectorPayload, OperatorDeclaredInfoSelectorLoadDeferred);
            });

            return OperatorDeclaredInfoSelectorLoadDeferred.promise;
        }

        function loadPartDefinitionSection() {
            var partDefinitionIds;
            if (OperatorDeclaredInfoEntity != undefined && OperatorDeclaredInfoEntity.Settings != null && OperatorDeclaredInfoEntity.Settings.PartDefinitionSettings != undefined) {
                partDefinitionIds = [];
                for (var i = 0; i < OperatorDeclaredInfoEntity.Settings.PartDefinitionSettings.length; i++) {
                    var partDefinitionSetting = OperatorDeclaredInfoEntity.Settings.PartDefinitionSettings[i];
                    partDefinitionIds.push(partDefinitionSetting.PartDefinitionId);
                }
            }
            var partDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            partDefinitionSelectorReadyDeferred.promise.then(function () {
                var partDefinitionSelectorPayload = partDefinitionIds != undefined ? { selectedIds: partDefinitionIds } : undefined;
                VRUIUtilsService.callDirectiveLoad(partDefinitionSelectorAPI, partDefinitionSelectorPayload, partDefinitionSelectorLoadDeferred);
            });

            return partDefinitionSelectorLoadDeferred.promise.then(function () {
                if (OperatorDeclaredInfoEntity != undefined && OperatorDeclaredInfoEntity.Settings != null && OperatorDeclaredInfoEntity.Settings.PartDefinitionSettings != undefined) {
                    for (var i = 0; i < OperatorDeclaredInfoEntity.Settings.PartDefinitionSettings.length; i++) {
                        var selectedPartDefinition = $scope.scopeModel.selectedPartDefinitions[i];
                        var partDefinitionSetting = OperatorDeclaredInfoEntity.Settings.PartDefinitionSettings[i];
                        addAccountPart(selectedPartDefinition, partDefinitionSetting);
                    }
                }
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
        function insertOperatorDeclaredInfo() {
            $scope.scopeModel.isLoading = true;

            var OperatorDeclaredInfoObj = buildOperatorDeclaredInfoObjFromScope();

            return Retail_BE_OperatorDeclaredInfoAPIService.AddOperatorDeclaredInfo(OperatorDeclaredInfoObj).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('Account Type', response, 'Name')) {
                    if ($scope.onOperatorDeclaredInfoAdded != undefined)
                        $scope.onOperatorDeclaredInfoAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function updateOperatorDeclaredInfo() {
            $scope.scopeModel.isLoading = true;

            var OperatorDeclaredInfoObj = buildOperatorDeclaredInfoObjFromScope();

            return Retail_BE_OperatorDeclaredInfoAPIService.UpdateOperatorDeclaredInfo(OperatorDeclaredInfoObj).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('Account Type', response, 'Name')) {
                    if ($scope.onOperatorDeclaredInfoUpdated != undefined) {
                        $scope.onOperatorDeclaredInfoUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function buildOperatorDeclaredInfoObjFromScope() {
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
                OperatorDeclaredInfoId: OperatorDeclaredInfoId,
                Name: $scope.scopeModel.name,
                Title: $scope.scopeModel.title,
                Settings: {
                    CanBeRootAccount: $scope.scopeModel.canBeRootAccount,
                    SupportedParentOperatorDeclaredInfoIds: OperatorDeclaredInfoSelectorAPI.getSelectedIds(),
                    PartDefinitionSettings: partDefinitionSettings,
                    InitialStatusId:statusDefinitionSelectorAPI.getSelectedIds()
                }
            };
            return obj;
        }
    }

    appControllers.controller('Retail_BE_OperatorDeclaredInfoEditorController', OperatorDeclaredInfoEditorController);

})(appControllers);