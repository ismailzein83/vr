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

            $scope.scopeModal.close = function () {
                $scope.modalContext.closeModal()
            };

            $scope.scopeModal.onRawDataRecordTypeFieldsSelectorReady = function (api) {
                directiveRawReadyAPI = api;
                directiveRawReadyPromiseDeferred.resolve();
            };

            $scope.scopeModal.onSummaryDataRecordTypeFieldsSelectorReady = function (api) {
                directiveSummaryReadyAPI = api;
                directiveSummaryReadyPromiseDeferred.resolve();
            };
        }

        function load() {
            $scope.scopeModal.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadFilterBySection, loadRawFieldMappingSection, loadSummaryDataRecordFieldTypeDirective, setTitle])
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
            if (isEditMode && keyFieldMappingEntity.RawFieldName != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(keyFieldMappingEntity.SummaryFieldName, 'Columns Mapping');
            else
                $scope.title = UtilsService.buildTitleForAddEditor('Columns Mapping');
        }

        function loadRawFieldMappingSection() {
            $scope.scopeModal.rawFieldOptions = [{
                value: "Field",
                text: "Field"
            },
           {
               value: "Expression",
               text: "Expression"
           }];
            if (keyFieldMappingEntity != undefined) {
                if(keyFieldMappingEntity.RawFieldName != undefined)
                    $scope.scopeModal.selectedRawFieldOption = UtilsService.getItemByVal($scope.scopeModal.rawFieldOptions, "Field", "value");
                else
                {
                    $scope.scopeModal.selectedRawFieldOption = UtilsService.getItemByVal($scope.scopeModal.rawFieldOptions, "Expression", "value");
                    $scope.scopeModal.rawExpression = keyFieldMappingEntity.GetRawFieldExpression;
                }

            }
            else
                $scope.scopeModal.selectedRawFieldOption = UtilsService.getItemByVal($scope.scopeModal.rawFieldOptions, "Field", "value");

            var dataRawRecordFieldTypeDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();
            directiveRawReadyPromiseDeferred.promise.then(function () {
                var payload = { dataRecordTypeId: rawDataRecordTypeId };
                if (keyFieldMappingEntity != undefined)
                    payload.selectedIds = keyFieldMappingEntity.RawFieldName;
                VRUIUtilsService.callDirectiveLoad(directiveRawReadyAPI, payload, dataRawRecordFieldTypeDirectiveLoadPromiseDeferred);
            });
            return dataRawRecordFieldTypeDirectiveLoadPromiseDeferred.promise;
        }

        function loadSummaryDataRecordFieldTypeDirective() {
            var dataSummaryRecordFieldTypeDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();
            directiveSummaryReadyPromiseDeferred.promise.then(function () {
                var payload = keyFieldMappingEntity != undefined ? { dataRecordTypeId: summaryDataRecordTypeId, selectedIds: keyFieldMappingEntity.SummaryFieldName } : { dataRecordTypeId: summaryDataRecordTypeId };
                VRUIUtilsService.callDirectiveLoad(directiveSummaryReadyAPI, payload, dataSummaryRecordFieldTypeDirectiveLoadPromiseDeferred);
            });
            return dataSummaryRecordFieldTypeDirectiveLoadPromiseDeferred.promise;
        }

        function buildDataRecordFieldObjectObjFromScope() {
            var item = {};
            item.SummaryFieldName = directiveSummaryReadyAPI.getSelectedIds();
            if ($scope.scopeModal.selectedRawFieldOption.value == "Field")
                item.RawFieldName = directiveRawReadyAPI.getSelectedIds();
            else
                item.GetRawFieldExpression = $scope.scopeModal.rawExpression;
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