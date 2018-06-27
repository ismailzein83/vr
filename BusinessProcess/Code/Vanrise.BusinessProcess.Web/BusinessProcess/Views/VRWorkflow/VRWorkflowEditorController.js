VRWorkflowEditorController.$inject = ['$scope', 'VRNavigationService', 'VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'BusinessProcess_VRWorkflowAPIService'];

function VRWorkflowEditorController($scope, VRNavigationService, VRNotificationService, UtilsService, VRUIUtilsService, BusinessProcess_VRWorkflowAPIService) {

    var isEditMode;
    var context;

    var workflowEntity;
    var workflowId;

    var argumentsGridAPI;
    var argumentsGridReadyDeferred = UtilsService.createPromiseDeferred();

    loadParameters();
    defineScope();
    load();

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);
        if (parameters != undefined && parameters != null) {
            context = parameters.context;
            workflowId = parameters.vrWorkflowId;
        }
        isEditMode = (workflowId != undefined);
    }

    function defineScope() {
        $scope.scopeModel = {};

        $scope.scopeModel.onArgumentsGridReady = function (api) {
            argumentsGridAPI = api;
            argumentsGridReadyDeferred.resolve();
        };

        $scope.saveWorkflow = function () {
            return (isEditMode) ? updateWorkflow() : addWorkflow();
        };

        $scope.close = function () {
            $scope.modalContext.closeModal();
        };
    }

        function buildWorkflowObjFromScope() {
            var workflowObj = {
                Name: $scope.scopeModel.name,
                Title: $scope.scopeModel.title,
                Settings: {
                    $type: "Vanrise.BusinessProcess.Entities.VRWorkflowSettings, Vanrise.BusinessProcess.Entities",
                    Arguments: argumentsGridAPI.getData()
                }
            };

            if (isEditMode) {
                workflowObj.VRWorkflowId = workflowId;
            }

            return workflowObj;
        }

    function addWorkflow() {
        $scope.scopeModel.isLoading = true;
        return BusinessProcess_VRWorkflowAPIService.InsertVRWorkflow(buildWorkflowObjFromScope()).then(function (response) {
            if (VRNotificationService.notifyOnItemAdded('Workflow', response, 'Name')) {
                if ($scope.onWorkflowAdded != undefined) {
                    $scope.onWorkflowAdded(response.InsertedObject);
                }
                $scope.modalContext.closeModal();
            }
        }).catch(function (error) {
            VRNotificationService.notifyException(error, $scope);
        }).finally(function () {
            $scope.scopeModel.isLoading = false;
        });
    }

    function updateWorkflow() {
        $scope.scopeModel.isLoading = true;
        return BusinessProcess_VRWorkflowAPIService.UpdateVRWorkflow(buildWorkflowObjFromScope()).then(function (response) {
            if (VRNotificationService.notifyOnItemUpdated('Workflow', response, 'Name')) {
                if ($scope.onWorkflowUpdated != undefined) {
                    $scope.onWorkflowUpdated(response.UpdatedObject);
                }
                $scope.modalContext.closeModal();
            }
        }).catch(function (error) {
            VRNotificationService.notifyException(error, $scope);
        }).finally(function () {
            $scope.scopeModel.isLoading = false;
        });
    }

    function load() {
        $scope.scopeModel.isLoading = true;
        if (isEditMode) {
            getBusinessProcessWorkflow().then(function () {
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

    function loadAllControls() {
        return UtilsService.waitMultipleAsyncOperations([loadStaticData, setTitle, loadArgumentsGrid]).then(function () {

        }).finally(function () {
            $scope.scopeModel.isLoading = false;
        }).catch(function (error) {
            VRNotificationService.notifyExceptionWithClose(error, $scope);
        });

        function setTitle() {
            if (workflowEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(workflowEntity.Title, 'Workflow');
            else
                $scope.title = UtilsService.buildTitleForAddEditor('Workflow');
        }

        function loadStaticData() {
            if (workflowEntity != undefined) {
                $scope.scopeModel.name = workflowEntity.Name;
                $scope.scopeModel.title = workflowEntity.Title;
            }
        }

        function loadArgumentsGrid() {
            var argumentsGridLoadDeferred = UtilsService.createPromiseDeferred();

            argumentsGridReadyDeferred.promise.then(function () {
                var argumentsGridPayload;
                if (workflowEntity != undefined && workflowEntity.Settings != undefined) {
                    argumentsGridPayload = {
                        workflowArguments: workflowEntity.Settings.Arguments
                    };
                }
                VRUIUtilsService.callDirectiveLoad(argumentsGridAPI, argumentsGridPayload, argumentsGridLoadDeferred);
            });

            return argumentsGridLoadDeferred.promise;
        }
    }

    function getBusinessProcessWorkflow() {
        return BusinessProcess_VRWorkflowAPIService.GetVRWorkflow(workflowId).then(function (response) {
            workflowEntity = response;
        });
    }

    function getContext() {
        var currentContext = context;
        if (currentContext == undefined)
            currentContext = {};
        return currentContext;
    }
}
appControllers.controller('BusinessProcess_VR_WorkflowEditorController', VRWorkflowEditorController);

