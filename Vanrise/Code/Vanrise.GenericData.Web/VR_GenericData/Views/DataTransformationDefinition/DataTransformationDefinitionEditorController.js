(function (appControllers) {

    "use strict";

    DataTransformationDefinitionEditorController.$inject = ['$scope', 'VR_GenericData_DataTransformationDefinitionAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService','VR_GenericData_DataTransformationStepConfigAPIService','VR_GenericData_DataRecordTypeAPIService'];

    function DataTransformationDefinitionEditorController($scope, VR_GenericData_DataTransformationDefinitionAPIService, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, VR_GenericData_DataTransformationStepConfigAPIService, VR_GenericData_DataRecordTypeAPIService) {

        var isEditMode;
        var dataTransformationDefinitionEntity;
        var dataTransformationDefinitionId;
        var dataRecordTypeAPI;
        var dataRecordTypeReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var editorDirectiveAPI;
        var gridDirectiveAPI;
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

            $scope.scopeModal.steps = [];

            $scope.scopeModal.stepsAdded = [];

            $scope.scopeModal.onGridReady = function (api)
            {
                gridDirectiveAPI = api;
            }

            $scope.scopeModal.addStep = function (dataItem)
            {
                if (UtilsService.getItemIndexByVal($scope.scopeModal.stepsAdded, dataItem.Name, "Name")==-1)
                {
                    dataItem.Context = {
                        getRecordNames: getRecordNames,
                        getRecordFields: getRecordFields
                    }
                    $scope.scopeModal.stepsAdded.push(dataItem);  
                }
                  
            }

            $scope.scopeModal.onEditStepClick = function (dataItem)
            {
                $scope.scopeModal.selectedStep = dataItem;
            }

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

            $scope.scopeModal.onEditorDirectiveReady=function(api)
            {
                editorDirectiveAPI = api;
                var setLoader = function (value) {
                    $scope.scopeModal.isLoadingDirective = value;
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope.scopeModal, editorDirectiveAPI, $scope.scopeModal.selectedStep, setLoader);
            }
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
            return UtilsService.waitMultipleAsyncOperations([loadFilterBySection, setTitle, loadDataRecordType, loadAllStepts])
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
                RecordTypes: obj != undefined ? obj.RecordTypes : undefined,
                MappingSteps: editorDirectiveAPI.getData()
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

        function loadAllStepts()
        {
            return VR_GenericData_DataTransformationStepConfigAPIService.GetDataTransformationSteps().then(function (response) {
                for(var i=0;i<response.length;i++)
                {
                    $scope.scopeModal.steps.push(response[i]);
                }
            });
        }

        function setTitle() {
            if (isEditMode && dataTransformationDefinitionEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(dataTransformationDefinitionEntity.Name, 'Data Transformation');
            else
                $scope.title = UtilsService.buildTitleForAddEditor('Data Transformation');
        }

        function getRecordFields(typeName)
        {
            var dataRecordTypes = dataRecordTypeAPI.getData();
            var loadPromiseDeferred = UtilsService.createPromiseDeferred();
            if (dataRecordTypes != undefined)
            {
                var recordType = UtilsService.getItemByVal(dataRecordTypes.RecordTypes, typeName, 'key');
                if (recordType != undefined)
                {
                    VR_GenericData_DataRecordTypeAPIService.GetDataRecordType(recordType.value).then(function (response) {
                        if (response)
                            loadPromiseDeferred.resolve(response.Fields);
                    }).catch(function (error) {
                        loadPromiseDeferred.reject(error);
                        VRNotificationService.notifyException(error, $scope);
                    });
                }
            } else
            {
                loadPromiseDeferred.resolve();
            }
            return loadPromiseDeferred.promise;
        }

        function getRecordNames()
        {
            var obj = dataRecordTypeAPI.getData();
            var recordTypeNames = [];
            for (var i = 0; i < obj.RecordTypes.length;i++)
            {
                recordTypeNames.push({Name:obj.RecordTypes[i].key});
            }
            return recordTypeNames;
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
