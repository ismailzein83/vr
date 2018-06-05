(function (appControllers) {

    'use strict';

    GenericBusinessEntityEditorController.$inject = ['$scope', 'VR_GenericData_GenericBEDefinitionAPIService', 'VR_GenericData_GenericBusinessEntityAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function GenericBusinessEntityEditorController($scope, VR_GenericData_GenericBEDefinitionAPIService, VR_GenericData_GenericBusinessEntityAPIService, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var isEditMode;
        var definitionTitle;
        var titleFieldName;

        var genericBusinessEntityId;
        var businessEntityDefinitionId;

        var fieldValues;

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
                fieldValues = parameters.fieldValues;
            }

            isEditMode = (genericBusinessEntityId != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {
        };

            $scope.scopeModel.onEditorRuntimeDirectiveReady = function (api) {
                runtimeEditorAPI = api;
                runtimeEditorReadyDeferred.resolve();
        };

            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateBusinessEntity(): insertBusinessEntity();
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
                            context: getContext()
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
            return context;
    }

        function buildGenericBusinessEntityObjFromScope() {
            return runtimeEditorAPI.getData();
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