(function (appControllers) {

    'use strict';

    ExecutionFlowEditorController.$inject = ['$scope', 'VR_Queueing_ExecutionFlowAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService','VRUIUtilsService'];

    function ExecutionFlowEditorController($scope, VR_Queueing_ExecutionFlowAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {
        var isEditMode;
        var executionFlowId;
        var executionFlowEntity;

        var executionFlowDefinitionSelectorAPI;
        var executionFlowDirectionSelectorReadyDeferred = UtilsService.createPromiseDeferred();


        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null)
                executionFlowId = parameters.ID;

            isEditMode = (executionFlowId != undefined);
        }

        function defineScope() {

            $scope.onExecutionFlowDefinitionSelectorReady = function (api) {
                executionFlowDefinitionSelectorAPI = api;
                executionFlowDirectionSelectorReadyDeferred.resolve();
            };

            $scope.save = function () {
                if (isEditMode) {
                   
                    return updateExecutionFlow();
                }
                else {
                    
                    return insertExecutionFlow();
                }
            };
            $scope.hasSaveExecutionFlowPermission = function () {
                if (isEditMode) {
                    return VR_Queueing_ExecutionFlowAPIService.HasUpdateExecutionFlow();
                }
                else {
                    return VR_Queueing_ExecutionFlowAPIService.HasAddExecutionFlow();
                }
            };
            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            $scope.isLoading = true;

            if (isEditMode) {
                
                getExecutionFlow().then(function () {
                    loadAllControls().finally(function () {
                        executionFlowDefinitionSelectorAPI.setDisabled(true);
                        executionFlowEntity = undefined;
                    });
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            }
            else {
               
                loadAllControls().finally(function () {
                    executionFlowDefinitionSelectorAPI.setDisabled(false);
                });
                
            }
        }

        function getExecutionFlow() {
            return VR_Queueing_ExecutionFlowAPIService.GetExecutionFlow(executionFlowId).then(function (response) {
                executionFlowEntity = response;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadExecutionFlowDefinition])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }

        function setTitle() {
            if (isEditMode && executionFlowEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(executionFlowEntity.Name, 'Execution Flow');
            else
                $scope.title = UtilsService.buildTitleForAddEditor('Execution Flow');
        }

        function loadStaticData() {

            if (executionFlowEntity == undefined) {
                return;
            }
            

            $scope.name = executionFlowEntity.Name;
             
        }

        function loadExecutionFlowDefinition() {
            var executionFlowDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            executionFlowDirectionSelectorReadyDeferred.promise.then(function () {
                
                var payload = {
                    filter: null,
                    selectedIds: executionFlowEntity == undefined ? null:executionFlowEntity.DefinitionId 
                   
                };

                VRUIUtilsService.callDirectiveLoad(executionFlowDefinitionSelectorAPI, payload, executionFlowDefinitionSelectorLoadDeferred);
            });
           
            return executionFlowDefinitionSelectorLoadDeferred.promise;
        }


        function buildExecutionFlowObjFromScope() {
            var executionFlowObject = {
                executionFlowId: (executionFlowId != null) ? executionFlowId : 0,
                name: $scope.name,
                definitionId: executionFlowDefinitionSelectorAPI.getSelectedIds()
            };
            return executionFlowObject;
        }

        function insertExecutionFlow() {
            $scope.isLoading = true;

            var executionFlowObject = buildExecutionFlowObjFromScope();

            return VR_Queueing_ExecutionFlowAPIService.AddExecutionFlow(executionFlowObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('Execution Flow', response, 'Name')) {
                    if ($scope.onExecutionFlowAdded != undefined)
                        $scope.onExecutionFlowAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });

        }

        function updateExecutionFlow() {
            $scope.isLoading = true;

            var executionFlowObject = buildExecutionFlowObjFromScope();

            return VR_Queueing_ExecutionFlowAPIService.UpdateExecutionFlow(executionFlowObject).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('Execution Flow', response, 'Name')) {
                    if ($scope.onExecutionFlowUpdated != undefined)
                        $scope.onExecutionFlowUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }
    }

    appControllers.controller('VR_Queueing_ExecutionFlowEditorController', ExecutionFlowEditorController);

})(appControllers);
