(function (appControllers) {

    'use strict';

    GenericBusinessEntityEditorController.$inject = ['$scope', 'VR_GenericData_GenericUIRuntimeAPIService', 'VR_GenericData_GenericBusinessEntityAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function GenericBusinessEntityEditorController($scope, VR_GenericData_GenericUIRuntimeAPIService, VR_GenericData_GenericBusinessEntityAPIService, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var isEditMode;

        var businessEntityDefinitionId;
        var runtimeEditor;
        var businessEntityTitle;

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

            $scope.scopeModel.onRuntimeEditorReady = function (api) {
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

            getRuntimeEditor().then(function () {
                if (isEditMode) {
                    getGenericBusinessEntity().then(function () {
                        loadAllControls().finally(function () {
                            genericBusinessEntity = undefined;
                            });
                    }).catch(function () {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                        $scope.scopeModel.isLoading = false;
                    });
                }
                else {
                    loadAllControls();
                }
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.scopeModel.isLoading = false;
            });
        }

        function getRuntimeEditor() {
            return VR_GenericData_GenericUIRuntimeAPIService.GetGenericEditorRuntime(businessEntityDefinitionId).then(function (response) {
                runtimeEditor = response;
            });
        }

        function getGenericBusinessEntity() {
            return VR_GenericData_GenericBusinessEntityAPIService.GetGenericBusinessEntity(genericBusinessEntityId, businessEntityDefinitionId).then(function (response) {
                genericBusinessEntity = response;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadRuntimeEditor, loadBusinessEntityTitle]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function setTitle() {
            if (businessEntityTitle != undefined)
                $scope.title = (isEditMode) ? UtilsService.buildTitleForUpdateEditor(businessEntityTitle.EntityName,businessEntityTitle.Title) : UtilsService.buildTitleForAddEditor(businessEntityTitle.Title);
        }

        function loadBusinessEntityTitle()
        {
            return VR_GenericData_GenericBusinessEntityAPIService.GetBusinessEntityTitle(businessEntityDefinitionId, genericBusinessEntityId).then(function (response) {
                businessEntityTitle = response;
                setTitle();
            });
        }

        function loadRuntimeEditor() {
            var runtimeEditorLoadDeferred = UtilsService.createPromiseDeferred();

            runtimeEditorReadyDeferred.promise.then(function () {
                var runtimeEditorPayload = {
                    sections: runtimeEditor.Sections,
                    selectedValues: (isEditMode) ? genericBusinessEntity.Details : undefined
                };
                VRUIUtilsService.callDirectiveLoad(runtimeEditorAPI, runtimeEditorPayload, runtimeEditorLoadDeferred);
            });

            return runtimeEditorLoadDeferred.promise;
        }

        function buildGenericBusinessEntityObjFromScope() {
            var genericBusinessEntity = {};

            genericBusinessEntity.GenericBusinessEntityId = genericBusinessEntityId;
            genericBusinessEntity.BusinessEntityDefinitionId = businessEntityDefinitionId;
            genericBusinessEntity.Details = runtimeEditorAPI.getData();

            return genericBusinessEntity;
        }

        function insertBusinessEntity() {
            $scope.scopeModel.isLoading = true;

            return VR_GenericData_GenericBusinessEntityAPIService.AddGenericBusinessEntity(buildGenericBusinessEntityObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded(businessEntityTitle.Title, response, 'Title')) {
                    if ($scope.onGenericBusinessEntityAdded != undefined)
                        $scope.onGenericBusinessEntityAdded(response.InsertedObject);
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
                if (VRNotificationService.notifyOnItemUpdated(businessEntityTitle.Title, response, 'Title')) {
                    if ($scope.onGenericBusinessEntityUpdated != undefined)
                        $scope.onGenericBusinessEntityUpdated(response.UpdatedObject);
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