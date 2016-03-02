(function (appControllers) {

    "use strict";

    ExecutionFlowStageEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

    function ExecutionFlowStageEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

        var isEditMode;
        var executionFlowStageEntity;
        var dataRecordTypeId;
        var existingExecutionFlowStages;

        var queueActivatorConfigDirectiveReadyAPI;
        var queueActivatorConfigDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var directiveReadyDeferred;

        var dataRecordTypeSelectorAPI;
        var dataRecordTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var existingStages = [];
        var secondtimeChanged = false;


        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                existingExecutionFlowStages = parameters.ExistingExecutionFlowStages;
                executionFlowStageEntity = parameters.ExecutionFlowStage;
            }
            isEditMode = (executionFlowStageEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModal = {};

            $scope.isDataRecordTypeSelected = false;

            $scope.scopeModal.queueTemplateName = "Queue_#FlowId#_#StageName#";
            $scope.scopeModal.queueTemplateTitle = "#StageName# Queue (#FlowName#)";
            $scope.scopeModal.batchDescription = "#RECORDSCOUNT# of CDRs";


            $scope.scopeModal.sourceStages = [];

            $scope.scopeModal.selectedSourceStages = [];

            $scope.scopeModal.onDirectiveReady = function (api) {
                queueActivatorConfigDirectiveReadyAPI = api;
                queueActivatorConfigDirectiveReadyPromiseDeferred.resolve();
            }

            $scope.scopeModal.SaveExecutionFlowStage = function () {
                if (isEditMode) {
                    return updateExecutionFlowStage();
                }
                else {
                    return insertExecutionFlowStage();
                }
            };


            $scope.scopeModal.validateStageName = function () {
                if (isEditMode && $scope.scopeModal.stageName == executionFlowStageEntity.StageName)
                    return null;
                else if (UtilsService.getItemIndexByVal(existingExecutionFlowStages, $scope.scopeModal.stageName, 'StageName') != -1)
                    return 'Same Name Exist.';
                return null;
            }

            $scope.scopeModal.validateQueueTemplateName = function () {
                if ($scope.scopeModal.queueTemplateName.indexOf('#FlowId#') >= 0)
                    return null;
                else
                    return 'Queue Template Name Must Contain #FlowId#.';
                return null;
            }

            $scope.scopeModal.validateBatchDescription = function () {
                if ($scope.scopeModal.batchDescription.indexOf('#RECORDSCOUNT#') >= 0)
                    return null;
                else
                    return 'Batch Description Must Contain #RECORDSCOUNT#.';
                return null;
            }

            $scope.onDataRecordTypeSelectorReady = function (api) {
                dataRecordTypeSelectorAPI = api;
                dataRecordTypeSelectorReadyDeferred.resolve();
            };

            $scope.scopeModal.close = function () {
                $scope.modalContext.closeModal()
            };


            $scope.onDataRecordTypeSelectionChange = function () {

                dataRecordTypeId = dataRecordTypeSelectorAPI.getSelectedIds();
                if (dataRecordTypeId != undefined) {
                    $scope.isDataRecordTypeSelected = true;
                    if (!secondtimeChanged) {
                        loadSourceStages(true);
                        loadQueueActivatorSection(dataRecordTypeId, false);
                        secondtimeChanged = true;
                    }
                    secondtimeChanged = false;
                   
                }
                else
                    $scope.isDataRecordTypeSelected = false;
            }

        }


        function load() {
            $scope.scopeModal.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadStaticSection, setTitle, loadDataRecordTypeSection])
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
                .finally(function () {
                    $scope.scopeModal.isLoading = false;
                });
        }

        function loadSourceStages(isUserSelected) {
            $scope.scopeModal.selectedSourceStages.length = 0;
            $scope.scopeModal.sourceStages.length = 0;
            existingStages.length = 0;
            var existingExecutionFlowStage;
            if (existingExecutionFlowStages != undefined) {
                for (var i = 0; i < existingExecutionFlowStages.length; i++) {
                    existingExecutionFlowStage = existingExecutionFlowStages[i];
                    existingStages.push({ stageName: existingExecutionFlowStage.StageName, DataRecordTypeId: existingExecutionFlowStage.QueueItemType.DataRecordTypeId });
                }
                for (var i = 0; i < existingExecutionFlowStages.length; i++) {
                    existingExecutionFlowStage = existingExecutionFlowStages[i];
                    if (isEditMode && !isUserSelected) {
                        existingStages.push({ stageName: existingExecutionFlowStage.StageName, DataRecordTypeId: existingExecutionFlowStage.QueueItemType.DataRecordTypeId });
                        if (executionFlowStageEntity != undefined && existingExecutionFlowStage.StageName != executionFlowStageEntity.StageName && existingExecutionFlowStage.QueueItemType.DataRecordTypeId == executionFlowStageEntity.QueueItemType.DataRecordTypeId)
                            $scope.scopeModal.sourceStages.push({ stageName: existingExecutionFlowStages[i].StageName });
                        if (executionFlowStageEntity.SourceStages != undefined) {
                            for (var i = 0; i < executionFlowStageEntity.SourceStages.length; i++) {
                                var selectedValue = UtilsService.getItemByVal($scope.scopeModal.sourceStages, executionFlowStageEntity.SourceStages[i], "stageName");
                                if (selectedValue != null)
                                    $scope.scopeModal.selectedSourceStages.push(selectedValue);
                            }
                        }

                    }
                    else if (isEditMode && dataRecordTypeId != undefined && existingExecutionFlowStage.StageName != executionFlowStageEntity.StageName && existingExecutionFlowStage.QueueItemType.DataRecordTypeId == dataRecordTypeId) {
                        $scope.scopeModal.sourceStages.push({ stageName: existingExecutionFlowStages[i].StageName });
                    }
                    else if (!isEditMode && dataRecordTypeId != undefined && existingExecutionFlowStage.QueueItemType.DataRecordTypeId == dataRecordTypeId) {
                        $scope.scopeModal.sourceStages.push({ stageName: existingExecutionFlowStages[i].StageName });
                    }
                }
            }

        }

        function loadStaticSection() {
            if (executionFlowStageEntity != undefined) {
                $scope.scopeModal.stageName = executionFlowStageEntity.StageName;
                $scope.scopeModal.queueTemplateName = executionFlowStageEntity.QueueNameTemplate;
                $scope.scopeModal.queueTemplateTitle = executionFlowStageEntity.QueueTitleTemplate;
                $scope.scopeModal.batchDescription = executionFlowStageEntity.QueueItemType.BatchDescription;
                $scope.scopeModal.maximumConcurrentReaders = executionFlowStageEntity.MaximumConcurrentReaders;
            }
        }

        function loadDataRecordTypeSection() {
            var promises = [];
            var laodDataRecordTypeSelectorPromise = loadDataRecordTypeSelector();
            promises.push(laodDataRecordTypeSelectorPromise);

            laodDataRecordTypeSelectorPromise.then(function () {

                if (executionFlowStageEntity != undefined) {
                    loadSourceStages(false);
                    var loadQueueActivatorSectionPromise = loadQueueActivatorSection(executionFlowStageEntity.QueueItemType.dataRecordTypeId, true);
                    promises.push(loadQueueActivatorSectionPromise);
                }
                else
                    loadSourceStages(false);
            });
            return UtilsService.waitMultiplePromises(promises);
        }


        function loadDataRecordTypeSelector() {

            var dataRecordTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            dataRecordTypeSelectorReadyDeferred.promise.then(function () {
                var payload;

                if (executionFlowStageEntity != undefined) {
                    payload = {
                        selectedIds: executionFlowStageEntity.QueueItemType.DataRecordTypeId
                    };
                }

                VRUIUtilsService.callDirectiveLoad(dataRecordTypeSelectorAPI, payload, dataRecordTypeSelectorLoadDeferred);
            });

            return dataRecordTypeSelectorLoadDeferred.promise;
        }


        function loadQueueActivatorSection(dataRecordTypeId, isEditMode) {
            var queueActivatorConfigDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            queueActivatorConfigDirectiveReadyPromiseDeferred.promise.then(function () {
                var payload = {
                    ExistingStages: existingStages,
                    DataRecordTypeId: dataRecordTypeSelectorAPI.getSelectedIds()
                }

                if (isEditMode) {
                    payload.QueueActivator = executionFlowStageEntity.QueueActivator;
                };

                VRUIUtilsService.callDirectiveLoad(queueActivatorConfigDirectiveReadyAPI, payload, queueActivatorConfigDirectiveLoadPromiseDeferred);
            });
            return queueActivatorConfigDirectiveLoadPromiseDeferred.promise;
        }


        function setTitle() {
            if (isEditMode && executionFlowStageEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(executionFlowStageEntity.StageName, 'Execution Flow Stage');
            else
                $scope.title = UtilsService.buildTitleForAddEditor('Execution Flow Stage');
        }


        function buildexecutionFlowStageObjectFromScope() {
            var executionFlowStage = {};
            var queueActivator = queueActivatorConfigDirectiveReadyAPI.getData();

            executionFlowStage.StageName = $scope.scopeModal.stageName;
            executionFlowStage.QueueNameTemplate = $scope.scopeModal.queueTemplateName;
            executionFlowStage.QueueTitleTemplate = $scope.scopeModal.queueTemplateTitle;
            executionFlowStage.MaximumConcurrentReaders = $scope.scopeModal.maximumConcurrentReaders;
            executionFlowStage.SourceStages = [];
            executionFlowStage.SourceStages = UtilsService.getPropValuesFromArray($scope.scopeModal.selectedSourceStages, "stageName");
            executionFlowStage.QueueItemType = {
                $type: "Vanrise.GenericData.QueueActivators.DataRecordBatchQueueItemType, Vanrise.GenericData.QueueActivators",
                DataRecordTypeId: dataRecordTypeSelectorAPI.getSelectedIds(),
                BatchDescription: $scope.scopeModal.batchDescription
            };
            executionFlowStage.QueueActivator = queueActivator;
            return executionFlowStage;
        }


        function insertExecutionFlowStage() {
            var executionFlowStageObject = buildexecutionFlowStageObjectFromScope();
            if ($scope.onExecutionFlowStageAdded != undefined)
                $scope.onExecutionFlowStageAdded(executionFlowStageObject);
            $scope.modalContext.closeModal();

        }

        function updateExecutionFlowStage() {
            var executionFlowStageObject = buildexecutionFlowStageObjectFromScope();
            if ($scope.onExecutionFlowStageUpdated != undefined)
                $scope.onExecutionFlowStageUpdated(executionFlowStageObject);
            $scope.modalContext.closeModal();
        }

    }

    appControllers.controller('VR_Queueing_ExecutionFlowStageEditorController', ExecutionFlowStageEditorController);
})(appControllers);