﻿(function (appControllers) {
    "use strict";

    newSchedulerTaskEditorController.$inject = ['$scope', 'SchedulerTaskAPIService', 'UtilsService', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'VRValidationService'];

    function newSchedulerTaskEditorController($scope, SchedulerTaskAPIService, UtilsService, VRModalService, VRNotificationService, VRNavigationService, VRUIUtilsService, VRValidationService) {

        var editMode;
        var taskId;
        var actionTypeId;
        var additionalParameter;
        var taskObject;
        var taskTriggerDirectiveAPI;
        var taskTriggerDirectiveReadyPromiseDeferred; // = UtilsService.createPromiseDeferred();
        var taskActionDirectiveAPI;
        var taskActionDirectiveReadyPromiseDeferred; // = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        $scope.scopeModel.disableActionType = false;
        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            taskId = undefined;
            actionTypeId = undefined;
            additionalParameter = undefined;

            if (parameters != undefined && parameters != null) {
                taskId = parameters.taskId;
                actionTypeId = parameters.actionTypeId;
                additionalParameter = parameters.additionalParameter;
            }


            if (taskId != undefined)
                editMode = true;
            else
                editMode = false;
        }

        function defineScope() {
            $scope.scopeModel = {};
            $scope.SaveTask = function () {
                if (editMode) {
                    return updateTask();
                }
                else {
                    return insertTask();
                }
            };

            $scope.close = function () {
                $scope.modalContext.closeModal()
            };

            $scope.triggerTypes = [];
            $scope.schedulerTaskTrigger = {};

            $scope.actionTypes = [];
            $scope.schedulerTaskAction = {};

            $scope.startEffDate = new Date();
            $scope.endEffDate = undefined;

            $scope.onTaskTriggerDirectiveReady = function (api) {
                taskTriggerDirectiveAPI = api;
                var setLoader = function (value) {
                    $scope.isLoadingTrigger = value;
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, taskTriggerDirectiveAPI, undefined, setLoader, taskTriggerDirectiveReadyPromiseDeferred);

            };
            $scope.onTaskActionDirectiveReady = function (api) {
                taskActionDirectiveAPI = api;
                var setLoader = function (value) {
                    $scope.isLoadingAction = value;
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, taskActionDirectiveAPI, undefined, setLoader, taskActionDirectiveReadyPromiseDeferred);
                if (api.setAdditionalParamter != undefined && typeof (api.setAdditionalParamter) == "function") {
                    api.setAdditionalParamter(additionalParameter);
                }
            };
            $scope.scopeModel.validateDates = function () {
                return VRValidationService.validateTimeRange($scope.scopeModel.startEffDate, $scope.scopeModel.endEffDate);
            };

            $scope.onTaskActionSelctionChanged = function () {
                var payload;
                if (taskObject && taskObject.TaskSettings != undefined && taskActionDirectiveAPI != undefined) {
                    payload = {
                        data: taskObject.TaskSettings.TaskActionArgument
                    };
                    taskActionDirectiveAPI.load(payload);
                }

            };

            $scope.hasSavePermission = function () {
                if (editMode)
                    return SchedulerTaskAPIService.HasUpdateSchedulerTaskPermission();
                else
                    return SchedulerTaskAPIService.HasAddSchedulerTaskPermission();
            };
        }

        function load() {
            $scope.isLoading = true;

            if (editMode) {
                getTask().then(function () {
                    loadAllControls().finally(function () {
                        taskObject = undefined;
                    });
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadTriggers, loadActions, setTitle, loadStaticData])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }

        function getTask() {
            return SchedulerTaskAPIService.GetTask(taskId)
               .then(function (response) {
                   taskObject = response;
               }).catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
        }

        function loadTriggers() {
            var promises = [];
            var triggersPayload;
            if (taskObject != undefined && taskObject.TaskSettings != null) {
                triggersPayload = {
                    data: taskObject.TaskSettings.TaskTriggerArgument
                };
            }
            var taskTriggerTypesLoad = SchedulerTaskAPIService.GetSchedulerTaskTriggerTypes().then(function (response) {
                angular.forEach(response, function (item) {
                    $scope.triggerTypes.push(item);
                });
                if (taskObject != undefined && taskObject.TriggerTypeId)
                    $scope.scopeModel.selectedTriggerType = UtilsService.getItemByVal($scope.triggerTypes, taskObject.TriggerTypeId, "TriggerTypeId");
                else {
                    $scope.scopeModel.selectedTriggerType = UtilsService.getItemByVal($scope.triggerTypes, 1, "TriggerTypeId");
                }
            });
            promises.push(taskTriggerTypesLoad);
            if (triggersPayload) {
                taskTriggerDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

                var loadTaskTriggerPromiseDeferred = UtilsService.createPromiseDeferred();
                promises.push(loadTaskTriggerPromiseDeferred.promise);

                taskTriggerDirectiveReadyPromiseDeferred.promise.then(function () {
                    taskTriggerDirectiveReadyPromiseDeferred = undefined;
                    VRUIUtilsService.callDirectiveLoad(taskTriggerDirectiveAPI, triggersPayload, loadTaskTriggerPromiseDeferred);
                });
            }

            return UtilsService.waitMultiplePromises(promises);
        }

        function loadActions() {
            var promises = [];
            var actionPayload;

            if (taskObject != undefined && taskObject.TaskSettings != null) {
                actionPayload = {
                    data: taskObject.TaskSettings.TaskActionArgument
                };
            }

            var loadActionTypesPromise = SchedulerTaskAPIService.GetSchedulerTaskActionTypes().then(function (response) {
                angular.forEach(response, function (item) {
                    if (!item.Info.SystemType) {
                        $scope.actionTypes.push(item);
                    }
                });
                if (taskObject != undefined && taskObject.ActionTypeId) {
                    $scope.scopeModel.selectedActionType = UtilsService.getItemByVal($scope.actionTypes, taskObject.ActionTypeId, "ActionTypeId");
                    $scope.scopeModel.disableActionType = true;
                }
                else if (additionalParameter != undefined && additionalParameter.actionTypeId) {
                    $scope.scopeModel.selectedActionType = UtilsService.getItemByVal($scope.actionTypes, additionalParameter.actionTypeId, "ActionTypeId");
                    $scope.scopeModel.disableActionType = true;
                }
                else {
                    $scope.scopeModel.selectedActionType = $scope.actionTypes != undefined && $scope.actionTypes.length > 0 ? $scope.actionTypes[0] : undefined;
                }

            });

            promises.push(loadActionTypesPromise);

            if (actionPayload != undefined && actionPayload.data) {
                taskActionDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

                var loadTaskActionPromiseDeferred = UtilsService.createPromiseDeferred();
                promises.push(loadTaskActionPromiseDeferred.promise);

                taskActionDirectiveReadyPromiseDeferred.promise.then(function () {
                    taskActionDirectiveReadyPromiseDeferred = undefined;
                    VRUIUtilsService.callDirectiveLoad(taskActionDirectiveAPI, actionPayload, loadTaskActionPromiseDeferred);
                });
            }

            return UtilsService.waitMultiplePromises(promises);
        }

        function setTitle() {
            if (editMode && taskObject != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(taskObject.Name, "Task");
            else
                $scope.title = UtilsService.buildTitleForAddEditor("Task");
        }

        function loadStaticData() {

            if (taskObject == undefined)
                return;

            $scope.scopeModel.name = taskObject.Name;
            $scope.scopeModel.isEnabled = taskObject.IsEnabled;
            $scope.scopeModel.startEffDate = taskObject.TaskSettings.StartEffDate;
            $scope.scopeModel.endEffDate = taskObject.TaskSettings.EndEffDate;
        }

        function buildTaskObjFromScope() {
            var taskObject = {
                TaskId: (taskId != null) ? taskId : 0,
                Name: $scope.scopeModel.name,
                IsEnabled: $scope.scopeModel.isEnabled,
                TaskType: 1,
                TriggerTypeId: $scope.scopeModel.selectedTriggerType.TriggerTypeId,
                ActionTypeId: $scope.scopeModel.selectedActionType.ActionTypeId,
                TaskSettings:
                    {
                        TaskTriggerArgument: taskTriggerDirectiveAPI.getData(),
                        TaskActionArgument: taskActionDirectiveAPI.getData(),
                        StartEffDate: $scope.scopeModel.startEffDate,
                        EndEffDate: $scope.scopeModel.endEffDate
                    }
            };
            return taskObject;
        }

        function insertTask() {
            $scope.isLoading = true;
            var taskObject = buildTaskObjFromScope();
            return SchedulerTaskAPIService.AddTask(taskObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Schedule Task", response)) {
                    if ($scope.onTaskAdded != undefined)
                        $scope.onTaskAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });;

        }

        function updateTask() {
            $scope.isLoading = true;
            var taskObject = buildTaskObjFromScope();
            SchedulerTaskAPIService.UpdateTask(taskObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Schedule Task", response)) {
                    if ($scope.onTaskUpdated != undefined)
                        $scope.onTaskUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });;
        }
    }

    appControllers.controller('Runtime_NewSchedulerTaskEditorController', newSchedulerTaskEditorController);

})(appControllers);