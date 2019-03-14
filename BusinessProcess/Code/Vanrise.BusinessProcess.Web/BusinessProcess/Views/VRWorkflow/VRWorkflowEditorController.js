﻿(function (appControllers) {

    "use strict";

    VRWorkflowEditorController.$inject = ['$scope', 'VRNavigationService', 'VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'BusinessProcess_VRWorkflowAPIService', 'BusinessProcess_VRWorkflowService'];

    function VRWorkflowEditorController($scope, VRNavigationService, VRNotificationService, UtilsService, VRUIUtilsService, BusinessProcess_VRWorkflowAPIService, BusinessProcess_VRWorkflowService) {

        var isEditMode;
        var allVariableNames = [];

        var vrWorkflowEntity;
        var vrWorkflowId;
        var vrWorkflowArgumentEditorRuntimeDict;

        var activitiesErrorsDictionary;
        var classMembersErrors;

        var argumentsGridAPI;
        var argumentsGridReadyDeferred = UtilsService.createPromiseDeferred();

        var workflowDesignerAPI;
        var workflowDesignerReadyDeferred = UtilsService.createPromiseDeferred();

        var activitiesList = [];
        var context;

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
            $scope.scopeModel.isCompileValidContext =
                {
                    validate: function () {
                        if (workflowDesignerAPI == undefined || !workflowDesignerAPI.isActivityValid())
                            return 'Compile action is not allowed';

                        return null;
                    }
                };

            $scope.scopeModel.onWorkflowDesignerReady = function (api) {
                workflowDesignerAPI = api;
                workflowDesignerReadyDeferred.resolve();
            };

            $scope.scopeModel.tryCompileWorkflow = function () {
                return tryCompileWorkflow();
            };

            $scope.scopeModel.tryCompileWorkflowClassMembers = function () {
                return tryCompileWorkflowClassMembers();
            };

            $scope.scopeModel.saveVRWorkflow = function () {
                var promiseDeferred = UtilsService.createPromiseDeferred();
                tryCompileWorkflow().then(function (response) {
                    if (response) {
                        var savePromise;
                        if (isEditMode)
                            savePromise = updateVRWorkflow();
                        else
                            savePromise = addVRWorkflow();

                        savePromise.then(function () {
                            promiseDeferred.resolve();
                        }).catch(function (error) {
                            VRNotificationService.notifyExceptionWithClose(error, $scope);
                            promiseDeferred.reject();
                        });
                    }
                    else {
                        promiseDeferred.resolve();
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    promiseDeferred.reject();
                });

                return promiseDeferred.promise;
            };

            $scope.scopeModel.classMembersHasErrorsValidator = function () {
                if (classMembersErrors != undefined && classMembersErrors.length > 0) {
                    var errorMessage = '';
                    for (var i = 0; i < classMembersErrors.length; i++) {
                        errorMessage += (i + 1) + ') ' + classMembersErrors[i] + '\n';
                    }
                    return errorMessage;
                }
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
                    $scope.scopeModel.classMembersCode = (vrWorkflowEntity.Settings != undefined && vrWorkflowEntity.Settings.ClassMembers != undefined) ? vrWorkflowEntity.Settings.ClassMembers.ClassMembersCode : null;
                }
            }

            function loadArgumentsGrid() {
                var argumentsGridLoadDeferred = UtilsService.createPromiseDeferred();

                argumentsGridReadyDeferred.promise.then(function () {
                    var argumentsGridPayload = {
                        vrWorkflowArguments: (vrWorkflowEntity != undefined && vrWorkflowEntity.Settings != undefined) ? vrWorkflowEntity.Settings.Arguments : undefined,
                        vrWorkflowArgumentEditorRuntimeDict: vrWorkflowArgumentEditorRuntimeDict,
                        reserveVariableName: reserveVariableName,
                        reserveVariableNames: reserveVariableNames,
                        isVariableNameReserved: isVariableNameReserved,
                        eraseVariableName: eraseVariableName
                    };
                    VRUIUtilsService.callDirectiveLoad(argumentsGridAPI, argumentsGridPayload, argumentsGridLoadDeferred);
                });

                return argumentsGridLoadDeferred.promise;
            }

            function addToList(activityId, resetErrorsFunction) {
                activitiesList[activityId] = resetErrorsFunction;
            }

            function removeFromList(activityId) {
                delete activitiesList[activityId];
            }

            function reserveVariableName(name) {
                allVariableNames.push(name);
            }

            function reserveVariableNames(variables) {
                if (variables != undefined && variables.length > 0)
                    allVariableNames = allVariableNames.concat(variables.map(a => a.Name));
            }

            function isVariableNameReserved(name) {
                var variableName = (name != undefined) ? name.toLowerCase() : null;
                return UtilsService.contains(allVariableNames, variableName);
            }

            function eraseVariableName(name) {
                var variableIndex = allVariableNames.indexOf(name);
                if (variableIndex >= 0)
                    allVariableNames.splice(variableIndex, 1);
            }

            function doesActivityhaveErrors(vrWorkflowActivityId) {
                if (vrWorkflowActivityId != undefined) {
                    if (activitiesErrorsDictionary != undefined) {
                        return activitiesErrorsDictionary[vrWorkflowActivityId];
                    }
                }
                return null;
            }

            function loadWorkflowDesigner() {
                var workflowDesignerLoadDeferred = UtilsService.createPromiseDeferred();

                UtilsService.waitMultiplePromises([workflowDesignerReadyDeferred.promise, argumentsGridReadyDeferred.promise]).then(function () {
                    var workflowDesignerPayload;
                    context = {};
                    workflowDesignerPayload = {
                        vrWorkflowId: vrWorkflowId,
                        getWorkflowArguments: argumentsGridAPI.getData,
                        rootActivity: (vrWorkflowEntity != undefined && vrWorkflowEntity.Settings != undefined) ? vrWorkflowEntity.Settings.RootActivity : undefined,
                        reserveVariableName: reserveVariableName,
                        addToList: addToList,
                        removeFromList: removeFromList,
                        reserveVariableNames: reserveVariableNames,
                        isVariableNameReserved: isVariableNameReserved,
                        eraseVariableName: eraseVariableName,
                        doesActivityhaveErrors: doesActivityhaveErrors,
                        context: context
                    };
                    VRUIUtilsService.callDirectiveLoad(workflowDesignerAPI, workflowDesignerPayload, workflowDesignerLoadDeferred);
                });
                return workflowDesignerLoadDeferred.promise;
            }

            return UtilsService.waitMultipleAsyncOperations([loadStaticData, setTitle, loadArgumentsGrid, loadWorkflowDesigner]).then(function () {

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

        function tryCompileWorkflow() {
            activitiesErrorsDictionary = undefined;
            classMembersErrors = undefined;

            if (activitiesList != undefined) {
                var resetErrorsFunction;
                for (var activityId in activitiesList) {
                    resetErrorsFunction = activitiesList[activityId];
                    resetErrorsFunction();
                }
            }

            var promiseDeferred = UtilsService.createPromiseDeferred();
            $scope.scopeModel.isLoading = true;
            var workflowObj = buildVRWorkflowObjFromScope();
            BusinessProcess_VRWorkflowAPIService.TryCompileWorkflow(workflowObj).then(function (response) {
                if (response.Result) {
                    VRNotificationService.showSuccess("Workflow compiled successfully.");
                    promiseDeferred.resolve(true);
                }
                else {
                    classMembersErrors = response.ClassMembersErrors;
                    activitiesErrorsDictionary = response.ActivitiesErrors;
                    //BusinessProcess_VRWorkflowService.tryCompilationResult(response.ErrorMessages, workflowObj);
                    promiseDeferred.resolve(false);
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
                promiseDeferred.reject(error);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
            return promiseDeferred.promise;
        }

        function tryCompileWorkflowClassMembers() {
            classMembersErrors = undefined;
            var promiseDeferred = UtilsService.createPromiseDeferred();
            $scope.scopeModel.isLoading = true;
            var workflowObj = buildVRWorkflowObjFromScope();
            BusinessProcess_VRWorkflowAPIService.TryCompileWorkflowClassMembers(workflowObj).then(function (response) {
                if (response.Result) {
                    VRNotificationService.showSuccess("Workflow class members compiled successfully.");
                    promiseDeferred.resolve(true);
                }
                else {
                    classMembersErrors = response.ClassMembersErrors;
                    promiseDeferred.resolve(false);
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
                promiseDeferred.reject(error);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
            return promiseDeferred.promise;
        }

        function buildVRWorkflowObjFromScope() {
            var vrWorkflowObj = {
                Name: $scope.scopeModel.name,
                Title: $scope.scopeModel.title,
                Settings: {
                    Arguments: argumentsGridAPI.getData(),
                    ClassMembers: { ClassMembersCode: $scope.scopeModel.classMembersCode },
                    RootActivity: workflowDesignerAPI.getData()
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