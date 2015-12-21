(function (appControllers) {

    "use strict";

    dataRecordFieldEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'VRCommon_DataRecordFieldAPIService', 'VRValidationService'];

    function dataRecordFieldEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, VRCommon_DataRecordFieldAPIService, VRValidationService) {

        var isEditMode;
        var dataRecordFieldID;
        var dataRecordFieldEntity;
        var dataRecordTypeId;
        var directiveReadyAPI;
        var directiveReadyPromiseDeferred;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {

            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                dataRecordFieldID = parameters.ID;
                dataRecordTypeId = parameters.DataRecordTypeId;
            }
            isEditMode = (dataRecordFieldID != undefined);
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
            if (isEditMode) {
                $scope.title = UtilsService.buildTitleForUpdateEditor("Data Record Field");
                getDataRecordField().then(function () {
                    loadAllControls();
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModal.isLoading = false;
                });

            }
            else {
                $scope.title = UtilsService.buildTitleForAddEditor("Data Record Field");
                loadAllControls();
            }

        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadFilterBySection, loadDataRecordFieldTypeTemplates])
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

        function loadDataRecordFieldTypeTemplates() {
            return VRCommon_DataRecordFieldAPIService.GetDataRecordFieldTypeTemplates().then(function (response) {
                angular.forEach(response, function (item) {
                    $scope.scopeModal.dataRecordFieldTypeTemplates.push(item);
                });

                if (dataRecordFieldEntity != undefined)
                    $scope.scopeModal.selectedDataRecordFieldTypeTemplate = UtilsService.getItemByVal($scope.scopeModal.dataRecordFieldTypeTemplates, dataRecordFieldEntity.Type.ConfigId, "TemplateConfigID");
            });
        }

        function getDataRecordField() {
            return VRCommon_DataRecordFieldAPIService.GetDataRecordField(dataRecordFieldID).then(function (dataRecordField) {
                dataRecordFieldEntity = dataRecordField;
            });
        }

        function buildDataRecordFieldObjectObjFromScope() {
            var dataRecordField = {};
            dataRecordField.Name = $scope.scopeModal.name;
            dataRecordField.Type = directiveReadyAPI.getData();

            dataRecordField.Type.ConfigId = $scope.scopeModal.selectedDataRecordFieldTypeTemplate.TemplateConfigID;
            dataRecordField.DataRecordTypeID = dataRecordFieldEntity != undefined ? dataRecordFieldEntity.DataRecordTypeID : dataRecordTypeId;
            return dataRecordField;
        }

        function insertDataRecordField() {

            var dataRecordFieldObject = buildDataRecordFieldObjectObjFromScope();
            return VRCommon_DataRecordFieldAPIService.AddDataRecordField(dataRecordFieldObject)
                .then(function (response) {
                    if (VRNotificationService.notifyOnItemAdded("Data Record Field", response, "Name")) {
                        if ($scope.onDataRecordFieldAdded != undefined)
                            $scope.onDataRecordFieldAdded(response.InsertedObject);
                        $scope.modalContext.closeModal();
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });

        }

        function updateDataRecordField() {
            var dataRecordFieldObject = buildDataRecordFieldObjectObjFromScope();
            dataRecordFieldObject.ID = dataRecordFieldID;

            return VRCommon_DataRecordFieldAPIService.UpdateDataRecordField(dataRecordFieldObject)
                .then(function (response) {
                    if (VRNotificationService.notifyOnItemUpdated("Data Record Field", response, "Name")) {
                        if ($scope.onDataRecordFieldUpdated != undefined)
                            $scope.onDataRecordFieldUpdated(response.UpdatedObject);
                        $scope.modalContext.closeModal();
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
        }

    }

    appControllers.controller('VRCommon_DataRecordFieldEditorController', dataRecordFieldEditorController);
})(appControllers);