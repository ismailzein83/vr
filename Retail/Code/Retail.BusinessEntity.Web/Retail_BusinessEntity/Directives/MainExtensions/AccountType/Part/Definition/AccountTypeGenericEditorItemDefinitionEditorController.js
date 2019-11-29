
(function (appControllers) {

    'use strict';

    AccountTypegenericEditorArrayItemDefinitionEditorController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService', 'VR_GenericData_DataRecordFieldAPIService'];

    function AccountTypegenericEditorArrayItemDefinitionEditorController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, VR_GenericData_DataRecordFieldAPIService) {
        var isEditMode;

        var dataRecordTypeId;
        var genericEditorArrayItem;
        var genericEditorEntity;
        var title;

        var editorDefinitionSettings;
        var dataRecordTypeFields;

        var genericBEEditorDefinitionDirectiveAPI;
        var genericBEEditorDefinitionReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var context;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                dataRecordTypeId = parameters.dataRecordTypeId;
                genericEditorArrayItem = parameters.item;
                if (genericEditorArrayItem != undefined) {
                    genericEditorEntity = genericEditorArrayItem.Entity;
                    if (genericEditorEntity != undefined) {
                        editorDefinitionSettings = genericEditorEntity.Settings;
                        title = genericEditorEntity.Title;
                    }
                }
            }
            isEditMode = (genericEditorArrayItem != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onGenericBEEditorDefinitionDirectiveReady = function (api) {
                genericBEEditorDefinitionDirectiveAPI = api;
                genericBEEditorDefinitionReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateGenericEditorArrayItem() : addGenericEditorArrayItem();
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
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadGenericEditorDefinition]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function loadStaticData() {
            if (genericEditorArrayItem != undefined) {
                $scope.scopeModel.title = title;
            }
        }

        function setTitle() {
            if (isEditMode) {
                $scope.title = UtilsService.buildTitleForUpdateEditor(title, 'Generic Editor Definition');
            }
            else {
                $scope.title = UtilsService.buildTitleForAddEditor('Generic Editor Definition');
            }
        }

        function updateGenericEditorArrayItem() {
            genericEditorArrayItem.Entity = buildgenericEditorArrayItemObjFromScope();
            $scope.onAccountPartGenericEditorItemDefinitionUpdated(genericEditorArrayItem);
            $scope.modalContext.closeModal();
        }

        function addGenericEditorArrayItem() {
            var genericEditorArrayItemToAdd = buildgenericEditorArrayItemObjFromScope();
            $scope.onAccountPartGenericEditorItemDefinitionAdded(genericEditorArrayItemToAdd);
            $scope.modalContext.closeModal();
        }

        function buildgenericEditorArrayItemObjFromScope() {
            return {
                Title: $scope.scopeModel.title,
                Settings: genericBEEditorDefinitionDirectiveAPI.getData()
            };
        }

        function loadGenericEditorDefinition() {
            var loadGenericPromiseDeferred = UtilsService.createPromiseDeferred();
            UtilsService.waitMultiplePromises([genericBEEditorDefinitionReadyPromiseDeferred.promise, getContext()]).then(function () {
                var directivePayload = {
                    settings: editorDefinitionSettings,
                    context: context
                };
                VRUIUtilsService.callDirectiveLoad(genericBEEditorDefinitionDirectiveAPI, directivePayload, loadGenericPromiseDeferred);
            });

            return loadGenericPromiseDeferred.promise;
        }

        function getContext() {
            return getDataRecordFieldsInfo().then(function () {
                buildContext();
            });
        }

        function getDataRecordFieldsInfo() {
            return VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldsInfo(dataRecordTypeId).then(function (response) {
                dataRecordTypeFields = [];
                if (response != undefined) {
                    for (var i = 0; i < response.length; i++) {
                        var currentField = response[i];
                        dataRecordTypeFields.push(currentField.Entity);
                    }
                }
            });
        }

        function buildContext() {
            if (context == undefined) {
                context = {};
            }
            context.getRecordTypeFields = function () {
                var data = [];
                for (var i = 0; i < dataRecordTypeFields.length; i++) {
                    data.push(dataRecordTypeFields[i]);
                }
                return data;
            };
            context.getDataRecordTypeId = function () {
                return dataRecordTypeId;
            };
            context.getFieldType = function (fieldName) {
                for (var i = 0; i < dataRecordTypeFields.length; i++) {
                    var field = dataRecordTypeFields[i];
                    if (field.Name == fieldName)
                        return field.Type;
                }
            };
            context.getFields = function () {
                var dataFields = [];
                for (var i = 0; i < dataRecordTypeFields.length; i++) {
                    dataFields.push({
                        FieldName: dataRecordTypeFields[i].Name,
                        FieldTitle: dataRecordTypeFields[i].Title,
                        Type: dataRecordTypeFields[i].Type
                    });
                }
                return dataFields;
            };
            context.getFilteredFields = function () {
                var data = [];
                var filterData = currentContext.getRecordTypeFields();
                for (var i = 0; i < filterData.length; i++) {
                    var fieldData = filterData[i];
                    data.push({ FieldPath: fieldData.Name, FieldTitle: fieldData.Title });
                }
                return data;
            };
        }

    }

    appControllers.controller('Retail_BE_AccountTypeGenericEditorItemDefinitionEditorController', AccountTypegenericEditorArrayItemDefinitionEditorController);

})(appControllers);