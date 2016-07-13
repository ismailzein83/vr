(function (appControllers) {

    'use strict';

    ActionDefinitionEditorController.$inject = ['$scope', 'Retail_BE_ActionDefinitionAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService','Retail_BE_EntityTypeEnum'];

    function ActionDefinitionEditorController($scope, Retail_BE_ActionDefinitionAPIService, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, Retail_BE_EntityTypeEnum) {
        var isEditMode;

        var actionDefinitionId;
        var actionDefinitionEntity;

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

            $scope.scopeModel.entityTypes = UtilsService.getArrayEnum(Retail_BE_EntityTypeEnum);


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
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function setTitle() {
            if (isEditMode) {
                var actionDefinitionName = (actionDefinitionEntity != undefined) ? actionDefinitionEntity.Name : undefined;
                $scope.title = UtilsService.buildTitleForUpdateEditor(actionDefinitionName, 'Account Type');
            }
            else {
                $scope.title = UtilsService.buildTitleForAddEditor('Account Type');
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
            var obj = {
                ActionDefinitionId: actionDefinitionId,
                Name: $scope.scopeModel.name,
                Settings: {
                    Description: $scope.scopeModel.description,
                    EntityType: $scope.scopeModel.selectedEntityType.value
                }
            };
            return obj;
        }
    }

    appControllers.controller('Retail_BE_ActionDefinitionEditorController', ActionDefinitionEditorController);

})(appControllers);