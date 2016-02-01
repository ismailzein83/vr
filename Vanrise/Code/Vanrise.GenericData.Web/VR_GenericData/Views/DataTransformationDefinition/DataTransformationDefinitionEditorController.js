(function (appControllers) {

    "use strict";

    DataTransformationDefinitionEditorController.$inject = ['$scope', 'VR_GenericData_DataTransformationDefinitionAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

    function DataTransformationDefinitionEditorController($scope, VR_GenericData_DataTransformationDefinitionAPIService, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

        var isEditMode;
        var dataTransformationDefinitionEntity;
        var dataTransformationDefinitionId;
        var dataRecordFieldAPI;
        var dataRecordFieldReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        loadParameters();
        defineScope();

        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                dataTransformationDefinitionId = parameters.DataTransformationDefinitionId;
            }
            isEditMode = (dataTransformationDefinitionId != undefined);
        }

        function defineScope() {
            $scope.scopeModal = {}

            $scope.scopeModal.onDataRecordFieldDirectiveReady = function (api) {
                dataRecordFieldAPI = api;
                dataRecordFieldReadyPromiseDeferred.resolve();
            }

            $scope.scopeModal.SaveDataTransformationDefinition = function () {
                if (isEditMode) {
                    return updateDataTransformationDefinition();
                }
                else {
                    return insertDataTransformationDefinition();
                }
            };

            $scope.scopeModal.close = function () {
                $scope.modalContext.closeModal()
            };
        }

        function load() {
            $scope.scopeModal.isLoading = true;

            if (isEditMode) {
                getDataTransformationDefinition().then(function () {
                    loadAllControls()
                        .finally(function () {
                            dataTransformationDefinitionEntity = undefined;
                        });
                }).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModal.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }


        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadFilterBySection, setTitle, loadDataRecordField])
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
               .finally(function () {
                   $scope.scopeModal.isLoading = false;
               });
        }

        function getDataTransformationDefinition() {
            return VR_GenericData_DataTransformationDefinitionAPIService.GetDataTransformationDefinition(dataTransformationDefinitionId).then(function (dataTransformationDefinition) {
                dataTransformationDefinitionEntity = dataTransformationDefinition;
            });
        }

        function buildDataTransformationDefinitionObjFromScope() {
            var obj = dataRecordFieldAPI.getData();
            var dataTransformationDefinition = {
                Name: $scope.scopeModal.name,
                DataTransformationDefinitionId: dataTransformationDefinitionId,
                Fields: obj != undefined ? obj.Fields : undefined
            }
            return dataTransformationDefinition;
        }
        function loadDataRecordField() {

            var loadDataRecordFieldPromiseDeferred = UtilsService.createPromiseDeferred();

            dataRecordFieldReadyPromiseDeferred.promise
                .then(function () {
                    var directivePayload = (dataTransformationDefinitionEntity != undefined) ? { Fields: dataTransformationDefinitionEntity.Fields } : undefined

                    VRUIUtilsService.callDirectiveLoad(dataRecordFieldAPI, directivePayload, loadDataRecordFieldPromiseDeferred);
                });

            return loadDataRecordFieldPromiseDeferred.promise;
        }
        function loadFilterBySection() {
            if (dataTransformationDefinitionEntity != undefined) {
                $scope.scopeModal.name = dataTransformationDefinitionEntity.Name;
            }
        }

        function setTitle() {
            if (isEditMode && dataTransformationDefinitionEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(dataTransformationDefinitionEntity.Name, 'Data Record Type');
            else
                $scope.title = UtilsService.buildTitleForAddEditor('Data Record Type');
        }

        function insertDataTransformationDefinition() {

            var dataTransformationDefinitionObject = buildDataTransformationDefinitionObjFromScope();
            return VR_GenericData_DataTransformationDefinitionAPIService.AddDataTransformationDefinition(dataTransformationDefinitionObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Record Type", response)) {
                    if ($scope.onDataTransformationDefinitionAdded != undefined)
                        $scope.onDataTransformationDefinitionAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });

        }

        function updateDataTransformationDefinition() {
            var dataTransformationDefinitionObject = buildDataTransformationDefinitionObjFromScope();
            VR_GenericData_DataTransformationDefinitionAPIService.UpdateDataTransformationDefinition(dataTransformationDefinitionObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Record Type", response)) {
                    if ($scope.onDataTransformationDefinitionUpdated != undefined)
                        $scope.onDataTransformationDefinitionUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }


    }

    appControllers.controller('VR_GenericData_DataTransformationDefinitionEditorController', DataTransformationDefinitionEditorController);
})(appControllers);
