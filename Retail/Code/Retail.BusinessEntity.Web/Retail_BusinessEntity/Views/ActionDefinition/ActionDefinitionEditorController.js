﻿(function (appControllers) {

    'use strict';

    ActionDefinitionEditorController.$inject = ['$scope', 'Retail_BE_ActionDefinitionAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService','Retail_BE_EntityTypeEnum'];

    function ActionDefinitionEditorController($scope, Retail_BE_ActionDefinitionAPIService, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, Retail_BE_EntityTypeEnum) {
        var isEditMode;

        var actionDefinitionId;
        var actionDefinitionEntity;

        var serviceTypeSelectorAPI;
        var serviceTypeSelectorReadyDeferred;

        var directiveAPI;
        var directiveReadyDeferred;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                actionDefinitionId = parameters.actionDefinitionId;
            }
            isEditMode = (actionDefinitionId != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.extensionConfigs = [];

            $scope.scopeModel.selectedExtensionConfig;

            $scope.scopeModel.onDirectiveReady = function (api) {
                directiveAPI = api;
               var directivePayload = undefined;
                var setLoader = function (value) {
                    $scope.scopeModel.isLoadingDirective = value;
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
            };

            $scope.scopeModel.entityTypes = UtilsService.getArrayEnum(Retail_BE_EntityTypeEnum);

            $scope.scopeModel.onServiceTypeSelectorReady = function (api) {
                serviceTypeSelectorAPI = api;
                var setLoader = function (value) {
                    $scope.scopeModel.isLoadingDirective = value;
                };
                var payload;
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, serviceTypeSelectorAPI, payload, setLoader, serviceTypeSelectorReadyDeferred);
            }

            $scope.scopeModel.showServiceTypeSelector = function()
            {
                if($scope.scopeModel.selectedEntityType != undefined && $scope.scopeModel.selectedEntityType.value == Retail_BE_EntityTypeEnum.AccountService.value)
                {
                    return true;
                }
                return false;
            }

            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateActionDefinition() : insertActionDefinition();
            };

            $scope.scopeModel.hasSaveActionDefinitionPermission = function () {
                return (isEditMode) ? Retail_BE_ActionDefinitionAPIService.HasUpdateActionDefinitionPermission() : Retail_BE_ActionDefinitionAPIService.HasAddActionDefinitionPermission();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                getActionDefinition().then(function () {
                    loadAllControls().finally(function () {
                        actionDefinitionEntity = undefined;
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

        function getActionDefinition() {
            return Retail_BE_ActionDefinitionAPIService.GetActionDefinition(actionDefinitionId).then(function (response) {
                actionDefinitionEntity = response;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadServiceTypeSelector, loadActionBPDefinitionExtensionConfigs, loadDirective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function loadServiceTypeSelector()
        {
            if (actionDefinitionEntity != undefined && actionDefinitionEntity.Settings != undefined && actionDefinitionEntity.Settings.EntityTypeId != undefined)
            {
                serviceTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();
                var serviceTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                serviceTypeSelectorReadyDeferred.promise.then(function () {
                    serviceTypeSelectorReadyDeferred = undefined;
                    var serviceTypeSelectorPayload = {
                            selectedIds: actionDefinitionEntity.Settings.EntityTypeId
                        };
                    VRUIUtilsService.callDirectiveLoad(serviceTypeSelectorAPI, serviceTypeSelectorPayload, serviceTypeSelectorLoadDeferred);
                });

                return serviceTypeSelectorLoadDeferred.promise;
            }
          
        }

        function setTitle() {
            if (isEditMode) {
                var actionDefinitionName = (actionDefinitionEntity != undefined) ? actionDefinitionEntity.Name : undefined;
                $scope.title = UtilsService.buildTitleForUpdateEditor(actionDefinitionName, 'Action Definition');
            }
            else {
                $scope.title = UtilsService.buildTitleForAddEditor('Action Definition');
            }
        }

        function loadStaticData() {
            if (actionDefinitionEntity == undefined)
                return;
            $scope.scopeModel.name = actionDefinitionEntity.Name;
            if (actionDefinitionEntity.Settings != undefined)
            {
                $scope.scopeModel.description = actionDefinitionEntity.Settings.Description;
                $scope.scopeModel.selectedEntityType = UtilsService.getItemByVal($scope.scopeModel.entityTypes, actionDefinitionEntity.Settings.EntityType, "value");
            }
        }

        function loadActionBPDefinitionExtensionConfigs() {
            return Retail_BE_ActionDefinitionAPIService.GetActionBPDefinitionExtensionConfigs().then(function (response) {
                if (response != undefined) {
                    for (var i = 0; i < response.length; i++) {
                        $scope.scopeModel.extensionConfigs.push(response[i]);
                    }
                    if (actionDefinitionEntity != undefined && actionDefinitionEntity.Settings != undefined &&  actionDefinitionEntity.Settings.BPDefinitionSettings != undefined)
                        $scope.scopeModel.selectedExtensionConfig = UtilsService.getItemByVal($scope.scopeModel.extensionConfigs, actionDefinitionEntity.Settings.BPDefinitionSettings.ConfigId, 'ExtensionConfigurationId');
                    else if ($scope.scopeModel.extensionConfigs.length > 0)
                        $scope.scopeModel.selectedExtensionConfig = $scope.scopeModel.extensionConfigs[0];
                }
            });
        }

        function loadDirective() {
            if (actionDefinitionEntity != undefined && actionDefinitionEntity.Settings != undefined && actionDefinitionEntity.Settings.BPDefinitionSettings != undefined) {
                directiveReadyDeferred = UtilsService.createPromiseDeferred();
                var directiveLoadDeferred = UtilsService.createPromiseDeferred();
                directiveReadyDeferred.promise.then(function () {
                    directiveReadyDeferred = undefined;
                    var directivePayload = {
                        bpDefinitionSettings: actionDefinitionEntity.Settings.BPDefinitionSettings
                    };
                    VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
                });
                return directiveLoadDeferred.promise;
            }

        }

        function insertActionDefinition() {
            $scope.scopeModel.isLoading = true;

            var actionDefinitionObj = buildActionDefinitionObjFromScope();

            return Retail_BE_ActionDefinitionAPIService.AddActionDefinition(actionDefinitionObj).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('Account Type', response, 'Name')) {
                    if ($scope.onActionDefinitionAdded != undefined)
                        $scope.onActionDefinitionAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function updateActionDefinition() {
            $scope.scopeModel.isLoading = true;

            var actionDefinitionObj = buildActionDefinitionObjFromScope();

            return Retail_BE_ActionDefinitionAPIService.UpdateActionDefinition(actionDefinitionObj).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('Account Type', response, 'Name')) {
                    if ($scope.onActionDefinitionUpdated != undefined) {
                        $scope.onActionDefinitionUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function buildActionDefinitionObjFromScope() {
            var bPDefinitionSettings = directiveAPI.getData();
            if (bPDefinitionSettings != undefined)
                bPDefinitionSettings.ConfigId = $scope.scopeModel.selectedExtensionConfig.ExtensionConfigurationId;
            var obj = {
                ActionDefinitionId: actionDefinitionId,
                Name: $scope.scopeModel.name,
                Settings: {
                    Description: $scope.scopeModel.description,
                    EntityType: $scope.scopeModel.selectedEntityType.value,
                    EntityTypeId: serviceTypeSelectorAPI != undefined ? serviceTypeSelectorAPI.getSelectedIds() : undefined,
                    BPDefinitionSettings: bPDefinitionSettings
                }
            };
            return obj;
        }
    }

    appControllers.controller('Retail_BE_ActionDefinitionEditorController', ActionDefinitionEditorController);

})(appControllers);