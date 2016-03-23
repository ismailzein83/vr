(function (appControllers) {

    "use strict";

    summaryGroupingColumnsEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'VR_GenericData_DataRecordTypeAPIService', 'VRValidationService'];

    function summaryGroupingColumnsEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, VR_GenericData_DataRecordTypeAPIService, VRValidationService) {

        var isEditMode;
        var keyFieldMappingEntity;
        var existingFields;

        var directiveRawReadyAPI;
        var directiveRawReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var directiveSummaryReadyAPI;
        var directiveSummaryReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var rawDataRecordTypeId;
        var summaryDataRecordTypeId;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                keyFieldMappingEntity = parameters.KeyFieldMapping;
                existingFields = parameters.ExistingFields;
                rawDataRecordTypeId = parameters.RawDataRecordTypeId;
                summaryDataRecordTypeId = parameters.SummaryDataRecordTypeId;
            }
            console.log('parameters')
            console.log(parameters)
            isEditMode = (keyFieldMappingEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModal = {};
            $scope.selectedRawDataRecordTypeFields;
            $scope.selectedSummaryDataRecordTypeFields;

            $scope.scopeModal.SaveColumnGroup = function () {
                if (isEditMode) {
                    return updateColumnGroup();
                }
                else {
                    return insertColumnGroup();
                }
            };

            $scope.scopeModal.validateName = function () {
                return validateName();
            }

            $scope.scopeModal.close = function () {
                $scope.modalContext.closeModal()
            };

            $scope.scopeModal.onRawDataRecordTypeFieldsSelectorReady = function (api) {
                directiveRawReadyAPI = api;
                directiveRawReadyPromiseDeferred.resolve();
            }

            $scope.scopeModal.onSummaryDataRecordTypeFieldsSelectorReady = function (api) {
                console.log('api')
                console.log(api)
                directiveSummaryReadyAPI = api;
                directiveSummaryReadyPromiseDeferred.resolve();
            }
        }

        function load() {
            $scope.scopeModal.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadFilterBySection, loadRawDataRecordFieldTypeDirective, loadSummaryDataRecordFieldTypeDirective, setTitle])
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
                .finally(function () {
                    $scope.scopeModal.isLoading = false;
                });
        }

        function loadFilterBySection() {
            if (keyFieldMappingEntity != undefined) {
            }
        }
        function setTitle() {
            if (isEditMode && keyFieldMappingEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor('', 'Columns Mapping');
            else
                $scope.title = UtilsService.buildTitleForAddEditor('Columns Mapping');
        }

        function loadRawDataRecordFieldTypeDirective() {
            var dataRawRecordFieldTypeDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();
            directiveRawReadyPromiseDeferred.promise.then(function () {
                var payload = keyFieldMappingEntity != undefined ? { dataRecordTypeId: rawDataRecordTypeId, selectedIds: keyFieldMappingEntity.RawFieldName } : { dataRecordTypeId: rawDataRecordTypeId };
                console.log('keyFieldMappingEntity')
                console.log(keyFieldMappingEntity)
                VRUIUtilsService.callDirectiveLoad(directiveRawReadyAPI, payload, dataRawRecordFieldTypeDirectiveLoadPromiseDeferred);
            });
            return dataRawRecordFieldTypeDirectiveLoadPromiseDeferred.promise;
        }

        function loadSummaryDataRecordFieldTypeDirective() {
            var dataSummaryRecordFieldTypeDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();
            directiveSummaryReadyPromiseDeferred.promise.then(function () {
                var payload = keyFieldMappingEntity != undefined ? { dataRecordTypeId: summaryDataRecordTypeId, selectedIds: keyFieldMappingEntity.SummaryFieldName } : { dataRecordTypeId: summaryDataRecordTypeId };
                console.log('keyFieldMappingEntity')
                console.log(keyFieldMappingEntity)
                VRUIUtilsService.callDirectiveLoad(directiveSummaryReadyAPI, payload, dataSummaryRecordFieldTypeDirectiveLoadPromiseDeferred);
            });
            return dataSummaryRecordFieldTypeDirectiveLoadPromiseDeferred.promise;
        }

        function validateName() {
            if (isEditMode && $scope.scopeModal.name == keyFieldMappingEntity.Name)
                return null;
            else if (UtilsService.getItemIndexByVal(existingFields, $scope.scopeModal.name, 'Name') != -1)
                return 'Same Name Exist.';
            return null;
        }

        function buildDataRecordFieldObjectObjFromScope() {
            var item = {};
            item.RawFieldName = directiveRawReadyAPI.getSelectedIds();
            item.SummaryFieldName = directiveSummaryReadyAPI.getSelectedIds();
            return item;
        }

        function insertColumnGroup() {
            var itemAdded = buildDataRecordFieldObjectObjFromScope();
            if ($scope.onDataItemAdded != undefined)
                $scope.onDataItemAdded(itemAdded);
            $scope.modalContext.closeModal();
        }

        function updateColumnGroup() {
            var itemUpdated = buildDataRecordFieldObjectObjFromScope();
            if ($scope.onDataItemUpdated != undefined)
                $scope.onDataItemUpdated(itemUpdated);
            $scope.modalContext.closeModal();
        }

    }

    appControllers.controller('VR_GenericData_SummaryGroupingColumnsEditorController', summaryGroupingColumnsEditorController);
})(appControllers);