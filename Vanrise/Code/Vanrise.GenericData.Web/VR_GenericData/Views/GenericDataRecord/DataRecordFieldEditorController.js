(function (appControllers) {

    "use strict";

    dataRecordFieldEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'VR_GenericData_DataRecordTypeAPIService', 'VRValidationService'];

    function dataRecordFieldEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, VR_GenericData_DataRecordTypeAPIService, VRValidationService) {

        var isEditMode;
        var dataRecordFieldEntity;
        var existingFields;

        var directiveReadyAPI;
        var directiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var dataRecordFieldAPI;
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                dataRecordFieldEntity = parameters.DataRecordField;
                existingFields = parameters.ExistingFields;
            }
            isEditMode = (dataRecordFieldEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModal = {};

            $scope.scopeModal.SaveDataRecordField = function () {
                if (isEditMode) {
                    return updateDataRecordField();
                }
                else {
                    return insertDataRecordField();
                }
            };

            $scope.scopeModal.validateName = function () {
                return validateName();
            }

            $scope.scopeModal.validateTitle = function () {
                return validateTitle();
            }


            $scope.scopeModal.close = function () {
                $scope.modalContext.closeModal()
            };

            $scope.scopeModal.onDirectiveReady = function (api) {
                directiveReadyAPI = api;
                directiveReadyPromiseDeferred.resolve();
            }
        }

        function load() {
            $scope.scopeModal.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadFilterBySection, loadDataRecordFieldTypeDirective, setTitle])
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
                .finally(function () {
                    $scope.scopeModal.isLoading = false;
                });
        }

        function loadFilterBySection() {
            if (dataRecordFieldEntity != undefined) {
                $scope.scopeModal.name = dataRecordFieldEntity.Name;
                $scope.scopeModal.title = dataRecordFieldEntity.Title;
            }
        }
        function setTitle() {
            if (isEditMode && dataRecordFieldEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(dataRecordFieldEntity.Name, 'Data Record Field');
            else
                $scope.title = UtilsService.buildTitleForAddEditor('Data Record Field');
        }

        function loadDataRecordFieldTypeDirective() {
            var dataRecordFieldTypeDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            directiveReadyPromiseDeferred.promise.then(function () {
                var dataRecordFieldTypePayload = dataRecordFieldEntity != undefined ? dataRecordFieldEntity.Type : undefined;

                VRUIUtilsService.callDirectiveLoad(directiveReadyAPI, dataRecordFieldTypePayload, dataRecordFieldTypeDirectiveLoadPromiseDeferred);
            });
            return dataRecordFieldTypeDirectiveLoadPromiseDeferred.promise;
        }

        function validateName() {
            if (isEditMode && $scope.scopeModal.name == dataRecordFieldEntity.Name)
                return null;
            else if (UtilsService.getItemIndexByVal(existingFields, $scope.scopeModal.name, 'Name') != -1)
                return 'Same Name Exist.';
            return null;
        }

        function validateTitle() {
            if (isEditMode && $scope.scopeModal.title == dataRecordFieldEntity.Title)
                return null;
            else if (UtilsService.getItemIndexByVal(existingFields, $scope.scopeModal.title, 'Title') != -1)
                return 'Same Title Exist.';
            return null;
        }


        function buildDataRecordFieldObjectObjFromScope() {
            var dataRecordField = {};
            dataRecordField.Name = $scope.scopeModal.name;
            dataRecordField.Title = $scope.scopeModal.title;
            dataRecordField.Type = directiveReadyAPI.getData();
            return dataRecordField;
        }

        function insertDataRecordField() {

            var dataRecordFieldObject = buildDataRecordFieldObjectObjFromScope();
            //  if (VRNotificationService.notifyOnItemAdded("Data Record Field",undefined, "Name")) {
            if ($scope.onDataRecordFieldAdded != undefined)
                $scope.onDataRecordFieldAdded(dataRecordFieldObject);
            $scope.modalContext.closeModal();
            //   }

        }

        function updateDataRecordField() {
            var dataRecordFieldObject = buildDataRecordFieldObjectObjFromScope();
            //    if (VRNotificationService.notifyOnItemUpdated("Data Record Field", undefined, "Name")) {
            if ($scope.onDataRecordFieldUpdated != undefined)
                $scope.onDataRecordFieldUpdated(dataRecordFieldObject);
            $scope.modalContext.closeModal();
            // }
        }

    }

    appControllers.controller('VR_GenericData_DataRecordFieldEditorController', dataRecordFieldEditorController);
})(appControllers);