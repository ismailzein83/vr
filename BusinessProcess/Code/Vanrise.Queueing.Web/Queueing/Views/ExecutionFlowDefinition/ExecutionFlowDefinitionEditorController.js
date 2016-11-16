(function (appControllers) {

    'use strict';

    ExecutionFlowDefinitionEditorController.$inject = ['$scope', 'VR_Queueing_ExecutionFlowDefinitionAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function ExecutionFlowDefinitionEditorController($scope, VR_Queueing_ExecutionFlowDefinitionAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {
        var isEditMode;
        var executionFlowDefinitionId;
        var executionFlowDefinitionEntity;
        var executionFlowStageAPI;
        var executionFlowStageReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null)
                executionFlowDefinitionId = parameters.ID;

            isEditMode = (executionFlowDefinitionId != undefined);
        }

        function defineScope() {
            $scope.scopeModal = {};

            $scope.scopeModal.onExecutionFlowStageDirectiveReady = function (api) {
                executionFlowStageAPI = api;
                executionFlowStageReadyPromiseDeferred.resolve();
            };

            $scope.save = function () {
                if (isEditMode) {

                    return updateExecutionFlowDefinition();
                }
                else {

                    return insertExecutionFlowDefinition();
                }
            };

            $scope.hasSaveExecutionFlowDefinitionPermession = function () {
                if (isEditMode) {
                    return VR_Queueing_ExecutionFlowDefinitionAPIService.HasUpdateExecutionFlowDefinition();
                }
                else {
                    return VR_Queueing_ExecutionFlowDefinitionAPIService.HasAddExecutionFlowDefinition();
                }
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            $scope.isLoading = true;

            if (isEditMode) {

                getExecutionFlowDefinition().then(function () {
                    loadAllControls().finally(function () {
                       
                    });
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            }
            else {

                loadAllControls().finally(function () {
                    
                })

            }
        }

        function getExecutionFlowDefinition() {
            return VR_Queueing_ExecutionFlowDefinitionAPIService.GetExecutionFlowDefinition(executionFlowDefinitionId).then(function (response) {
                executionFlowDefinitionEntity = response;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadExecutionFlowStage])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }

        function setTitle() {
            if (isEditMode && executionFlowDefinitionEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(executionFlowDefinitionEntity.Name, 'Execution Flow Definition');
            else
                $scope.title = UtilsService.buildTitleForAddEditor('Execution Flow Definition');
        }

        function loadStaticData() {

            if (executionFlowDefinitionEntity == undefined) {
                return;
            }


            $scope.scopeModal.name = executionFlowDefinitionEntity.Name;
            $scope.scopeModal.title = executionFlowDefinitionEntity.Title;

        }


        function loadExecutionFlowStage() {

            var loadExecutionFlowStagePromiseDeferred = UtilsService.createPromiseDeferred();

            executionFlowStageReadyPromiseDeferred.promise
                .then(function () {
                    var directivePayload = (executionFlowDefinitionEntity != undefined) ? { Stages: executionFlowDefinitionEntity.Stages } : undefined;

                    VRUIUtilsService.callDirectiveLoad(executionFlowStageAPI, directivePayload, loadExecutionFlowStagePromiseDeferred);
                });

            return loadExecutionFlowStagePromiseDeferred.promise;
        }
     
        function buildExecutionFlowDefinitionObjFromScope() {
            var stages = executionFlowStageAPI.getData();
            var executionFlowDefinitionObject = {
                ID: (executionFlowDefinitionId != null) ? executionFlowDefinitionId : 0,
                Name: $scope.scopeModal.name,
                Title: $scope.scopeModal.title,
                Stages: stages != undefined ? stages.Stages : undefined
            };
            return executionFlowDefinitionObject;
        }

        function insertExecutionFlowDefinition() {
            $scope.isLoading = true;

            var executionFlowDefinitionObject = buildExecutionFlowDefinitionObjFromScope();

            return VR_Queueing_ExecutionFlowDefinitionAPIService.AddExecutionFlowDefinition(executionFlowDefinitionObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('Execution Flow Definition', response, 'Name')) {
                    if ($scope.onExecutionFlowDefinitionAdded != undefined)
                        $scope.onExecutionFlowDefinitionAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });

        }

        function updateExecutionFlowDefinition() {
            $scope.isLoading = true;

            var executionFlowDefinitionObject = buildExecutionFlowDefinitionObjFromScope();

            return VR_Queueing_ExecutionFlowDefinitionAPIService.UpdateExecutionFlowDefinition(executionFlowDefinitionObject).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('Execution Flow Definition', response, 'Name')) {
                    if ($scope.onExecutionFlowDefinitionUpdated != undefined)
                        $scope.onExecutionFlowDefinitionUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }
    }

    appControllers.controller('VR_Queueing_ExecutionFlowDefinitionEditorController', ExecutionFlowDefinitionEditorController);

})(appControllers);
