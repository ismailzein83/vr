(function (appControllers) {

    "use strict";

    dataRecordFieldEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'VR_GenericData_DataRecordTypeAPIService', 'VRValidationService'];

    function dataRecordFieldEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, VR_GenericData_DataRecordTypeAPIService, VRValidationService) {

        var isEditMode;
        var dataRecordFieldEntity;
        var directiveReadyAPI;
        var directiveReadyPromiseDeferred;

        var dataRecordFieldAPI;
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                dataRecordFieldEntity = parameters.DataRecordFieldEntity;
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



            $scope.scopeModal.close = function () {
                $scope.modalContext.closeModal()
            };

            $scope.scopeModal.onDirectiveReady = function (api) {
                directiveReadyAPI = api;
                var setLoader = function (value) {
                    $scope.isLoadingDirective = value
                };
                var payload;
                if (dataRecordFieldEntity != undefined) {
                    payload = dataRecordFieldEntity.Type;
                }
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveReadyAPI, payload, setLoader, directiveReadyPromiseDeferred);
            }

            $scope.scopeModal.dataRecordFieldTypeTemplates = [];
        }

        function load() {
            $scope.scopeModal.isLoading = true;
                loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadFilterBySection, loadDataRecordFieldTypeTemplates, setTitle])
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
            }
        }
        function setTitle() {
            if (isEditMode && dataRecordFieldEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(dataRecordFieldEntity.Name, 'Data Record Field');
            else
                $scope.title = UtilsService.buildTitleForAddEditor('Data Record Field');
        }
        function loadDataRecordFieldTypeTemplates() {
            return VR_GenericData_DataRecordTypeAPIService.GetDataRecordFieldTypeTemplates().then(function (response) {
                angular.forEach(response, function (item) {
                    $scope.scopeModal.dataRecordFieldTypeTemplates.push(item);
                });

                if (dataRecordFieldEntity != undefined)
                    $scope.scopeModal.selectedDataRecordFieldTypeTemplate = UtilsService.getItemByVal($scope.scopeModal.dataRecordFieldTypeTemplates, dataRecordFieldEntity.Type.ConfigId, "TemplateConfigID");
            });
        }



        function buildDataRecordFieldObjectObjFromScope() {
            var dataRecordField = {};
            dataRecordField.Name = $scope.scopeModal.name;
            dataRecordField.Type = directiveReadyAPI.getData();
            dataRecordField.Type.ConfigId = $scope.scopeModal.selectedDataRecordFieldTypeTemplate.TemplateConfigID;
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
            dataRecordFieldObject.ID = dataRecordFieldID;
        //    if (VRNotificationService.notifyOnItemUpdated("Data Record Field", undefined, "Name")) {
                if ($scope.onDataRecordFieldUpdated != undefined)
                    $scope.onDataRecordFieldUpdated(dataRecordFieldObject);
                $scope.modalContext.closeModal();
           // }
        }

    }

    appControllers.controller('VR_GenericData_DataRecordFieldEditorController', dataRecordFieldEditorController);
})(appControllers);