(function (appControllers) {

    "use strict";

    ExecutionFlowStageEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

    function ExecutionFlowStageEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

        var isEditMode;
        var executionFlowStageEntity;
        var dataRecordTypeId;

        var queueActivatorConfigDirectiveReadyAPI;
        var queueActivatorConfigDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var directiveReadyDeferred;

        var dataRecordTypeSelectorAPI;
        var dataRecordTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var stagesDataSource = [];
        var filteredStages = [];

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                executionFlowStageEntity = parameters.ExecutionFlowStage;
                for (var i = 0; i < parameters.ExistingFields.length; i++) {
                    var existingFieldsInstance=parameters.ExistingFields[i];
                    stagesDataSource.push({ stageName: existingFieldsInstance.StageName });
                    
                    if (executionFlowStageEntity != undefined && existingFieldsInstance.StageName != executionFlowStageEntity.StageName && existingFieldsInstance.QueueItemType.DataRecordTypeId == executionFlowStageEntity.QueueItemType.DataRecordTypeId)
                        filteredStages.push({ stageName: parameters.ExistingFields[i].StageName });
                    else if(executionFlowStageEntity == undefined)
                        filteredStages.push({ stageName: parameters.ExistingFields[i].StageName });
                }

            }
            isEditMode = (executionFlowStageEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModal = {};

            $scope.isDataRecordTypeSelected = false;

            $scope.scopeModal.sourceStages = [];

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

            $scope.recordTypesWithStages = [];


            $scope.onDataRecordTypeSelectorReady = function (api) {
                dataRecordTypeSelectorAPI = api;
                dataRecordTypeSelectorReadyDeferred.resolve();
            };

            $scope.scopeModal.close = function () {
                $scope.modalContext.closeModal()
            };


            $scope.onDataRecordTypeSelectionChange = function () {

                var selectedDataRecordTypeId = dataRecordTypeSelectorAPI.getSelectedIds();
                if (selectedDataRecordTypeId != undefined) {
                    $scope.isDataRecordTypeSelected = true;
                    loadSourceStages();
                    loadQueueActivatorConfigDirective(selectedDataRecordTypeId, false);

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
            return UtilsService.waitMultipleAsyncOperations([loadFilterBySection, setTitle, loadDataRecordTypeSelectorWithActivatorTemplate])
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
                .finally(function () {
                    $scope.scopeModal.isLoading = false;
                });
        }

        function loadSourceStages() {
            $scope.scopeModal.sourceStages = filteredStages;
              }

        function loadFilterBySection() {
            if (executionFlowStageEntity != undefined) {
                $scope.scopeModal.stageName = executionFlowStageEntity.StageName;
                $scope.scopeModal.queueTemplateName = executionFlowStageEntity.QueueNameTemplate;
                $scope.scopeModal.queueTemplateTitle = executionFlowStageEntity.QueueTitleTemplate;
                $scope.scopeModal.batchDescription = executionFlowStageEntity.QueueItemType.BatchDescription;
                $scope.scopeModal.singleConcurrentReader = executionFlowStageEntity.SingleConcurrentReader;
            }
        }

        function loadDataRecordTypeSelectorWithActivatorTemplate() {
            var laodDataRecordTypeSelectorPromise = loadDataRecordTypeSelector();

            laodDataRecordTypeSelectorPromise.then(function () {
                if (executionFlowStageEntity != undefined) {
                    loadQueueActivatorConfigDirective(executionFlowStageEntity.QueueItemType.dataRecordTypeId, true);
                }
            });
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


        function loadQueueActivatorConfigDirective(dataRecordTypeId, isEditMode) {
            var queueActivatorConfigDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            queueActivatorConfigDirectiveReadyPromiseDeferred.promise.then(function () {
                var payload;
                if (isEditMode) {
                    payload = {
                        QueueActivator: executionFlowStageEntity.QueueActivator,
                        StagesDataSource: stagesDataSource,
                        DataRecordTypeId: dataRecordTypeSelectorAPI.getSelectedIds()
                    };
                }

                else {
                    payload = {
                        StagesDataSource: stagesDataSource,
                        DataRecordTypeId: dataRecordTypeSelectorAPI.getSelectedIds()

                    };
                }
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
            var obj = queueActivatorConfigDirectiveReadyAPI.getData();

            executionFlowStage.StageName = $scope.scopeModal.stageName;
            executionFlowStage.QueueNameTemplate = $scope.scopeModal.queueTemplateName;
            executionFlowStage.QueueTitleTemplate = $scope.scopeModal.queueTemplateTitle;
            executionFlowStage.SingleConcurrentReader = $scope.scopeModal.singleConcurrentReader;
            executionFlowStage.SourceStages = [];
            executionFlowStage.SourceStages.push($scope.scopeModal.selectedSourceStages.stageName);
            executionFlowStage.QueueItemType = {
                $type: "Vanrise.GenericData.QueueActivators.DataRecordBatchQueueItemType, Vanrise.GenericData.QueueActivators",
                DataRecordTypeId: dataRecordTypeSelectorAPI.getSelectedIds(),
                BatchDescription: $scope.scopeModal.batchDescription
            };
            executionFlowStage.QueueActivator = obj;
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