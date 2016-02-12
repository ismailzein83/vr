(function (appControllers) {

    "use strict";

    ExecutionFlowStageEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

    function ExecutionFlowStageEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

        var isEditMode;
        var ExecutionFlowStageEntity;

        var queueActivatorConfigDirectiveReadyAPI;
        var queueActivatorConfigDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var dataRecordTypeSelectorAPI;
        var dataRecordTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                ExecutionFlowStageEntity = parameters.ExecutionFlowStage;
            }
            isEditMode = (ExecutionFlowStageEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModal = {};

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

            $scope.onSelectedDataTransformationChanged = function () {
                alert("test");
            }

        }

        function load() {
            $scope.scopeModal.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadFilterBySection, setTitle, loadDataRecordTypeSelector, loadQueueActivatorConfigDirective])
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
                .finally(function () {
                    $scope.scopeModal.isLoading = false;
                });
        }

        function loadFilterBySection() {
            if (ExecutionFlowStageEntity != undefined) {
                $scope.scopeModal.stageName = ExecutionFlowStageEntity.StageName;
                $scope.scopeModal.queueTemplateName = ExecutionFlowStageEntity.QueueNameTemplate;
                $scope.scopeModal.queueTemplateTitle = ExecutionFlowStageEntity.QueueTitleTemplate;
                $scope.scopeModal.batchDescription = ExecutionFlowStageEntity.QueueItemType.BatchDescription;
            }
        }

        function loadDataRecordTypeSelector() {
            var dataRecordTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            dataRecordTypeSelectorReadyDeferred.promise.then(function () {
                var payload;

                if (ExecutionFlowStageEntity != undefined) {
                    payload = {
                        selectedIds: ExecutionFlowStageEntity.QueueItemType.DataRecordTypeId
                    };
                }

                VRUIUtilsService.callDirectiveLoad(dataRecordTypeSelectorAPI, payload, dataRecordTypeSelectorLoadDeferred);
            });

            return dataRecordTypeSelectorLoadDeferred.promise;
        }


        function loadQueueActivatorConfigDirective() {
            var queueActivatorConfigDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            queueActivatorConfigDirectiveReadyPromiseDeferred.promise.then(function () {
                var payload;

                if (ExecutionFlowStageEntity != undefined) {

                    payload = {
                        QueueActivator: ExecutionFlowStageEntity.QueueActivator
                      
                    };
                }

                VRUIUtilsService.callDirectiveLoad(queueActivatorConfigDirectiveReadyAPI, payload, queueActivatorConfigDirectiveLoadPromiseDeferred);
            });
            return queueActivatorConfigDirectiveLoadPromiseDeferred.promise;
        }


        function setTitle() {
            if (isEditMode && ExecutionFlowStageEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(ExecutionFlowStageEntity.Name, 'Execution Flow Stage');
            else
                $scope.title = UtilsService.buildTitleForAddEditor('Execution Flow Stage');
        }



        function buildexecutionFlowStageObjectFromScope() {
            var executionFlowStage = {};
            var obj = queueActivatorConfigDirectiveReadyAPI.getData();
  
            executionFlowStage.StageName = $scope.scopeModal.stageName;
            executionFlowStage.QueueNameTemplate = $scope.scopeModal.queueTemplateName;
            executionFlowStage.QueueTitleTemplate = $scope.scopeModal.queueTemplateTitle;
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