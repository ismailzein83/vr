(function (appControllers) {

    "use strict";

    VRWorkflowEditorController.$inject = ['$scope', 'VRNavigationService', 'VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'BusinessProcess_VRWorkflowAPIService'];

    function VRWorkflowEditorController($scope, VRNavigationService, VRNotificationService, UtilsService, VRUIUtilsService, BusinessProcess_VRWorkflowAPIService) {

        var isEditMode;

        var vrWorkflowEntity;
        var vrWorkflowId;
        var vrWorkflowArgumentEditorRuntimeDict;

        var argumentsGridAPI;
        var argumentsGridReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                vrWorkflowId = parameters.vrWorkflowId;
            }

            isEditMode = (vrWorkflowId != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onArgumentsGridReady = function (api) {
                argumentsGridAPI = api;
                argumentsGridReadyDeferred.resolve();
            };

            $scope.scopeModel.saveVRWorkflow = function () {
                return (isEditMode) ? updateVRWorkflow() : addVRWorkflow();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            if (isEditMode) {
                getVRWorkflowEditorRuntime(vrWorkflowId).then(function () {
                    loadAllControls();
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }
        }

        function getVRWorkflowEditorRuntime(vrWorkflowId) {
            return BusinessProcess_VRWorkflowAPIService.GetVRWorkflowEditorRuntime(vrWorkflowId).then(function (response) {
                if (response != undefined) {
                    vrWorkflowEntity = response.Entity;
                    vrWorkflowArgumentEditorRuntimeDict = response.VRWorkflowArgumentEditorRuntimeDict;
                }
            });
        }

        function loadAllControls() {

            function setTitle() {
                if (vrWorkflowEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(vrWorkflowEntity.Title, 'Workflow');
                else
                    $scope.title = UtilsService.buildTitleForAddEditor('Workflow');
            }

            function loadStaticData() {
                if (vrWorkflowEntity != undefined) {
                    $scope.scopeModel.name = vrWorkflowEntity.Name;
                    $scope.scopeModel.title = vrWorkflowEntity.Title;
                }
            }

            function loadArgumentsGrid() {
                var argumentsGridLoadDeferred = UtilsService.createPromiseDeferred();

                argumentsGridReadyDeferred.promise.then(function () {
                    var argumentsGridPayload;
                    if (vrWorkflowEntity != undefined && vrWorkflowEntity.Settings != undefined) {
                        argumentsGridPayload = {
                            vrWorkflowArguments: vrWorkflowEntity.Settings.Arguments,
                            vrWorkflowArgumentEditorRuntimeDict: vrWorkflowArgumentEditorRuntimeDict
                        };
                    }
                    VRUIUtilsService.callDirectiveLoad(argumentsGridAPI, argumentsGridPayload, argumentsGridLoadDeferred);
                });

                return argumentsGridLoadDeferred.promise;
            }

            return UtilsService.waitMultipleAsyncOperations([loadStaticData, setTitle, loadArgumentsGrid]).then(function () {

            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function addVRWorkflow() {
            $scope.scopeModel.isLoading = true;
            return BusinessProcess_VRWorkflowAPIService.InsertVRWorkflow(buildVRWorkflowObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('Workflow', response, 'Name')) {
                    if ($scope.onVRWorkflowAdded != undefined) {
                        $scope.onVRWorkflowAdded(response.InsertedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function updateVRWorkflow() {
            $scope.scopeModel.isLoading = true;
            return BusinessProcess_VRWorkflowAPIService.UpdateVRWorkflow(buildVRWorkflowObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('Workflow', response, 'Name')) {
                    if ($scope.onVRWorkflowUpdated != undefined) {
                        $scope.onVRWorkflowUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function buildVRWorkflowObjFromScope() {
            var vrWorkflowObj = {
                Name: $scope.scopeModel.name,
                Title: $scope.scopeModel.title,
                Settings: {
                    Arguments: argumentsGridAPI.getData()
                }
            };

            if (isEditMode) {
                vrWorkflowObj.VRWorkflowId = vrWorkflowId;
            }
            
            return vrWorkflowObj;
        }
    }

    appControllers.controller('BusinessProcess_VR_WorkflowEditorController', VRWorkflowEditorController);
})(appControllers);