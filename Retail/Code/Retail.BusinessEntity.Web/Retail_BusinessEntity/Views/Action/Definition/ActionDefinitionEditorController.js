(function (appControllers) {

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
        var actionBPDefinitionSelectedDefferred;

        var statusDefinitionSelectorAPI;
        var statusDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var statusDefinitionSelectedDefferred;

        var entityTypeAPI;
        var entityTypeAPISelectorReadyDeferred = UtilsService.createPromiseDeferred();


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

            $scope.scopeModel.onEntityTypeSelectorReady = function (api) {
                entityTypeAPI = api;
                entityTypeAPISelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onStatusDefinitionSelectorReady = function (api) {
                statusDefinitionSelectorAPI = api;
                statusDefinitionSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.selectedExtensionConfig;

            $scope.scopeModel.onDirectiveReady = function (api) {
                directiveAPI = api;
               var directivePayload = undefined;
                var setLoader = function (value) {
                    $scope.scopeModel.isLoadingDirective = value;
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
            };

            $scope.scopeModel.onServiceTypeSelectorReady = function (api) {
                serviceTypeSelectorAPI = api;
                var setLoader = function (value) {
                    $scope.scopeModel.isLoadingDirective = value;
                };
                var payload;
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, serviceTypeSelectorAPI, payload, setLoader, serviceTypeSelectorReadyDeferred);
            };

            $scope.scopeModel.showServiceTypeSelector = function () {
                if ($scope.scopeModel.selectedEntityType != undefined && $scope.scopeModel.selectedEntityType.value == Retail_BE_EntityTypeEnum.AccountService.value) {
                    return true;
                }
                return false;
            };

            $scope.scopeModel.onEntityTypeSelectionChanged = function () {

                var selectedEntityType = entityTypeAPI.getSelectedIds();
                if (selectedEntityType != undefined) {
                    var setStatusDefinitionLoader = function (value) {
                        $scope.scopeModel.isLoadingStatusDefinitionDirective = value;
                    };
                    var payload = { filter: { EntityType: selectedEntityType } };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, statusDefinitionSelectorAPI, payload, setStatusDefinitionLoader, statusDefinitionSelectedDefferred);

                    if (directiveAPI != undefined) {
                        var setActionBPDefinitionLoader = function (value) {
                            $scope.scopeModel.isLoadingActionBPDefinitionDirective = value;
                        };
                        var actionBPDefinitionPayload = { entityType: selectedEntityType };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, actionBPDefinitionPayload, setActionBPDefinitionLoader, actionBPDefinitionSelectedDefferred);
                    }

                }
            };

            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateActionDefinition() : insertActionDefinition();
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
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadServiceTypeSelector, loadActionBPDefinitionExtensionConfigs, loadDirective, loadStatusDefinitionSelector, loadEntityTypeSelector]).catch(function (error) {
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

        function loadStatusDefinitionSelector() {
            if (actionDefinitionEntity)
            {
                statusDefinitionSelectedDefferred = UtilsService.createPromiseDeferred();
                var statusDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                statusDefinitionSelectorReadyDeferred.promise.then(function () {
                    statusDefinitionSelectorReadyDeferred = undefined;
                    var statusDefinitionSelectorPayload = {
                        filter:  { EntityType: actionDefinitionEntity.EntityType },
                        selectedIds: convertSupportedOnStatusesFromObj()
                    };
                    VRUIUtilsService.callDirectiveLoad(statusDefinitionSelectorAPI, statusDefinitionSelectorPayload, statusDefinitionSelectorLoadDeferred);
                });
                return statusDefinitionSelectorLoadDeferred.promise.then(function () {
                    statusDefinitionSelectedDefferred = undefined;
                });
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

        function loadEntityTypeSelector() {
            var entityTypeLoadDeferred = UtilsService.createPromiseDeferred();
            entityTypeAPISelectorReadyDeferred.promise.then(function () {
                var entityTypePayload;
                if (isEditMode) {
                    entityTypePayload = {
                      
                        selectedIds: actionDefinitionEntity.EntityType
                    };
                }
                VRUIUtilsService.callDirectiveLoad(entityTypeAPI, entityTypePayload, entityTypeLoadDeferred);
            });
            return entityTypeLoadDeferred.promise;
        }

        function loadStaticData() {
            if (actionDefinitionEntity == undefined)
                return;
            $scope.scopeModel.name = actionDefinitionEntity.Name;
            if (actionDefinitionEntity.Settings != undefined)
            {
                $scope.scopeModel.description = actionDefinitionEntity.Settings.Description;
                
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
                        bpDefinitionSettings: actionDefinitionEntity.Settings.BPDefinitionSettings,
                        entityType: entityTypeAPI.getSelectedIds()
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

        function convertSupportedOnStatusesFromScope(selectedStatuses)
        {
            var supportedOnStatuses = [];
            if(selectedStatuses != undefined)
            {
                for(var i=0;i< selectedStatuses.length;i++)
                {
                    var selectedStatus = selectedStatuses[i];
                    supportedOnStatuses.push({
                        StatusDefinitionId: selectedStatus
                    });
                }
            }
            return supportedOnStatuses;
        }

        function convertSupportedOnStatusesFromObj() {
            var statusDefinitionIds = [];
            if (actionDefinitionEntity != undefined && actionDefinitionEntity.Settings != undefined && actionDefinitionEntity.Settings.SupportedOnStatuses != undefined)
            {
                for (var i = 0; i < actionDefinitionEntity.Settings.SupportedOnStatuses.length; i++) {
                    var supportedOnStatus = actionDefinitionEntity.Settings.SupportedOnStatuses[i];
                    statusDefinitionIds.push(supportedOnStatus.StatusDefinitionId);
                }
            }
            return statusDefinitionIds;
        }

        function buildActionDefinitionObjFromScope() {
            var bPDefinitionSettings = directiveAPI.getData();
            if (bPDefinitionSettings != undefined)
                bPDefinitionSettings.ConfigId = $scope.scopeModel.selectedExtensionConfig.ExtensionConfigurationId;

            var supportedOnStatuses = convertSupportedOnStatusesFromScope( statusDefinitionSelectorAPI.getSelectedIds());
            var obj = {
                ActionDefinitionId: actionDefinitionId,
                Name: $scope.scopeModel.name,
                EntityType: entityTypeAPI.getSelectedIds(),
                Settings: {
                    Description: $scope.scopeModel.description,
                    
                    EntityTypeId: serviceTypeSelectorAPI != undefined ? serviceTypeSelectorAPI.getSelectedIds() : undefined,
                    BPDefinitionSettings: bPDefinitionSettings,
                    SupportedOnStatuses: supportedOnStatuses
                }
            };
            return obj;
        }
    }

    appControllers.controller('Retail_BE_ActionDefinitionEditorController', ActionDefinitionEditorController);

})(appControllers);