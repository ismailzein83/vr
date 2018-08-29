(function (appControllers) {

    "use strict";

    TransformationRecordTypeEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'VR_GenericData_DataRecordTypeAPIService', 'VRValidationService'];

    function TransformationRecordTypeEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, VR_GenericData_DataRecordTypeAPIService, VRValidationService) {

        var isEditMode;
        var dataRecordTypeEntity;
        var existingTypes;
        var existingRecordNames;
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
                existingRecordNames = [];
                for(var i=0; i<existingTypes.length;i++)
                {
                    existingRecordNames.push({ RecordName: existingTypes[i].RecordName.toLowerCase() });
                }
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
            return UtilsService.waitMultipleAsyncOperations([loadScopeDataFromObj, loadDataRecordTypeSelector, setTitle])
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
                .finally(function () {
                    $scope.scopeModal.isLoading = false;
                });

            function setTitle() {
                if (isEditMode && dataRecordTypeEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(dataRecordTypeEntity.Name, 'Data Record');
                else
                    $scope.title = UtilsService.buildTitleForAddEditor('Data Record');
            }
            function loadScopeDataFromObj() {
                if (dataRecordTypeEntity != undefined) {
                    $scope.scopeModal.name = dataRecordTypeEntity.RecordName;
                    $scope.scopeModal.fullTypeName = dataRecordTypeEntity.FullTypeName;
                    $scope.scopeModal.isArray = dataRecordTypeEntity.IsArray;
                }
            }
            function loadDataRecordTypeSelector() {
                var loadDataRecordTypePromiseDeferred = UtilsService.createPromiseDeferred();

                directiveReadyPromiseDeferred.promise
                    .then(function () {
                        var directivePayload = (dataRecordTypeEntity != undefined) ? { selectedIds: dataRecordTypeEntity.DataRecordTypeId } : undefined;
                        VRUIUtilsService.callDirectiveLoad(directiveReadyAPI, directivePayload, loadDataRecordTypePromiseDeferred);
                    });

                return loadDataRecordTypePromiseDeferred.promise;
            }
        }

        function validateName() {
            if ($scope.scopeModal.name != undefined)
            {
                if (isEditMode && $scope.scopeModal.name.toLowerCase() == dataRecordTypeEntity.RecordName.toLowerCase())
                    return null;
                else if (UtilsService.getItemIndexByVal(existingRecordNames, $scope.scopeModal.name.toLowerCase(), 'RecordName') != -1)
                    return 'Same name exists.';
                return null;
            }
            return null;
        }

        function buildDataRecordTypeObjectObjFromScope() {
            var dataRecordType = {};
            dataRecordType.RecordName = $scope.scopeModal.name;
            if ($scope.scopeModal.selectedRecordType != undefined)
                dataRecordType.DataRecordTypeId = $scope.scopeModal.selectedRecordType.DataRecordTypeId;
            else
                dataRecordType.FullTypeName = $scope.scopeModal.fullTypeName;
            dataRecordType.IsArray = $scope.scopeModal.isArray;
            return dataRecordType;
        }

        function insertDataRecordType() {

            var dataRecordTypeObject = buildDataRecordTypeObjectObjFromScope();
            if ($scope.onDataRecordTypeAdded != undefined)
                $scope.onDataRecordTypeAdded(dataRecordTypeObject);
            $scope.modalContext.closeModal();
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