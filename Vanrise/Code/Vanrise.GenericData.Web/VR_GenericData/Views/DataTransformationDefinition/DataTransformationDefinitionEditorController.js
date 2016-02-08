﻿(function (appControllers) {

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

            $scope.scopeModal.onEditorRemove= function()
            {
                $scope.scopeModal.selectedStep = undefined;
            }

            $scope.scopeModal.steps = [];

            $scope.scopeModal.stepsAdded = [];

            $scope.scopeModal.onGridReady = function (api)
            {
                gridDirectiveAPI = api;
            }

            $scope.scopeModal.addStep = function (dataItem)
            {
                var obj = {
                    Id: $scope.scopeModal.stepsAdded.length + 1,
                    Editor: dataItem.Editor,
                    StepPreviewUIControl: dataItem.StepPreviewUIControl,
                    Title: dataItem.Title,
                    DataTransformationStepConfigId: dataItem.DataTransformationStepConfigId
                }

                obj.onPreviewDirectiveReady = function (api) {
                    obj.previewAPI = api;
                    var setLoader = function (value) {
                        obj.isLoadingPreviewDirective = value;
                    };
                    var payload = {
                        stepDetails: { ConfigId: obj.DataTransformationStepConfigId },
                        Context: getChildrenContext()
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope.scopeModal, obj.previewAPI, payload, setLoader);
                }

                if ($scope.scopeModal.selectedStep != undefined && $scope.scopeModal.selectedStep.previewAPI.isCompositeStep) {                    
                    $scope.scopeModal.selectedStep.previewAPI.addStep(obj);
                }
                else {
                    obj.Context = getChildrenContext();
                    $scope.scopeModal.stepsAdded.push(obj);
                }                  
            }

            $scope.scopeModal.onEditStepClick = function (dataItem)
            {
                if ($scope.scopeModal.selectedStep != undefined)
                {
                    $scope.scopeModal.selectedStep.isSelected = false;
                        dataItem.isSelected= true;
                        applyChanges();
                        $scope.scopeModal.selectedStep = undefined;
                        setTimeout(function () { $scope.scopeModal.selectedStep = dataItem; UtilsService.safeApply($scope)})
                }
                else
                {
                    $scope.scopeModal.selectedStep = dataItem;
                }
               
            }

            $scope.scopeModal.applyChanges = function ()
            {
                applyChanges();
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
                var payload={
                    Context : {
                        getRecordNames: getRecordNames,
                        getRecordFields: getRecordFields
                    },
                    stepDetails:$scope.scopeModal.selectedStep.previewAPI != undefined? $scope.scopeModal.selectedStep.previewAPI.getData():undefined
                }
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope.scopeModal, editorDirectiveAPI, payload, setLoader);
            }

            $scope.scopeModal.onRemoveStep= function(dataItem)
            {
                var index = UtilsService.getItemIndexByVal($scope.scopeModal.stepsAdded, dataItem.Id, 'Id');
                if(index != -1)
                {
                    $scope.scopeModal.stepsAdded.splice(index, 1);
                    if ($scope.scopeModal.selectedStep !=undefined && $scope.scopeModal.selectedStep.Id == dataItem.Id)
                            $scope.scopeModal.selectedStep = undefined;
                }
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

            function loadAllControls() {
                return UtilsService.waitMultipleAsyncOperations([loadFilterBySection, setTitle, loadDataRecordType, loadAllStepts]).then(function () {
                    loadPreviewStepsSection();
                })
                    .catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    })
                   .finally(function () {
                       $scope.scopeModal.isLoading = false;
                   });


                function setTitle() {
                    if (isEditMode && dataTransformationDefinitionEntity != undefined)
                        $scope.title = UtilsService.buildTitleForUpdateEditor(dataTransformationDefinitionEntity.Name, 'Data Transformation');
                    else
                        $scope.title = UtilsService.buildTitleForAddEditor('Data Transformation');
                }
                function loadAllStepts() {
                    return VR_GenericData_DataTransformationStepConfigAPIService.GetDataTransformationSteps().then(function (response) {
                        for (var i = 0; i < response.length; i++) {
                            $scope.scopeModal.steps.push(response[i]);
                        }
                    });
                }
                function loadFilterBySection() {
                    if (dataTransformationDefinitionEntity != undefined) {
                        $scope.scopeModal.name = dataTransformationDefinitionEntity.Name;
                        $scope.scopeModal.title = dataTransformationDefinitionEntity.Title;
                    }
                }
                function loadDataRecordType() {
                    var loadDataRecordTypePromiseDeferred = UtilsService.createPromiseDeferred();

                    dataRecordTypeReadyPromiseDeferred.promise
                        .then(function () {
                            var directivePayload = (dataTransformationDefinitionEntity != undefined) ? { RecordTypes: dataTransformationDefinitionEntity.RecordTypes } : undefined

                            VRUIUtilsService.callDirectiveLoad(dataRecordTypeAPI, directivePayload, loadDataRecordTypePromiseDeferred);
                        });

                    return loadDataRecordTypePromiseDeferred.promise;
                }
                function loadPreviewStepsSection() {
                    var promises = [];
                    if (dataTransformationDefinitionEntity != undefined && dataTransformationDefinitionEntity.MappingSteps != undefined) {

                        for (var i = 0; i < dataTransformationDefinitionEntity.MappingSteps.length; i++) {
                            var payloadEntity = dataTransformationDefinitionEntity.MappingSteps[i];

                            var stepValue = UtilsService.cloneObject(UtilsService.getItemByVal($scope.scopeModal.steps, payloadEntity.ConfigId, "DataTransformationStepConfigId"), true);


                            var newStepObj = {
                                Id:$scope.scopeModal.stepsAdded.length +1,
                                Editor: stepValue.Editor,
                                StepPreviewUIControl: stepValue.StepPreviewUIControl,
                                Title: stepValue.Title,
                                DataTransformationStepConfigId: stepValue.DataTransformationStepConfigId,
                                Context: {
                                    getRecordNames: getRecordNames,
                                    getRecordFields: getRecordFields
                                }
                            }

                            addPreviewAPI(newStepObj, payloadEntity);


                        }
                        function addPreviewAPI(newStepObj, payloadEntity) {
                            newStepObj.onPreviewDirectiveReady = function (api) {
                                newStepObj.previewAPI = api;
                                var setLoader = function (value) {
                                    newStepObj.isLoadingPreviewDirective = value;
                                };
                                var payload = {
                                    Context: {
                                        getRecordNames: getRecordNames,
                                        getRecordFields: getRecordFields
                                    },
                                    stepDetails: payloadEntity
                                }
                                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope.scopeModal, api, payload, setLoader);
                            }
                            $scope.scopeModal.stepsAdded.push(newStepObj);
                        }
                    }

                    // return UtilsService.waitMultiplePromises(promises);
                }
            }
            function getDataTransformationDefinition() {
                return VR_GenericData_DataTransformationDefinitionAPIService.GetDataTransformationDefinition(dataTransformationDefinitionId).then(function (dataTransformationDefinition) {
                    dataTransformationDefinitionEntity = dataTransformationDefinition;
                });
            }
        }

        function getChildrenContext() {
            return {
                getRecordNames: getRecordNames,
                getRecordFields: getRecordFields
            };
        }

        function applyChanges() {
            if ($scope.scopeModal.selectedStep != undefined && $scope.scopeModal.selectedStep.previewAPI != undefined) {
                $scope.scopeModal.selectedStep.previewAPI.applyChanges(editorDirectiveAPI.getData());
                checkValidation();
            }
        }

        function getRecordFields(typeName)
        {
            var dataRecordTypes = dataRecordTypeAPI.getData();
            var loadPromiseDeferred = UtilsService.createPromiseDeferred();
            if (dataRecordTypes != undefined)
            {
                var recordType = UtilsService.getItemByVal(dataRecordTypes.RecordTypes, typeName, 'RecordName');
                if (recordType != undefined)
                {
                    VR_GenericData_DataRecordTypeAPIService.GetDataRecordType(recordType.DataRecordTypeId).then(function (response) {
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

        function checkChangesExist()
        {
            if ($scope.scopeModal.selectedStep != undefined) {
                VRNotificationService.showConfirmation().then(function (response) {
                    if (response) {
                        applyChanges();
                    }
                });
            }
        }

        function checkValidation()
        {
            if ($scope.scopeModal.selectedStep != undefined && $scope.scopeModal.selectedStep.previewAPI != undefined) {
                $scope.scopeModal.selectedStep.validationMessage = $scope.scopeModal.selectedStep.previewAPI.checkValidation();
            }
        }

        function getRecordNames()
        {
            var obj = dataRecordTypeAPI.getData();
            var recordTypeNames = [];
            for (var i = 0; i < obj.RecordTypes.length;i++)
            {
                recordTypeNames.push({ Name: obj.RecordTypes[i].RecordName });
            }
            return recordTypeNames;
        }

        function buildDataTransformationDefinitionObjFromScope() {
            var obj = dataRecordTypeAPI.getData();
            var dataTransformationDefinition = {
                Name: $scope.scopeModal.name,
                Title: $scope.scopeModal.title,
                DataTransformationDefinitionId: dataTransformationDefinitionId,
                RecordTypes: obj != undefined ? obj.RecordTypes : undefined,
                MappingSteps: buildStepsData()
            }

            function buildStepsData() {
                var steps = [];

                for (var i = 0; i < $scope.scopeModal.stepsAdded.length; i++) {
                    if ($scope.scopeModal.stepsAdded[i].previewAPI != undefined)
                        steps.push($scope.scopeModal.stepsAdded[i].previewAPI.getData())
                }
                return steps;
            }

            return dataTransformationDefinition;
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
