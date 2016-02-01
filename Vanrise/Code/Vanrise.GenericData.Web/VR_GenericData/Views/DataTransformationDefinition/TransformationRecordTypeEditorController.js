(function (appControllers) {

    "use strict";

    TransformationRecordTypeEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'VR_GenericData_DataRecordTypeAPIService', 'VRValidationService'];

    function TransformationRecordTypeEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, VR_GenericData_DataRecordTypeAPIService, VRValidationService) {

        var isEditMode;
        var dataRecordTypeEntity;
        var existingTypes;

        var directiveReadyAPI;
        var directiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var dataRecordTypeAPI;
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                dataRecordTypeEntity = parameters.DataRecordType;
                existingTypes = parameters.ExistingTypes;
            }
            isEditMode = (dataRecordTypeEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModal = {};

            $scope.scopeModal.SaveDataRecordType = function () {
                if (isEditMode) {
                    return updateDataRecordType();
                }
                else {
                    return insertDataRecordType();
                }
            };

            $scope.scopeModal.validateName = function () {
                return validateName();
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
            return UtilsService.waitMultipleAsyncOperations([loadFilterBySection, loadDataRecordTypeSelector, setTitle])
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
                .finally(function () {
                    $scope.scopeModal.isLoading = false;
                });
        }

        function loadFilterBySection() {
            if (dataRecordTypeEntity != undefined) {
                $scope.scopeModal.name = dataRecordTypeEntity.Name;
            }
        }
        function setTitle() {
            if (isEditMode && dataRecordTypeEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(dataRecordTypeEntity.Name, 'Data Record Type');
            else
                $scope.title = UtilsService.buildTitleForAddEditor('Data Record Type');
        }
        function loadDataRecordTypeSelector() {
            var loadDataRecordTypePromiseDeferred = UtilsService.createPromiseDeferred();

            directiveReadyPromiseDeferred.promise
                .then(function () {
                    var directivePayload = (dataRecordTypeEntity != undefined) ? { Types: dataRecordTypeEntity.Types } : undefined
                    VRUIUtilsService.callDirectiveLoad(directiveReadyAPI, directivePayload, loadDataRecordTypePromiseDeferred);
                });

            return loadDataRecordTypePromiseDeferred.promise;
        }

        function validateName() {
            if (isEditMode && $scope.scopeModal.name == dataRecordTypeEntity.Name)
                return null;
            else if (UtilsService.getItemIndexByVal(existingTypes, $scope.scopeModal.name, 'Name') != -1)
                return 'Same Name Exist.';
            return null;
        }

        function buildDataRecordTypeObjectObjFromScope() {
            var dataRecordType = {};
            dataRecordType.key = $scope.scopeModal.name;
            dataRecordType.value = $scope.scopeModal.selectedRecordType != undefined? $scope.scopeModal.selectedRecordType.DataRecordTypeId:undefined
            return dataRecordType;
        }

        function insertDataRecordType() {

            var dataRecordTypeObject = buildDataRecordTypeObjectObjFromScope();
            //  if (VRNotificationService.notifyOnItemAdded("Data Record Type",undefined, "Name")) {
            if ($scope.onDataRecordTypeAdded != undefined)
                $scope.onDataRecordTypeAdded(dataRecordTypeObject);
            $scope.modalContext.closeModal();
            //   }

        }

        function updateDataRecordType() {
            var dataRecordTypeObject = buildDataRecordTypeObjectObjFromScope();
            //    if (VRNotificationService.notifyOnItemUpdated("Data Record Type", undefined, "Name")) {
            if ($scope.onDataRecordTypeUpdated != undefined)
                $scope.onDataRecordTypeUpdated(dataRecordTypeObject);
            $scope.modalContext.closeModal();
            // }
        }

    }

    appControllers.controller('VR_GenericData_TransformationRecordTypeEditorController', TransformationRecordTypeEditorController);
})(appControllers);