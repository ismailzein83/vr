(function (appControllers) {

    "use strict";

    BPGenericTaskTypeSettingsEditorController.$inject = ['$scope', 'BusinessProcess_BPTaskAPIService', 'BusinessProcess_BPTaskTypeAPIService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'VRButtonTypeEnum', 'BusinessProcess_TaskTypeActionService', 'BusinessProcess_BPTaskService', 'BusinessProcess_BPGenericTaskTypeActionAPIService', 'BPTaskStatusEnum'];

    function BPGenericTaskTypeSettingsEditorController($scope, BusinessProcess_BPTaskAPIService, BusinessProcess_BPTaskTypeAPIService, VRNavigationService, UtilsService, VRUIUtilsService, VRNotificationService, VRButtonTypeEnum, BusinessProcess_TaskTypeActionService, BusinessProcess_BPTaskService, BusinessProcess_BPGenericTaskTypeActionAPIService, BPTaskStatusEnum) {

        var bpTaskId;
        var bpTaskType;
        var fieldValues;
        var bpTask;

        var runtimeEditorAPI;
        var runtimeEditorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var bpTaskTypeActions;

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
                                    promises: userIdResponse!=undefined ? [assignTask(userIdResponse)] : [],
                                    getChildNode: function () {
                                        return {
                                            promises: userIdResponse!=undefined ? [load(true)] : [],
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

                        if (bpTask.Status == BPTaskStatusEnum.New.value || bpTask.Status == BPTaskStatusEnum.Started.value) {
                            $scope.scopeModel.showActionBar = true;
                        }
                        else {
                            $scope.scopeModel.showActionBar = false;
                        }

                        fieldValues = bpTask.TaskData != undefined ? bpTask.TaskData.FieldValues : undefined;
                        if (fieldValues != undefined) {
                            var fieldValueObjects = {};
                            for (var key in fieldValues) {
                                if (key != "$type") {
                                    fieldValueObjects[key] = {
                                        value: fieldValues[key],
                                        isHidden: false,
                                        isDisabled: false
                                    };
                                }
                            }
                            fieldValues = fieldValueObjects;
                        }
                        if (!isReload)
                            setTitle();
                          if (bpTaskTypeActions != undefined && bpTaskTypeActions.length > 0) {
                                for (var i = 0; i < bpTaskTypeActions.length; i++) {
                                    var action = bpTaskTypeActions[i];
                                    if (action.Settings != undefined && action.Settings.DefaultFieldValues != undefined && action.Settings.DefaultFieldValues.length > 0) {
                                        for (var j = 0; j < action.Settings.DefaultFieldValues.length; j++) {
                                            var defaultValue = action.Settings.DefaultFieldValues[j];
                                            if (fieldValues == undefined)
                                                fieldValues = {};
                                            if (fieldValues[defaultValue.FieldName] == undefined)
                                                fieldValues[defaultValue.FieldName] = {};
                                            fieldValues[defaultValue.FieldName].isHidden = true;
                                        }
                                    }
                                }
                        }
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
                    for (var key in fieldValues) {
                        if (key != "$type" && !fieldValues[key].isHidden)
                            defaultValues[key] = fieldValues[key].value;
                    }
                }

                var runtimeEditorPayload = {
                    selectedValues: defaultValues,
                    dataRecordTypeId: bpTaskType != undefined && bpTaskType.Settings != undefined ? bpTaskType.Settings.RecordTypeId : undefined,
                    definitionSettings: bpTaskType != undefined && bpTaskType.Settings != undefined ? bpTaskType.Settings.EditorSettings : undefined,
                    parentFieldValues: fieldValues
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
                var menuActions;
                function addMenuActions(taskTypeAction) {
                    menuActions.push({
                        name: taskTypeAction.Name,
                        clicked: function () {
                            return callActionMethod(taskTypeAction);
                        }
                    });
                }
                $scope.scopeModel.actions.length = 0;
                if (actions != undefined && actions.length > 0) {
                    menuActions = [];
                    bpTaskTypeActions = actions;
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
                    for (var prop in actionsDictionary) {
                        if (actionsDictionary[prop].length > 1) {
                            for (var i = 0; i < actionsDictionary[prop].length; i++) {
                                if (menuActions == undefined)
                                    menuActions = [];
                                var object = actionsDictionary[prop][i];
                                addMenuActions(object.taskTypeAction);
                            }
                            addActionToList(object.buttonType, undefined, menuActions);
                        } else {
                            var object = actionsDictionary[prop][0];
                            addActionToList(object.buttonType, getClickFunction(object.taskTypeAction), undefined);
                        }
                    }
                }
            });
        }
        function getClickFunction(action) {
            var clickFunc = function () {
                return callActionMethod(action);
            };
            return clickFunc;
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
            var promises = [];
            var runtimeValues = getFieldValues();
            if (taskTypeAction.Settings != undefined && taskTypeAction.Settings.DefaultFieldValues != undefined && taskTypeAction.Settings.DefaultFieldValues.length > 0) {
                if (runtimeValues == undefined)
                    runtimeValues = {};
                for (var i = 0; i < taskTypeAction.Settings.DefaultFieldValues.length; i++) {
                    var defaultFieldValue = taskTypeAction.Settings.DefaultFieldValues[i];
                    runtimeValues[defaultFieldValue.FieldName] = defaultFieldValue.FieldValue;
                }
            }

            var decisionMappingFieldDescription;
            var notesMappingFieldDescription;
            function getMappingFieldsDescription(input) {
                return BusinessProcess_BPGenericTaskTypeActionAPIService.GetMappingFieldsDescription(input).then(function (response) {
                    if (response != undefined) {
                        decisionMappingFieldDescription = response.DecisionFieldDescription;
                        notesMappingFieldDescription = response.NotesFieldDescription;
                    }
                });
            }
            if (taskTypeAction.Settings.DecisionMappingField != undefined || taskTypeAction.Settings.NotesMappingField != undefined) {
                var decisionFieldValue;
                var notesFieldValue;
                if (taskTypeAction.Settings.DecisionMappingField != undefined) {
                    decisionFieldValue = fieldValues != undefined ? fieldValues[taskTypeAction.Settings.DecisionMappingField] : undefined;
                    if (decisionFieldValue == undefined) {
                        decisionFieldValue = runtimeValues != undefined ? { value: runtimeValues[taskTypeAction.Settings.DecisionMappingField] } : undefined;
                    }
                }
                if (taskTypeAction.Settings.NotesMappingField != undefined) {
                    notesFieldValue = fieldValues != undefined ? fieldValues[taskTypeAction.Settings.NotesMappingField] : undefined;
                    if (notesFieldValue == undefined) {
                        notesFieldValue = runtimeValues != undefined ? { value: runtimeValues[taskTypeAction.Settings.NotesMappingField] } : undefined;
                    }
                }
                
                var input = {
                    DecisionFieldName: taskTypeAction.Settings.DecisionMappingField,
                    NotesFieldName: taskTypeAction.Settings.NotesMappingField,
                    DecisionFieldValue: decisionFieldValue != undefined ? decisionFieldValue.value : undefined,
                    NotesFieldValue: notesFieldValue != undefined ? notesFieldValue.value : undefined,
                    DataRecordTypeId: bpTaskType != undefined && bpTaskType.Settings != undefined ? bpTaskType.Settings.RecordTypeId : undefined
                };
                promises.push(getMappingFieldsDescription(input));
            }


            var actionType = BusinessProcess_TaskTypeActionService.getActionTypeIfExist(taskTypeAction.Settings.ActionTypeName);
          
            var rootPromiseNode = {
                promises: promises,
                getChildNode: function () {
                    var payload = {
                        context: getContext(),
                        taskId: bpTaskId,
                        fieldValues: runtimeValues,
                        notes: notesMappingFieldDescription,
                        decision: decisionMappingFieldDescription
                    };
                    return {
                        promises: [actionType.actionMethod(payload)]
                    };
                }
            };
            return UtilsService.waitPromiseNode(rootPromiseNode);
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
