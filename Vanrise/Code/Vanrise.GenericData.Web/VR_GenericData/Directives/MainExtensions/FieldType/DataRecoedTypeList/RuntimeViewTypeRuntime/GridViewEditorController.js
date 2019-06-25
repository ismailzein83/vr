(function (appControllers) {
    "use strict";
    gridViewEditorController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function gridViewEditorController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var isEditMode;
        var fieldValues;
        var dataRow;
        var runtimeEditorAPI;
        var runtimeEditorReadyDeferred = UtilsService.createPromiseDeferred();
        var title;
        $scope.scopeModel = {};

        var dataRecordTypeId;
        var definitionSettings;
        var dataRow;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope); 
            if (parameters != undefined && parameters != null) {
                dataRecordTypeId = parameters.dataRecordTypeId;
                definitionSettings = parameters.definitionSettings;
                dataRow = parameters.dataRow;
                title = parameters.title;
            }
            if (dataRow != undefined) {
                isEditMode = true;
                fieldValues = dataRow.Entity;
            }
        }

        function defineScope() {
            $scope.scopeModel.runtimeEditor = definitionSettings.RuntimeEditor;
            $scope.scopeModel.saveDataRow = function () {

                if (isEditMode)
                    return updateDataRow();
                else
                    return insertDataRow();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

            $scope.scopeModel.onEditorRuntimeDirectiveReady = function (api) {
                runtimeEditorAPI = api;
                runtimeEditorReadyDeferred.resolve();
            };
        }

        function loadEditorRuntimeDirective() {
            var runtimeEditorLoadDeferred = UtilsService.createPromiseDeferred();
            runtimeEditorReadyDeferred.promise.then(function () {
             
                var runtimeEditorPayload = {
                    selectedValues: fieldValues,
                    dataRecordTypeId: dataRecordTypeId,
                    definitionSettings: definitionSettings,
                    parentFieldValues: fieldValues,
                    runtimeEditor: definitionSettings != undefined ? definitionSettings.RuntimeEditor : undefined,
                    isEditMode: false
                };
                VRUIUtilsService.callDirectiveLoad(runtimeEditorAPI, runtimeEditorPayload, runtimeEditorLoadDeferred);
            });

            return runtimeEditorLoadDeferred.promise;
        }

        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                loadAllControls().finally(function () {
                });
            }
            else
                loadAllControls();
        }

        function loadAllControls() {

            function setTitle() {
                if (isEditMode)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(title);
                else
                    $scope.title = UtilsService.buildTitleForAddEditor(title);
            }

            return UtilsService.waitMultipleAsyncOperations([loadEditorRuntimeDirective, setTitle]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function buildDataRowFromScope() {
            var genericBusinessEntity = {};
            var fieldValuesObj = fieldValues != undefined ? fieldValues : {};
            runtimeEditorAPI.setData(fieldValuesObj);
            genericBusinessEntity.Entity = fieldValuesObj;
            return genericBusinessEntity;
        }


        function insertDataRow() {
            $scope.scopeModel.isLoading = true;
            if ($scope.onRowAdded != undefined) {
                $scope.onRowAdded(buildDataRowFromScope());
            }
            $scope.modalContext.closeModal();
            $scope.scopeModel.isLoading = true;
        }

        function updateDataRow() {
            $scope.scopeModel.isLoading = true;
            if ($scope.onRowUpdated != undefined) {
                $scope.onRowUpdated(buildDataRowFromScope());
            }
            $scope.modalContext.closeModal();
            $scope.scopeModel.isLoading = true;

        }

    }
    appControllers.controller('VR_GenericData_GridViewEditorController', gridViewEditorController);
})(appControllers);