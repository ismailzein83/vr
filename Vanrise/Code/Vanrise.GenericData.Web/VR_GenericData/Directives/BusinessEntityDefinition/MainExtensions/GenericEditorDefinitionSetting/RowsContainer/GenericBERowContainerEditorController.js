//(function (appControllers) {

//    "use strict";

//    GenericRowContainerEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'VR_GenericData_ContainerTypeEnum'];

//    function GenericRowContainerEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, VR_GenericData_ContainerTypeEnum) {

//        var isEditMode;
//        var recordTypeFields;
//        var rowContainerEntity;
//        var context;

//        loadParameters();
//        defineScope();
//        load();

//        function loadParameters() {
//            var parameters = VRNavigationService.getParameters($scope);
//            if (parameters != undefined && parameters != null) {
//                recordTypeFields = parameters.recordTypeFields;
//                context = parameters.context;
//                rowContainerEntity = parameters.rowContainerEntity;
//            }
//            isEditMode = (rowContainerEntity != undefined);
//        }

//        function defineScope() {
//            $scope.scopeModel = {};

//            $scope.scopeModel.addedFields = [];

//            $scope.scopeModel.addRowContainer = function () {
//                var fieldEditorDefinitionLoadPromiseDeferred = UtilsService.createPromiseDeferred();
//                var field = prepareFieldData(undefined, fieldEditorDefinitionLoadPromiseDeferred);
//                field.expanded = true;
//                $scope.scopeModel.addedFields.push(field);
//            };

//            $scope.scopeModel.isValid = function () {
//                if ($scope.scopeModel.addedFields == undefined || $scope.scopeModel.addedFields.length == 0)
//                    return "You Should add at least one Field Type.";

//                return null;
//            };

//            $scope.scopeModel.onRemoveField = function (dataItem) {
//                if ($scope.scopeModel.addedFields.length == 1) {
//                    VRNotificationService.showWarning("At least one Field Type should exist.");
//                    return;
//                }

//                var index = $scope.scopeModel.addedFields.indexOf(dataItem);
//                if (index != -1)
//                    $scope.scopeModel.addedFields.splice(index, 1);
//            };

//            $scope.scopeModel.saveRowContainer = function () {
//                if (isEditMode) {
//                    return updateRow();
//                }
//                else {
//                    return insertRow();
//                }
//            };

//            $scope.scopeModel.close = function () {
//                $scope.modalContext.closeModal();
//            };
//        }

//        function load() {
//            $scope.scopeModel.isLoading = true;
//            loadAllControls();
//        }

//        function loadAllControls() {
//            return UtilsService.waitMultipleAsyncOperations([loadRowContainerFields, setTitle])
//                .catch(function (error) {
//                    VRNotificationService.notifyExceptionWithClose(error, $scope);
//                })
//                .finally(function () {
//                    $scope.scopeModel.isLoading = false;
//                });

//            function setTitle() {
//                if (isEditMode)
//                    $scope.title = UtilsService.buildTitleForUpdateEditor('Row Container');
//                else
//                    $scope.title = UtilsService.buildTitleForAddEditor('Row Container');
//            }

//            function loadRowContainerFields() {
//                var promises = [];
//                var rootPromiseNode = {
//                    promises: promises
//                };

//                if (rowContainerEntity != undefined) {

//                    for (var i = 0; i < rowContainerEntity.length; i++) {
//                        var currentField = rowContainerEntity[i];
//                        var fieldEditorDefinitionLoadPromiseDeferred = UtilsService.createPromiseDeferred();
//                        promises.push(fieldEditorDefinitionLoadPromiseDeferred.promise);
//                        var fieldData = prepareFieldData(currentField, fieldEditorDefinitionLoadPromiseDeferred);
//                        fieldData.expanded = false;
//                        $scope.scopeModel.addedFields.push(fieldData);
//                    }
//                }
//                else {
//                    $scope.scopeModel.addRowContainer();
//                }

//                return UtilsService.waitPromiseNode(rootPromiseNode);
//            }
//        }

//        function prepareFieldData(field, fieldEditorDefinitionLoadPromiseDeferred) {
//            if (field == undefined)
//                field = {};

//            var fieldData = {
//                directiveAPI: undefined,
//                fieldEditorDefinitionReadyPromiseDeferred: UtilsService.createPromiseDeferred()
//            };

//            fieldData.onGenericBEFieldEditorDefinitionDirectiveReady = function (api) {
//                fieldData.directiveAPI = api;
//                fieldData.fieldEditorDefinitionReadyPromiseDeferred.resolve();
//            };

//            fieldData.fieldEditorDefinitionReadyPromiseDeferred.promise.then(function () {
//                var payload = {
//                    settings: field,
//                    context: context,
//                    containerType: VR_GenericData_ContainerTypeEnum.Row.value
//                };

//                VRUIUtilsService.callDirectiveLoad(fieldData.directiveAPI, payload, fieldEditorDefinitionLoadPromiseDeferred);
//            });

//            fieldData.onFieldEditorDefinitionSelectionChanged = function (selectedFieldEditor) {
//                fieldData.title = "";
//                if (selectedFieldEditor == undefined)
//                    return; 

//                fieldData.title = selectedFieldEditor.Title;
//            };

//            return fieldData;
//        }

//        function buildRowSettingsObject() {
//            var rowSettingsObject = [];
//            for (var i = 0; i < $scope.scopeModel.addedFields.length; i++) {
//                var currentField = $scope.scopeModel.addedFields[i];
//                rowSettingsObject.push(currentField.directiveAPI.getData());
//            }

//            rowSettingsObject.numberOfFields = rowSettingsObject.length + " Field Types";

//            return rowSettingsObject;
//        }

//        function insertRow() {
//            var rowSettingsObject = buildRowSettingsObject();
//            if ($scope.onRowContainerAdded != undefined)
//                $scope.onRowContainerAdded(rowSettingsObject);
//            $scope.modalContext.closeModal();
//        }

//        function updateRow() {
//            var rowSettingsObject = buildRowSettingsObject();
//            if ($scope.onRowContainerUpdated != undefined)
//                $scope.onRowContainerUpdated(rowSettingsObject);
//            $scope.modalContext.closeModal();
//        }

//    }

//    appControllers.controller('VR_GenericData_GenericRowContainerEditorController', GenericRowContainerEditorController);
//})(appControllers);