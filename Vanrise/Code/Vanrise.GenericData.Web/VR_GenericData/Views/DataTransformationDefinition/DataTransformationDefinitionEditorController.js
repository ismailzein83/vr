(function (appControllers) {

    "use strict";

    DataTransformationDefinitionEditorController.$inject = ['$scope', 'VR_GenericData_DataTransformationDefinitionAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'VR_GenericData_DataRecordTypeAPIService', 'VR_GenericData_DataTransformationDefinitionService'];

    function DataTransformationDefinitionEditorController($scope, VR_GenericData_DataTransformationDefinitionAPIService, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, VR_GenericData_DataRecordTypeAPIService, VR_GenericData_DataTransformationDefinitionService) {

        var isEditMode;
        var dataTransformationDefinitionEntity;
        var dataTransformationDefinitionId;
        var dataRecordTypeAPI;
        var dataRecordTypeReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var editorDirectiveAPI;
        var gridDirectiveAPI;

        var sequenceDirectiveAPI;
        var sequenceReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var savePromiseDeferred = UtilsService.createPromiseDeferred();
        var selectedComposite;

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

            $scope.scopeModal.onEditorRemove = function () {
                $scope.scopeModal.selectedStep.isSelected = false;
                $scope.scopeModal.selectedStep = undefined;
            };

            $scope.scopeModal.stepsDefinition = [];

            $scope.scopeModal.steps = [];

            $scope.scopeModal.onGridReady = function (api) {
                gridDirectiveAPI = api;
            };

            $scope.scopeModal.addStep = function (dataItem) {
                if (selectedComposite != undefined) {
                    checkValidation();
                    selectedComposite.addStep(dataItem);
                }
            };

            $scope.scopeModal.applyChanges = function () {
                applyChanges();
                $scope.scopeModal.selectedStep.isSelected = false;
                $scope.scopeModal.selectedStep = undefined;
            };

            $scope.scopeModal.onDataRecordTypeDirectiveReady = function (api) {
                dataRecordTypeAPI = api;
                dataRecordTypeReadyPromiseDeferred.resolve();
            };

            $scope.scopeModal.SaveDataTransformationDefinition = function () {

            
                
                tryCompile().then(function (response) {
                    if (response) {
                        if (isEditMode) {
                            updateDataTransformationDefinition();
                        }
                        else {
                            insertDataTransformationDefinition();
                        }
                    } else {
                        savePromiseDeferred.resolve();
                    }
                });
                return savePromiseDeferred.promise;
            };

            $scope.hasSaveDataTransformationDefinition = function () {
                if (isEditMode) {
                    return VR_GenericData_DataTransformationDefinitionAPIService.HasUpdateDataTransformationDefinition();
                }
                else {
                    return VR_GenericData_DataTransformationDefinitionAPIService.HasAddDataTransformationDefinition();
                }
            };

            $scope.scopeModal.close = function () {
                $scope.modalContext.closeModal()
            };

            $scope.scopeModal.onEditorDirectiveReady = function (api) {
                editorDirectiveAPI = api;
                var setLoader = function (value) {
                    $scope.scopeModal.isLoadingDirective = value;
                };
                var payload = {
                    context: $scope.scopeModal.selectedStep.context,
                    stepDetails: $scope.scopeModal.selectedStep.previewAPI != undefined ? $scope.scopeModal.selectedStep.previewAPI.getData() : undefined
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope.scopeModal, editorDirectiveAPI, payload, setLoader);
            };

            $scope.scopeModal.onSequenceDirectiveReady = function (api) {
                sequenceDirectiveAPI = api;
                selectedComposite = api;
                selectedComposite.setCheckedSequence(true);
                sequenceReadyPromiseDeferred.resolve();
            };

            $scope.scopeModal.onCompileClick = function () {

                return tryCompile().then(function (response) {
                    if (response)
                        VRNotificationService.showSuccess("All steps compiled successfully.");
                });

            };
            $scope.hasTryCompilePermission = function () {
                return VR_GenericData_DataTransformationDefinitionAPIService.HasTryCompileSteps();
            };

            $scope.scopeModal.refreshEditor = function ()
            {
                refreshEditor();
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
                return UtilsService.waitMultipleAsyncOperations([loadDefinitionSection, setTitle, loadDataRecordType, loadAllStepts]).then(function () {
                    loadSequenceStepSection().catch(function (error) {
                            VRNotificationService.notifyExceptionWithClose(error, $scope);
                        }).finally(function () {
                            $scope.scopeModal.isLoading = false;
                        });
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });


                function setTitle() {
                    if (isEditMode && dataTransformationDefinitionEntity != undefined)
                        $scope.title = UtilsService.buildTitleForUpdateEditor(dataTransformationDefinitionEntity.Name, 'Data Transformation');
                    else
                        $scope.title = UtilsService.buildTitleForAddEditor('Data Transformation');
                }
                function loadAllStepts() {
                    return VR_GenericData_DataTransformationDefinitionAPIService.GetDataTransformationStepConfig().then(function (response) {
                        for (var i = 0; i < response.length; i++) {
                            $scope.scopeModal.stepsDefinition.push(response[i]);
                        }
                    });
                }
                function loadDefinitionSection() {
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
                function loadSequenceStepSection() {
                    var loadSequencePromiseDeferred = UtilsService.createPromiseDeferred();

                    sequenceReadyPromiseDeferred.promise
                        .then(function () {
                            var directivePayload = { context: getChildrenContext() };
                            if (dataTransformationDefinitionEntity != undefined)
                                directivePayload.MappingSteps = dataTransformationDefinitionEntity.MappingSteps;

                            VRUIUtilsService.callDirectiveLoad(sequenceDirectiveAPI, directivePayload, loadSequencePromiseDeferred);
                        });

                    return loadSequencePromiseDeferred.promise;
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
                getRecordFields: getRecordFields,
                getArrayRecordNames: getArrayRecordNames,
                getAllRecordNames:getAllRecordNames,
                createStepItem: createStepItem,
                setSelectedComposite:setSelectedComposite,
                editStep: editStep,
                removeStep: removeStep,
                setSelectedStep: setSelectedStep,
                checkValidation: checkValidation,
        getAllRecordNamesExceptDynamic: getAllRecordNamesExceptDynamic
            };
        }

        function createStepItem(stepDefinition, stepEntity, context) {
            var stepItem = {};
            if (stepDefinition == null)
            {
                var stepDefinition = UtilsService.getItemByVal($scope.scopeModal.stepsDefinition, stepEntity.ConfigId, "ExtensionConfigurationId");
                stepItem.readyPromiseDeferred= UtilsService.createPromiseDeferred();
            }
         
            stepItem.editor= stepDefinition.Editor;
            stepItem.stepPreviewUIControl= stepDefinition.StepPreviewUIControl;
            stepItem.title= stepDefinition.Title;
            stepItem.dataTransformationStepConfigId = stepDefinition.ExtensionConfigurationId;
            stepItem.context= context;
            stepItem.loadPromiseDeferred= UtilsService.createPromiseDeferred();
            
            var payload = {
                context: context,
                stepDetails: stepEntity
            };
            stepItem.onPreviewDirectiveReady = function (api) {
                stepItem.previewAPI = api;
               
                var setLoader = function (value) {
                    stepItem.isLoadingPreviewDirective = value;
                    if(!value)
                      context.checkValidation(stepItem);
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, stepItem.previewAPI, payload, setLoader, stepItem.readyPromiseDeferred);
              
            }

            if (stepItem.readyPromiseDeferred != undefined)
            {
                stepItem.readyPromiseDeferred.promise.then(function () {
                    stepItem.readyPromiseDeferred = undefined;
                    VRUIUtilsService.callDirectiveLoad(stepItem.previewAPI, payload, stepItem.loadPromiseDeferred);
                });
            }
            
            return stepItem;
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
                if (recordType != undefined && recordType.DataRecordTypeId !=undefined)
                {
                    VR_GenericData_DataRecordTypeAPIService.GetDataRecordType(recordType.DataRecordTypeId).then(function (response) {
                        if (response)
                            loadPromiseDeferred.resolve(response.Fields);
                    }).catch(function (error) {
                        loadPromiseDeferred.reject(error);
                        VRNotificationService.notifyException(error, $scope);
                    });
                }
            else
            {
                    loadPromiseDeferred.resolve();
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

        function checkValidation(stepItem)
        {
          
            if (stepItem != undefined && stepItem.previewAPI != undefined)
            {
                stepItem.validationMessage = stepItem.previewAPI.checkValidation();
            }
               
            else if ($scope.scopeModal.selectedStep != undefined && $scope.scopeModal.selectedStep.previewAPI != undefined) {
                $scope.scopeModal.selectedStep.validationMessage = $scope.scopeModal.selectedStep.previewAPI.checkValidation();
            }
        }

        function getAllRecordNames() {
            var obj = dataRecordTypeAPI.getData();
            var recordTypeNames = [];
            for (var i = 0; i < obj.RecordTypes.length; i++) {
                var recordType = obj.RecordTypes[i];
                recordTypeNames.push({ Name: recordType.RecordName });
            }
            return recordTypeNames;
        }

        function getRecordNames()
        {
            var obj = dataRecordTypeAPI.getData();
            var recordTypeNames = [];
            for (var i = 0; i < obj.RecordTypes.length;i++)
            {
                var recordType = obj.RecordTypes[i];
                if (!recordType.IsArray)
                {
                    recordTypeNames.push({ Name: recordType.RecordName });

                }
            }
            return recordTypeNames;
        }

        function getArrayRecordNames() {
            var obj = dataRecordTypeAPI.getData();
            var recordTypeNames = [];
            for (var i = 0; i < obj.RecordTypes.length; i++) {
                var recordType = obj.RecordTypes[i];
                if (recordType.IsArray) {
                  recordTypeNames.push({ Name: recordType.RecordName });
                }
            }
            return recordTypeNames;
        }

        function getAllRecordNamesExceptDynamic() {
            var obj = dataRecordTypeAPI.getData();
            var recordTypeNames = [];
            for (var i = 0; i < obj.RecordTypes.length; i++) {
                var recordType = obj.RecordTypes[i];
                if (recordType.DataRecordTypeId != undefined)
                  recordTypeNames.push({ Name: recordType.RecordName });
            }
            return recordTypeNames;
        }

        function setSelectedComposite(api)
        {
            if (selectedComposite != undefined && selectedComposite != api)
            {
                selectedComposite.setCheckedSequence(false);
            }
            if (api == undefined)
            {
                selectedComposite = sequenceDirectiveAPI;
                selectedComposite.setCheckedSequence(true);
            }   
            else 
            {
                selectedComposite = api;
            }
        }

        function editStep(stepItem)
        {
            if ($scope.scopeModal.selectedStep != undefined) {
                applyChanges();
                if ($scope.scopeModal.selectedStep == stepItem) {
                    $scope.scopeModal.selectedStep.isSelected = false;
                    $scope.scopeModal.selectedStep = undefined;
                }
                else {
                    $scope.scopeModal.selectedStep.isSelected = false;
                    stepItem.isSelected = true;
                    $scope.scopeModal.selectedStep = undefined;
                    setTimeout(function () { $scope.scopeModal.selectedStep = stepItem; UtilsService.safeApply($scope) })
                }

            }
            else {
                $scope.scopeModal.selectedStep = stepItem;
            }
        }

        function removeStep(stepItem)
        {
            if ($scope.scopeModal.selectedStep == stepItem)
                $scope.scopeModal.selectedStep = undefined;
        }

        function setSelectedStep(stepItem)
        {
            if ($scope.scopeModal.selectedStep != undefined)
                $scope.scopeModal.selectedStep.isSelected = false;
            stepItem.isSelected = true;
            $scope.scopeModal.selectedStep = stepItem;
        }
        
        function tryCompile()
        {
              applyChanges();
              var dataTransformationDefinitionObject = buildDataTransformationDefinitionObjFromScope();
              return VR_GenericData_DataTransformationDefinitionAPIService.TryCompileSteps(dataTransformationDefinitionObject).then(function (response) {
                if (response) {
                    if (response.Result) {
                        return true;
                      } else {
                        VR_GenericData_DataTransformationDefinitionService.tryCompilationResult(response.ErrorMessages, dataTransformationDefinitionObject);
                        return false;
                    }
                }
            });
         
           
        }

        function refreshEditor()
        {
            var setLoader = function (value) {
                $scope.scopeModal.isLoadingDirective = value;
            };
            var payload = {
                context: $scope.scopeModal.selectedStep.context,
                stepDetails: $scope.scopeModal.selectedStep.previewAPI != undefined ? $scope.scopeModal.selectedStep.previewAPI.getData() : undefined
            };
            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope.scopeModal, editorDirectiveAPI, payload, setLoader);
        }


        function buildDataTransformationDefinitionObjFromScope() {
            var obj = dataRecordTypeAPI.getData();
            var dataTransformationDefinition = {
                Name: $scope.scopeModal.name,
                Title: $scope.scopeModal.title,
                DataTransformationDefinitionId: dataTransformationDefinitionId,
                RecordTypes: obj != undefined ? obj.RecordTypes : undefined,
                MappingSteps: sequenceDirectiveAPI != undefined ? sequenceDirectiveAPI.getData() : undefined
            };
            return dataTransformationDefinition;
        }

        function insertDataTransformationDefinition() {

            var dataTransformationDefinitionObject = buildDataTransformationDefinitionObjFromScope();
            return VR_GenericData_DataTransformationDefinitionAPIService.AddDataTransformationDefinition(dataTransformationDefinitionObject)
            .then(function (response) {
                 savePromiseDeferred.resolve();
                if (VRNotificationService.notifyOnItemAdded("Data Transformation", response)) {
                    if ($scope.onDataTransformationDefinitionAdded != undefined)
                        $scope.onDataTransformationDefinitionAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                 savePromiseDeferred.reject(error);
                VRNotificationService.notifyException(error, $scope);
            });

        }

        function updateDataTransformationDefinition() {
            var dataTransformationDefinitionObject = buildDataTransformationDefinitionObjFromScope();
            VR_GenericData_DataTransformationDefinitionAPIService.UpdateDataTransformationDefinition(dataTransformationDefinitionObject)
            .then(function (response) {
                savePromiseDeferred.resolve();
                if (VRNotificationService.notifyOnItemUpdated("Data Transformation", response)) {
                    if ($scope.onDataTransformationDefinitionUpdated != undefined)
                        $scope.onDataTransformationDefinitionUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                savePromiseDeferred.reject(error);
                VRNotificationService.notifyException(error, $scope);
            });
        }

    }

    appControllers.controller('VR_GenericData_DataTransformationDefinitionEditorController', DataTransformationDefinitionEditorController);
})(appControllers);
