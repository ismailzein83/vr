(function (appControllers) {

    "use strict";

    GenericRowEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService'];

    function GenericRowEditorController($scope, UtilsService, VRNotificationService, VRNavigationService) {

        var isEditMode;
        var recordTypeFields;
        var rowEntity;
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
                $scope.scopeModal.fields.length = 0;
                for (var i = 0; i < recordTypeFields.length; i++)
                {
                    var field = recordTypeFields[i];
                    $scope.scopeModal.fields.push({ fieldPath: field.FieldPath, fieldTitle: field.FieldPath });
                }
                rowEntity = parameters.rowEntity;
            }
            isEditMode = (rowEntity != undefined);
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
                $scope.modalContext.closeModal()
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
                
                if (rowEntity != undefined && recordTypeFields != undefined) {
                    
                    for (var i = 0; i < rowEntity.length; i++) {
                        var selectedField = rowEntity[i];
                        for (var j = 0; j < recordTypeFields.length; j++)
                        {
                            var field = recordTypeFields[j];
                            if (field.FieldPath == selectedField.FieldPath)
                            {
                                $scope.scopeModal.selectedFields.push({ fieldPath: field.FieldPath, fieldTitle: selectedField.FieldTitle, isRequired: selectedField.IsRequired, isDisabled: selectedField.IsDisabled });
                            }
                        }
                    }

                }
            }
        }

        function buildRowObjectFromScope() {
            var rowObject = [];
            for (var i = 0; i < $scope.scopeModal.selectedFields.length; i++)
            {
                rowObject.push({
                    FieldPath: $scope.scopeModal.selectedFields[i].fieldPath,
                    FieldTitle: $scope.scopeModal.selectedFields[i].fieldTitle,
                    IsRequired: $scope.scopeModal.selectedFields[i].isRequired,
                    IsDisabled: $scope.scopeModal.selectedFields[i].isDisabled,
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