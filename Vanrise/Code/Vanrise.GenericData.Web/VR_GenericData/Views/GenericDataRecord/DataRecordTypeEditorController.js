(function (appControllers) {

    "use strict";
    DataRecordTypeEditorController.$inject = ['$scope', 'VR_GenericData_DataRecordTypeAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

    function DataRecordTypeEditorController($scope, VR_GenericData_DataRecordTypeAPIService, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

        var isEditMode;

        var dataRecordTypeEntity;
        var dataRecordTypeId;

        var dataRecordFieldAPI;
        var dataRecordFieldReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                dataRecordTypeId = parameters.DataRecordTypeId;
            }
            isEditMode = (dataRecordTypeId != undefined);
        };

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onDataRecordFieldDirectiveReady = function (api) {
                dataRecordFieldAPI = api;
                dataRecordFieldReadyPromiseDeferred.resolve();
            };

            $scope.hasSaveDataRecordType = function () {
                if (isEditMode) {
                    return VR_GenericData_DataRecordTypeAPIService.HasUpdateDataRecordType();
                }
                else {
                    return VR_GenericData_DataRecordTypeAPIService.HasAddDataRecordType();
                }
            };
            $scope.scopeModel.SaveDataRecordType = function () {
                $scope.scopeModel.isLoading = true;
                if (isEditMode) {
                    return updateDataRecordType();
                }
                else {
                    return insertDataRecordType();
                }
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };
        };

        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                getDataRecordType().then(function () {
                    loadAllControls()
                        .finally(function () {
                            dataRecordTypeEntity = undefined;
                        });
                }).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }
        };

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadFilterBySection, setTitle, loadDataRecordField])
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
               .finally(function () {
                   $scope.scopeModel.isLoading = false;
               });
        };

        function getDataRecordType() {
            return VR_GenericData_DataRecordTypeAPIService.GetDataRecordTypeToEdit(dataRecordTypeId).then(function (dataRecordType) {
                dataRecordTypeEntity = dataRecordType;
            });
        };

        function buildDataRecordTypeObjFromScope() {
            var obj = dataRecordFieldAPI.getData();

            var dataRecordType = {
                Name: $scope.scopeModel.name,
                DataRecordTypeId: dataRecordTypeId,
            };

            if (obj != undefined) {
                dataRecordType.Fields = obj.Fields;
                dataRecordType.ExtraFieldsEvaluator = obj.ExtraFieldsEvaluator;
            }
            return dataRecordType;
        };

        function loadDataRecordField() {
            var loadDataRecordFieldPromiseDeferred = UtilsService.createPromiseDeferred();

            dataRecordFieldReadyPromiseDeferred.promise
                .then(function () {
                    var directivePayload = (dataRecordTypeEntity != undefined) ? { Fields: dataRecordTypeEntity.Fields, ExtraFieldsEvaluator: dataRecordTypeEntity.ExtraFieldsEvaluator } : undefined;

                    VRUIUtilsService.callDirectiveLoad(dataRecordFieldAPI, directivePayload, loadDataRecordFieldPromiseDeferred);
                });

            return loadDataRecordFieldPromiseDeferred.promise;
        };

        function loadFilterBySection() {
            if (dataRecordTypeEntity != undefined) {
                $scope.scopeModel.name = dataRecordTypeEntity.Name;
            }
        };

        function setTitle() {
            if (isEditMode && dataRecordTypeEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(dataRecordTypeEntity.Name, 'Data Record Type');
            else
                $scope.title = UtilsService.buildTitleForAddEditor('Data Record Type');
        };

        function insertDataRecordType() {

            var dataRecordTypeObject = buildDataRecordTypeObjFromScope();
            return VR_GenericData_DataRecordTypeAPIService.AddDataRecordType(dataRecordTypeObject)
            .then(function (response) {
                $scope.scopeModel.isLoading = false;
                if (VRNotificationService.notifyOnItemAdded("Record Type", response)) {
                    if ($scope.onDataRecordTypeAdded != undefined)
                        $scope.onDataRecordTypeAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        };

        function updateDataRecordType() {
            var dataRecordTypeObject = buildDataRecordTypeObjFromScope();
            VR_GenericData_DataRecordTypeAPIService.UpdateDataRecordType(dataRecordTypeObject)
            .then(function (response) {
                $scope.scopeModel.isLoading = false;
                if (VRNotificationService.notifyOnItemUpdated("Record Type", response)) {
                    if ($scope.onDataRecordTypeUpdated != undefined)
                        $scope.onDataRecordTypeUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        };
    }

    appControllers.controller('VR_GenericData_DataRecordTypeEditorController', DataRecordTypeEditorController);
})(appControllers);
