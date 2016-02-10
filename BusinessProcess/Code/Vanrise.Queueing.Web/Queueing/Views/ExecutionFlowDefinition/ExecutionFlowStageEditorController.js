(function (appControllers) {

    "use strict";

    ExecutionFlowStageEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

    function ExecutionFlowStageEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

        var isEditMode;
        var ExecutionFlowStageEntity;

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

            $scope.scopeModal.SaveExecutionFlowStage = function () {
                if (isEditMode) {
                    return updateExecutionFlowStage();
                }
                else {
                    return insertExecutionFlowStage();
                }
            };


            $scope.scopeModal.close = function () {
                $scope.modalContext.closeModal()
            };

        }

        function load() {
            $scope.scopeModal.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadFilterBySection, setTitle])
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
            }
        }

        function setTitle() {
            if (isEditMode && ExecutionFlowStageEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(ExecutionFlowStageEntity.Name, 'Execution Flow Stage');
            else
                $scope.title = UtilsService.buildTitleForAddEditor('Execution Flow Stage');
        }



        function buildexecutionFlowStageObjectFromScope() {
            var executionFlowStage = {};
            executionFlowStage.StageName = $scope.scopeModal.stageName;
            executionFlowStage.QueueNameTemplate = $scope.scopeModal.queueTemplateName;
            executionFlowStage.QueueTitleTemplate = $scope.scopeModal.queueTemplateTitle;

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