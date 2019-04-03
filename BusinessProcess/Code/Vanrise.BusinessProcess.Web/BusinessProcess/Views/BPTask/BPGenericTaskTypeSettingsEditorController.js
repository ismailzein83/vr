(function (appControllers) {

    "use strict";

    BPGenericTaskTypeSettingsEditorController.$inject = ['$scope', 'BusinessProcess_BPTaskAPIService', 'BusinessProcess_BPTaskTypeAPIService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'VRButtonTypeEnum', 'BusinessProcess_TaskTypeActionService', 'BusinessProcess_BPTaskService', 'BusinessProcess_BPGenericTaskTypeActionAPIService'];

    function BPGenericTaskTypeSettingsEditorController($scope, BusinessProcess_BPTaskAPIService, BusinessProcess_BPTaskTypeAPIService, VRNavigationService, UtilsService, VRUIUtilsService, VRNotificationService, VRButtonTypeEnum, BusinessProcess_TaskTypeActionService, BusinessProcess_BPTaskService, BusinessProcess_BPGenericTaskTypeActionAPIService) {

        var bpTaskId;
        var bpTaskType;
        var fieldValues;
        var bpTask;

        var runtimeEditorAPI;
        var runtimeEditorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters !== undefined && parameters !== null) {
                bpTaskId = parameters.TaskId;
            }
        }

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.actions = [];

            $scope.scopeModel.onEditorRuntimeDirectiveReady = function (api) {
                runtimeEditorAPI = api;
                runtimeEditorReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.take = function () {
                var takeTaskResponse;
                
                function takeTask() {
                    return BusinessProcess_BPTaskAPIService.TakeTask(bpTaskId).then(function (response) {
                        takeTaskResponse = response;
                    });
                }
                var rootPromiseNode = {
                    promises: [takeTask()],
                    getChildNode: function () {
                        return {
                            promises: [load(true)],
                            getChildNode: function () {
                                if (takeTaskResponse != undefined) {
                                    $scope.scopeModel.showTake = takeTaskResponse.ShowTake;
                                    $scope.scopeModel.showRelease = takeTaskResponse.ShowRelease;
                                    $scope.scopeModel.showAssign = takeTaskResponse.ShowAssign;
                                }
                                return {
                                    promises:[]
                                };
                            }
                        };
                    }
                };
                return UtilsService.waitPromiseNode(rootPromiseNode);
             
            };

            $scope.scopeModel.release = function () {
                var releaseTaskResponse;
                function releaseTask() {
                    return BusinessProcess_BPTaskAPIService.ReleaseTask(bpTaskId).then(function (response) {
                        releaseTaskResponse = response;
                    });
                }
           
                var rootPromiseNode = {
                    promises: [releaseTask()],
                    getChildNode: function () {
                     
                        return {
                            promises: [load(true)],
                            getChildNode: function () {
                                if (releaseTaskResponse != undefined) {
                                    $scope.scopeModel.showTake = releaseTaskResponse.ShowTake;
                                    $scope.scopeModel.showRelease = releaseTaskResponse.ShowRelease;
                                    $scope.scopeModel.showAssign = releaseTaskResponse.ShowAssign;
                                }
                                return {
                                    promises: []
                                };
                            }
                        };
                    }
                };
                return UtilsService.waitPromiseNode(rootPromiseNode);
            };

            $scope.scopeModel.assign = function () {
                var userIdsResponse;
                var assignTaskResponse;
                function getAssignedUsers() {
                   return  BusinessProcess_BPTaskAPIService.GetAssignedUsers(bpTaskId).then(function (userIds) {
                       userIdsResponse = userIds;
                    });

                }

                function assignTask(userId) {
                    return BusinessProcess_BPTaskAPIService.AssignTask(bpTaskId, userId).then(function (response) {
                        assignTaskResponse = response;
                    });
                }

                var rootPromiseNode = {
                    promises: [getAssignedUsers()],
                    getChildNode: function () {
                        var userIdResponse;
                        var onUserAssigned = function (userId) {
                            userIdResponse = userId;
                        };
                        return {
                            promises: [BusinessProcess_BPTaskService.assignTask(onUserAssigned, userIdsResponse)],
                            getChildNode: function () {
                                return {
                                    promises: [assignTask(userIdResponse)],
                                    getChildNode: function () {
                                        return {
                                            promises: [load(true)],
                                            getChildNode: function () {
                                                if (assignTaskResponse != undefined) {
                                                    $scope.scopeModel.showTake = assignTaskResponse.ShowTake;
                                                    $scope.scopeModel.showRelease = assignTaskResponse.ShowRelease;
                                                    $scope.scopeModel.showAssign = assignTaskResponse.ShowAssign;
                                                }
                                                return {
                                                    promises: []
                                                };
                                            }
                                        };
                                    }
                                };
                            }
                        };
                    }
                };
                return UtilsService.waitPromiseNode(rootPromiseNode);
            };
        }
        function getBPTask() {
            return BusinessProcess_BPTaskAPIService.GetTask(bpTaskId).then(function (task) {
                bpTask = task;
            });
        }
        function getBPTaskType(typeId) {
            return BusinessProcess_BPTaskTypeAPIService.GetBPTaskType(typeId).then(function (taskType) {
                if (taskType != undefined) {
                    bpTaskType = taskType;
                }
            });
        }

        function setTitle() {
            $scope.title = bpTask.Title;
        }

        function load(isReload) {
            $scope.scopeModel.isLoading = true;
            if (isReload) {
                $scope.scopeModel.runtimeEditor = undefined;
                runtimeEditorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                bpTask = undefined;
            }
            var rootPromiseNode = {
                promises: [getBPTask(), getActions()],
                getChildNode: function () {
                    if (bpTask != undefined) {
                        fieldValues = bpTask.TaskData != undefined ? bpTask.TaskData.FieldValues : undefined;
                        if (!isReload)
                            setTitle();
                        return {
                            promises: [getBPTaskType(bpTask.TypeId)],
                            getChildNode: function () {
                                if (bpTaskType != undefined && bpTaskType.Settings != undefined) {
                                    $scope.scopeModel.runtimeEditor = bpTaskType.Settings.EditorSettings != undefined ? bpTaskType.Settings.EditorSettings.RuntimeEditor : undefined;
                                    $scope.scopeModel.includeTaskLock = bpTaskType.Settings.IncludeTaskLock;
                                    return {
                                        promises: !isReload ? [getInitialBPTaskDefaultActionsState(), loadRuntimeEditor()] : [loadRuntimeEditor()]
                                    };
                                }
                            }
                        };
                    }
                }
            };
            return UtilsService.waitPromiseNode(rootPromiseNode).catch(function (error) {
                VRNotificationService.notifyException(error);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function loadRuntimeEditor() {
            var runtimeEditorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
            runtimeEditorReadyPromiseDeferred.promise.then(function () {
                runtimeEditorReadyPromiseDeferred = undefined;
                var defaultValues = {};
                if (fieldValues != undefined) {
                    for (var prop in fieldValues) {
                        if (prop != "$type")
                            defaultValues[prop] = fieldValues[prop];
                    }
                }

                var runtimeEditorPayload = {
                    selectedValues: defaultValues,
                    dataRecordTypeId: bpTaskType != undefined && bpTaskType.Settings != undefined ? bpTaskType.Settings.RecordTypeId : undefined,
                    definitionSettings: bpTaskType != undefined && bpTaskType.Settings != undefined ? bpTaskType.Settings.EditorSettings : undefined
                };
                VRUIUtilsService.callDirectiveLoad(runtimeEditorAPI, runtimeEditorPayload, runtimeEditorLoadPromiseDeferred);
            });
            return runtimeEditorLoadPromiseDeferred.promise;
        }

        function getInitialBPTaskDefaultActionsState() {
            return BusinessProcess_BPTaskAPIService.GetInitialBPTaskDefaultActionsState(bpTask.TakenBy).then(function (response) {
                if (response != undefined) {
                    $scope.scopeModel.showTake = response.ShowTake;
                    $scope.scopeModel.showRelease = response.ShowRelease;
                    $scope.scopeModel.showAssign = response.ShowAssign;

                }
            });
        }
        function getFieldValues() {
            var fieldValuesObj = {};
            runtimeEditorAPI.setData(fieldValuesObj);
            return fieldValuesObj;
        }

        function getActions() {
            return BusinessProcess_BPGenericTaskTypeActionAPIService.GetTaskTypeActions(bpTaskId).then(function (actions) {
                if (actions != undefined && actions.length > 0) {
                    var actionsDictionary = {};
                    for (var i = 0; i < actions.length; i++) {
                        var taskTypeAction = actions[i];
                        var buttonType = UtilsService.getEnum(VRButtonTypeEnum, "value", taskTypeAction.ButtonType);

                        if (actionsDictionary[buttonType.value] == undefined) {
                            actionsDictionary[buttonType.value] = [];
                        }
                        actionsDictionary[buttonType.value].push({
                            buttonType: buttonType,
                            taskTypeAction: taskTypeAction,
                        });
                    }
                    $scope.scopeModel.actions.length = 0;
                    for (var prop in actionsDictionary) {
                        if (actionsDictionary[prop].length > 1) {
                            var menuActions = [];
                            for (var i = 0; i < actionsDictionary[prop].length; i++) {
                                if (menuActions == undefined)
                                    menuActions = [];
                                var object = actionsDictionary[prop][i];
                                menuActions.push({
                                    name: object.taskTypeAction.Name,
                                    clicked: function () {
                                        return callActionMethod(object.taskTypeAction);
                                    }
                                });
                            }
                            addActionToList(object.buttonType, undefined, menuActions);
                        } else {
                            var object = actionsDictionary[prop][0];
                            var clickFunc = function () {
                                return callActionMethod(object.taskTypeAction);
                            };
                            addActionToList(object.buttonType, clickFunc, undefined);
                        }
                    }
                }
            });
        }
       

        function addActionToList(buttonType, clickEvent, menuActions) {
            var type = buttonType != undefined ? buttonType.type : undefined;
            $scope.scopeModel.actions.push({
                type: type,
                onclick: clickEvent,
                menuActions: menuActions
            });
        }

        function callActionMethod(taskTypeAction) {
            var payload = {
                context: getContext(),
                taskId: bpTaskId,
                fieldValues: getFieldValues(),
            };
            var actionType = BusinessProcess_TaskTypeActionService.getActionTypeIfExist(taskTypeAction.Settings.ActionTypeName);
            var promise = actionType.actionMethod(payload);
            promise.then(function () {
            }).catch(function (error) {
                VRNotificationService.notifyException(error);
            });

            return promise;
        }

        function getContext() {
            return {
                closeModal: function () {
                    $scope.modalContext.closeModal();
                }
            };
        }

    }

    appControllers.controller('BusinessProcess_BP_BPGenericTaskTypeSettingsEditorController', BPGenericTaskTypeSettingsEditorController);
})(appControllers);
