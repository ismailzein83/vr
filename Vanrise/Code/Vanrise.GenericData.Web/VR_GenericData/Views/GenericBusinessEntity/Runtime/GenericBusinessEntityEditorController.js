(function (appControllers) {

    'use strict';

    GenericBusinessEntityEditorController.$inject = ['$scope', 'VR_GenericData_GenericBEDefinitionAPIService', 'VR_GenericData_GenericBusinessEntityAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function GenericBusinessEntityEditorController($scope, VR_GenericData_GenericBEDefinitionAPIService, VR_GenericData_GenericBusinessEntityAPIService, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var saveAndNew = false;
        var definitionTitle;
        var titleFieldName;
        var titleOperation;

        var genericBusinessEntityId;
        var businessEntityDefinitionId;
        var isReadOnly;

        var additionalErrorsDirectiveAPI;
        var additionalErrorsReadyDeferred = UtilsService.createPromiseDeferred();

        var fieldValues;

        var runtimeEditorAPI;
        var runtimeEditorReadyDeferred = UtilsService.createPromiseDeferred();

        var errorEntity;
        var historyId;
        $scope.scopeModel = {};

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                businessEntityDefinitionId = parameters.businessEntityDefinitionId;
                genericBusinessEntityId = parameters.genericBusinessEntityId;
                fieldValues = parameters.fieldValues;
                isReadOnly = parameters.isReadOnly;
                historyId = parameters.historyId;
            }
            $scope.scopeModel.isEditMode = (genericBusinessEntityId != undefined);
        }

        function defineScope() {

            $scope.scopeModel.onEditorRuntimeDirectiveReady = function (api) {
                runtimeEditorAPI = api;
                runtimeEditorReadyDeferred.resolve();
            };
            $scope.scopeModel.onAdditionalErrorsDirectiveReady = function (api) {
                additionalErrorsDirectiveAPI = api;
                additionalErrorsReadyDeferred.resolve();
            };
            $scope.scopeModel.saveAndNew = function () {
                saveAndNew = true;
                return $scope.scopeModel.save();
            };
            $scope.scopeModel.save = function () {
                return ($scope.scopeModel.isEditMode) ? updateBusinessEntity() : insertBusinessEntity();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {

            function loadEditorRuntimeDirective() {
                var runtimeEditorLoadDeferred = UtilsService.createPromiseDeferred();

                runtimeEditorReadyDeferred.promise.then(function () {
                    var runtimeEditorPayload = {
                        businessEntityDefinitionId: businessEntityDefinitionId,
                        genericBusinessEntityId: genericBusinessEntityId,
                        fieldValues: fieldValues,
                        context: getContext(),
                        isReadOnly: isReadOnly,
                        historyId: historyId
                    };
                    VRUIUtilsService.callDirectiveLoad(runtimeEditorAPI, runtimeEditorPayload, runtimeEditorLoadDeferred);
                });

                return runtimeEditorLoadDeferred.promise;
            }

            return UtilsService.waitMultipleAsyncOperations([loadEditorRuntimeDirective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function getContext() {
            var context = {
            };
            context.setTitle = function (title) {
                $scope.title = title;
            };
            context.setTitleFieldName = function (fieldName) {
                titleFieldName = fieldName;
            };
            context.setTitleOperation = function (fieldName) {
                titleOperation = fieldName;
            };
            return context;
        }

        function buildGenericBusinessEntityObjFromScope() {
            return runtimeEditorAPI.getData();
        }


        function insertBusinessEntity() {
            $scope.scopeModel.isLoading = true;

            return VR_GenericData_GenericBusinessEntityAPIService.AddGenericBusinessEntity(buildGenericBusinessEntityObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded(titleOperation, response, 'Title', additionalErrorsDirectiveAPI)) {
                    if ($scope.onGenericBEAdded != undefined) {
                        if (response.Message != undefined && response.ShowPopupErrorMessage) {
                            errorEntity = {
                                message: response.Message
                            };
                        }
                        $scope.onGenericBEAdded(response.InsertedObject, saveAndNew, errorEntity);
                    }
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
                if (VRNotificationService.notifyOnItemUpdated(titleOperation, response, 'Title', additionalErrorsDirectiveAPI)) {
                    if ($scope.onGenericBEUpdated != undefined) {
                        if (response.Message != undefined && response.ShowPopupErrorMessage) {
                            errorEntity = {
                                message: response.Message
                            };
                        }
                        $scope.onGenericBEUpdated(response.UpdatedObject,errorEntity);
                    }
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