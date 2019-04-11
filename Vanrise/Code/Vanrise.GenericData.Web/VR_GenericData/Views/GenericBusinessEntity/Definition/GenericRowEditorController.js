(function (appControllers) {

    "use strict";

    GenericRowEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService','VRUIUtilsService'];

    function GenericRowEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

        var isEditMode;
        var recordTypeFields;
        var rowEntity;
        var context;
        $scope.scopeModal = {};
        $scope.scopeModal.fields = [];
        var dataRecordTypeAPI;
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                recordTypeFields = parameters.recordTypeFields;
                context = parameters.context;
                $scope.scopeModal.fields.length = 0;
                for (var i = 0; i < recordTypeFields.length; i++) {
                    var field = recordTypeFields[i];
                    $scope.scopeModal.fields.push(prepareFieldData(field));
                }
                rowEntity = parameters.rowEntity;
            }
            isEditMode = (rowEntity != undefined);
        }

        function prepareFieldData(field) {
            var fieldType = context.getFieldType(field.FieldPath);
            var fieldData = {
                fieldPath: field.FieldPath,
                fieldTitle: field.FieldTitle,
                runtimeViewSettingEditor: fieldType.RuntimeViewSettingEditor,
                directiveAPI: undefined
            };
            if (fieldType.RuntimeViewSettingEditor)
            fieldData.onRuntimeViewSettingsEditorDirectiveReady = function (api) {
                fieldData.directiveAPI = api;
                var setLoader = function (value) { $scope.scopeModal.isRuntimeViewSettingsEditorDirectiveLoading = value; };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, fieldData.directiveAPI, { context: context, dataRecordTypeId: fieldType.DataRecordTypeId }, setLoader);
            };
            return fieldData;
        }
        function defineScope() {
            
            $scope.scopeModal.selectedFields = [];

            $scope.scopeModal.onRemoveField = function (dataItem) {
                var index = $scope.scopeModal.selectedFields.indexOf(dataItem);
                if (index != -1)
                    $scope.scopeModal.selectedFields.splice(index, 1);
            };

            $scope.scopeModal.SaveRow = function () {
                if (isEditMode) {
                    return updateRow();
                }
                else {
                    return insertRow();
                }
            };

            $scope.scopeModal.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            $scope.scopeModal.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadScopeDataFromObj, setTitle])
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
                .finally(function () {
                    $scope.scopeModal.isLoading = false;
                });

            function setTitle() {
                if (isEditMode && rowEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor('Row');
                else
                    $scope.title = UtilsService.buildTitleForAddEditor('Row');
            }
            function loadScopeDataFromObj() {
                var promises=[];
                var rootPromiseNode = {
                    promises: promises
                };
                if (rowEntity != undefined && recordTypeFields != undefined) {
                    
                    for (var i = 0; i < rowEntity.length; i++) {
                        var selectedField = rowEntity[i];
                        for (var j = 0; j < recordTypeFields.length; j++)
                        {
                            var field = recordTypeFields[j];
                            if (field.FieldPath == selectedField.FieldPath)
                            {
                                var fieldInfo = prepareFieldInfo(field, selectedField);
                                if (fieldInfo.runtimeViewSettingEditor!=undefined)
                                    promises.push(fieldInfo.fieldSettingsLoadPromiseDeferred.promise);
                                $scope.scopeModal.selectedFields.push(fieldInfo);
                            }
                        }
                    }

                }
                return UtilsService.waitPromiseNode(rootPromiseNode);
            }
        }
        function prepareFieldInfo(field, selectedField) {
            var fieldType = context.getFieldType(field.FieldPath);
            var fieldInfo = {
                fieldPath: field.FieldPath,
                fieldTitle: field.FieldTitle,
                runtimeViewSettingEditor: fieldType.RuntimeViewSettingEditor,
                fieldSettingsLoadPromiseDeferred: UtilsService.createPromiseDeferred(),
                isRequired: selectedField.IsRequired,
                isDisabled: selectedField.IsDisabled,
                //isDisabledOnEdit: selectedField.IsDisabledOnEdit,
                directiveAPI: undefined
            };
            if (fieldType.RuntimeViewSettingEditor)
            fieldInfo.onRuntimeViewSettingsEditorDirectiveReady = function (api) {
                fieldInfo.directiveAPI = api;
                VRUIUtilsService.callDirectiveLoad(fieldInfo.directiveAPI, {
                    configId: selectedField.FieldViewSettings != undefined ? selectedField.FieldViewSettings.ConfigId : undefined,
                    context: context,
                    settings: selectedField.FieldViewSettings,
                    dataRecordTypeId: fieldType.DataRecordTypeId
                }, fieldInfo.fieldSettingsLoadPromiseDeferred);
            };
            return fieldInfo;
        }
        function buildRowObjectFromScope() {
            var rowObject = [];
            for (var i = 0; i < $scope.scopeModal.selectedFields.length; i++)
            {
                var selectedField = $scope.scopeModal.selectedFields[i];
                rowObject.push({
                    FieldPath: selectedField.fieldPath,
                    FieldTitle: selectedField.fieldTitle,
                    IsRequired: selectedField.isRequired,
                    IsDisabled: selectedField.isDisabled,
                    FieldViewSettings: selectedField.directiveAPI != undefined ? selectedField.directiveAPI.getData() : undefined ,                      
                    //IsDisabledOnEdit: $scope.scopeModal.selectedFields[i].isDisabledOnEdit
                }); 
            }
            return rowObject;
        }

        function insertRow() {

            var rowObject = buildRowObjectFromScope();
            if ($scope.onRowAdded != undefined)
                $scope.onRowAdded(rowObject);
            $scope.modalContext.closeModal();
        }

        function updateRow() {
            var rowObject = buildRowObjectFromScope();
            if ($scope.onRowUpdated != undefined)
                $scope.onRowUpdated(rowObject);
            $scope.modalContext.closeModal();
        }

    }

    appControllers.controller('VR_GenericData_GenericRowEditorController', GenericRowEditorController);
})(appControllers);