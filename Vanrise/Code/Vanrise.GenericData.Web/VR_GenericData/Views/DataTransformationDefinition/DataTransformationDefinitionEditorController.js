(function (appControllers) {

    "use strict";

    DataTransformationDefinitionEditorController.$inject = ['$scope', 'VR_GenericData_DataTransformationDefinitionAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

    function DataTransformationDefinitionEditorController($scope, VR_GenericData_DataTransformationDefinitionAPIService, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

        var isEditMode;
        var dataTransformationDefinitionEntity;
        var dataTransformationDefinitionId;
        var dataRecordTypeAPI;
        var dataRecordTypeReadyPromiseDeferred = UtilsService.createPromiseDeferred();
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

            $scope.scopeModal.onDataRecordTypeDirectiveReady = function (api) {
                dataRecordTypeAPI = api;
                dataRecordTypeReadyPromiseDeferred.resolve();
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
            return UtilsService.waitMultipleAsyncOperations([loadFilterBySection, setTitle, loadDataRecordType])
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
            var obj = dataRecordTypeAPI.getData();
            var dataTransformationDefinition = {
                Name: $scope.scopeModal.name,
                Title: $scope.scopeModal.title,
                DataTransformationDefinitionId: dataTransformationDefinitionId,
                RecordTypes: obj != undefined ? obj.RecordTypes : undefined
            }
            return dataTransformationDefinition;
        }

        function loadDataRecordType() {
            console.log(dataTransformationDefinitionEntity);
            var loadDataRecordTypePromiseDeferred = UtilsService.createPromiseDeferred();

            dataRecordTypeReadyPromiseDeferred.promise
                .then(function () {
                    var directivePayload = (dataTransformationDefinitionEntity != undefined) ? { RecordTypes: dataTransformationDefinitionEntity.RecordTypes } : undefined

                    VRUIUtilsService.callDirectiveLoad(dataRecordTypeAPI, directivePayload, loadDataRecordTypePromiseDeferred);
                });

            return loadDataRecordTypePromiseDeferred.promise;
        }

        function loadFilterBySection() {
            if (dataTransformationDefinitionEntity != undefined) {
                $scope.scopeModal.name = dataTransformationDefinitionEntity.Name;
                $scope.scopeModal.title = dataTransformationDefinitionEntity.Title;
            }
        }

        function setTitle() {
            if (isEditMode && dataTransformationDefinitionEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(dataTransformationDefinitionEntity.Name, 'Data Transformation');
            else
                $scope.title = UtilsService.buildTitleForAddEditor('Data Transformation');
        }

        function insertDataTransformationDefinition() {

            var dataTransformationDefinitionObject = buildDataTransformationDefinitionObjFromScope();
            return VR_GenericData_DataTransformationDefinitionAPIService.AddDataTransformationDefinition(dataTransformationDefinitionObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Data Transformation", response)) {
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
                if (VRNotificationService.notifyOnItemUpdated("Data Transformation", response)) {
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
