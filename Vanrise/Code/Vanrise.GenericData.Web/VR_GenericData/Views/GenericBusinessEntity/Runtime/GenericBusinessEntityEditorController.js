(function (appControllers) {

    'use strict';

    GenericBusinessEntityEditorController.$inject = ['$scope', 'VR_GenericData_GenericBEDefinitionAPIService', 'VR_GenericData_GenericBusinessEntityAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function GenericBusinessEntityEditorController($scope, VR_GenericData_GenericBEDefinitionAPIService, VR_GenericData_GenericBusinessEntityAPIService, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var isEditMode;
        var businessEntityDefinitionId;
        var businessEntityDefinitionSettings;

        var genericBusinessEntityEditorRuntime;
        var definitionTitle;
        var titleFieldName;

        var genericBusinessEntityId;
        var genericBusinessEntity;

        var runtimeEditorAPI;
        var runtimeEditorReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                businessEntityDefinitionId = parameters.businessEntityDefinitionId;
                genericBusinessEntityId = parameters.genericBusinessEntityId;
            }

            isEditMode = (genericBusinessEntityId != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onEditorRuntimeDirectiveReady = function (api) {
                runtimeEditorAPI = api;
                runtimeEditorReadyDeferred.resolve();
            };

            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateBusinessEntity() : insertBusinessEntity();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;

            getGenericBusinessEntityEditorRuntime().then(function () {
                loadAllControls().finally(function () {
                    genericBusinessEntity = undefined;
                });
               
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.scopeModel.isLoading = false;
            });
        }

        function getGenericBusinessEntityEditorRuntime() {
            return VR_GenericData_GenericBusinessEntityAPIService.GetGenericBusinessEntityEditorRuntime(businessEntityDefinitionId, genericBusinessEntityId).then(function (response) {
                genericBusinessEntityEditorRuntime = response;
                if(genericBusinessEntityEditorRuntime != undefined)
                {
                    definitionTitle = response.DefinitionTitle;
                    titleFieldName = response.TitleFieldName;
                    genericBusinessEntity = response.GenericBusinessEntity;
                    businessEntityDefinitionSettings = response.GenericBEDefinitionSettings;
                    if (businessEntityDefinitionSettings != undefined && businessEntityDefinitionSettings.EditorDefinition != undefined && businessEntityDefinitionSettings.EditorDefinition.Settings != undefined)
                        $scope.scopeModel.runtimeEditor = businessEntityDefinitionSettings.EditorDefinition.Settings.RuntimeEditor;
                }
            });
        }

        function loadAllControls() {

            function setTitle() {
                $scope.title = genericBusinessEntity != undefined && genericBusinessEntity.FieldValues != undefined ? UtilsService.buildTitleForUpdateEditor(genericBusinessEntity.FieldValues[titleFieldName], definitionTitle) : UtilsService.buildTitleForAddEditor(definitionTitle);
            }

            function loadEditorRuntimeDirective() {
                var runtimeEditorLoadDeferred = UtilsService.createPromiseDeferred();

                runtimeEditorReadyDeferred.promise.then(function () {
                    var runtimeEditorPayload = {
                        selectedValues: (isEditMode) ? genericBusinessEntity.FieldValues : undefined,
                        dataRecordTypeId: businessEntityDefinitionSettings.DataRecordTypeId,
                        definitionSettings: businessEntityDefinitionSettings.EditorDefinition.Settings
                    };
                    VRUIUtilsService.callDirectiveLoad(runtimeEditorAPI, runtimeEditorPayload, runtimeEditorLoadDeferred);
                });

                return runtimeEditorLoadDeferred.promise;
            }

            return UtilsService.waitMultipleAsyncOperations([loadEditorRuntimeDirective, setTitle]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function buildGenericBusinessEntityObjFromScope() {
            var genericBusinessEntity = {};

            genericBusinessEntity.GenericBusinessEntityId = genericBusinessEntityId;
            genericBusinessEntity.BusinessEntityDefinitionId = businessEntityDefinitionId;

            var fieldValues = {};
            runtimeEditorAPI.setData(fieldValues);
          
            genericBusinessEntity.FieldValues = fieldValues;

            return genericBusinessEntity;
        }

        function insertBusinessEntity() {
            $scope.scopeModel.isLoading = true;

            return VR_GenericData_GenericBusinessEntityAPIService.AddGenericBusinessEntity(buildGenericBusinessEntityObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded(titleFieldName, response, 'Title')) {
                    if ($scope.onGenericBEAdded != undefined)
                        $scope.onGenericBEAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function updateBusinessEntity() {
            $scope.scopeModel.isLoading = true;

            return VR_GenericData_GenericBusinessEntityAPIService.UpdateGenericBusinessEntity(buildGenericBusinessEntityObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated(titleFieldName, response, 'Title')) {
                    if ($scope.onGenericBEUpdated != undefined)
                        $scope.onGenericBEUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
    }

    appControllers.controller('VR_GenericData_GenericBusinessEntityEditorController', GenericBusinessEntityEditorController);
})(appControllers);