﻿(function (appControllers) {

    "use strict";

    dataRecordFieldEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'VR_GenericData_DataRecordTypeAPIService', 'VRValidationService'];

    function dataRecordFieldEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, VR_GenericData_DataRecordTypeAPIService, VRValidationService) {

        var isEditMode;
        var dataRecordFieldEntity;
        var existingFields;
        var showDependantFieldsGrid;

        var directiveReadyAPI;
        var directiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var dataRecordTypeFieldsFormulaAPI;
        var dataRecordTypeFieldsFormulaReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var dataRecordFieldAPI;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                dataRecordFieldEntity = parameters.Formula;
                existingFields = parameters.ExistingFields;
                showDependantFieldsGrid = parameters.showDependantFieldsGrid;
            }

            isEditMode = (dataRecordFieldEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModal = {};

            $scope.scopeModal.SaveDataRecordField = function () {
                    return updateDataRecordField();

            };

            $scope.scopeModal.onDataRecordTypeFieldsFormulaReady = function (api) {
                dataRecordTypeFieldsFormulaAPI = api;
                dataRecordTypeFieldsFormulaReadyPromiseDeferred.resolve();
            };
            $scope.scopeModal.validateName = function () {
                return validateName();
            };

            $scope.scopeModal.validateTitle = function () {
                return validateTitle();
            };

            $scope.scopeModal.close = function () {
                $scope.modalContext.closeModal();
            };

            $scope.scopeModal.onDirectiveReady = function (api) {
                directiveReadyAPI = api;
                directiveReadyPromiseDeferred.resolve();
            };
        }

        function load() {
            $scope.scopeModal.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle,loadDataRecordTypeFieldsFormulaDirective])
                    .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
                    .finally(function () {
                    $scope.scopeModal.isLoading = false;
                });
        }

        function setTitle() {
            if (isEditMode && dataRecordFieldEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(dataRecordFieldEntity.Name, 'Data Record Field');
            else
                $scope.title = UtilsService.buildTitleForAddEditor('Data Record Field');
        }



        function loadDataRecordTypeFieldsFormulaDirective() {
            var dataRecordTypeFieldsFormulaDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            dataRecordTypeFieldsFormulaReadyPromiseDeferred.promise.then(function () {
                var dataRecordTypeFieldsFormulaPayload = { formula: dataRecordFieldEntity, context: getContext() };

                VRUIUtilsService.callDirectiveLoad(dataRecordTypeFieldsFormulaAPI, dataRecordTypeFieldsFormulaPayload, dataRecordTypeFieldsFormulaDirectiveLoadPromiseDeferred);
            });
            return dataRecordTypeFieldsFormulaDirectiveLoadPromiseDeferred.promise;
        }

        function getContext() {
            var context = {
                getFields: function () {
                    var fields = [];
                    if (existingFields != undefined) {
                        for (var i = 0; i < existingFields.length; i++) {
                            var existingField = existingFields[i];
                            fields.push({ fieldName: existingField.Name, fieldTitle: existingField.Title, fieldType: existingField.Type });
                        }
                    }
                    return fields;
                }
            };
            return context;
        }

        function validateName() {
            if (isEditMode && $scope.scopeModal.name == dataRecordFieldEntity.Name)
                return null;
            else if (UtilsService.getItemIndexByVal(existingFields, $scope.scopeModal.name, 'Name') != -1)
                return 'Same name exists.';
            return null;
        }

        function validateTitle() {
            if (isEditMode && $scope.scopeModal.title == dataRecordFieldEntity.Title)
                return null;
            else if (UtilsService.getItemIndexByVal(existingFields, $scope.scopeModal.title, 'Title') != -1)
                return 'Same title exists.';
            return null;
        }

        function buildDataRecordFieldObjectObjFromScope() {
            var dataRecordField = {};

            dataRecordField.Formula = dataRecordTypeFieldsFormulaAPI != undefined ? dataRecordTypeFieldsFormulaAPI.getData() : undefined;
            return dataRecordField;
        }

        function insertDataRecordField() {

            var dataRecordFieldObject = buildDataRecordFieldObjectObjFromScope();
			if ($scope.onDataRecordFieldAdded != undefined) {
				$scope.onDataRecordFieldAdded(dataRecordFieldObject);
			}
            $scope.modalContext.closeModal();
        }

		function updateDataRecordField() {
			var dataRecordFieldObject = buildDataRecordFieldObjectObjFromScope();
			if ($scope.onDataRecordFieldUpdated != undefined)
				$scope.onDataRecordFieldUpdated(dataRecordFieldObject);
			$scope.modalContext.closeModal();
		}
    }

    appControllers.controller('VR_GenericData_DataRecordFieldEditorController', dataRecordFieldEditorController);
})(appControllers);