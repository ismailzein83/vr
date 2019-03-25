(function (appControllers) {

    "use strict";

    BPGenericTaskTypeSettingsEditorController.$inject = ['$scope', 'BusinessProcess_BPTaskAPIService', 'BusinessProcess_BPTaskTypeAPIService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'VRButtonTypeEnum', 'BusinessProcess_TaskTypeActionService'];

    function BPGenericTaskTypeSettingsEditorController($scope, BusinessProcess_BPTaskAPIService, BusinessProcess_BPTaskTypeAPIService, VRNavigationService, UtilsService, VRUIUtilsService,
        VRNotificationService, VRButtonTypeEnum, BusinessProcess_TaskTypeActionService) {

        var bpTaskId;
        var bpTaskType;
        var fieldValues;

        var runtimeEditorAPI;
        var runtimeEditorReadyDeferred = UtilsService.createPromiseDeferred();

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
                runtimeEditorReadyDeferred.resolve();
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;

            var bpTask;
            function getBPTask() {
                return BusinessProcess_BPTaskAPIService.GetTask(bpTaskId).then(function (task) {
                    bpTask = task;
                });
            }

            function getBPTaskType(typeId) {
                return BusinessProcess_BPTaskTypeAPIService.GetBPTaskType(typeId).then(function (taskType) {
                    if (taskType != undefined) {
                        bpTaskType = taskType;
                        var actions = bpTaskType.Settings.TaskTypeActions;
                        var actionsDictionary = getActionsDictionary();
                        buildActionsFromDictionary(actionsDictionary);
                    }
                });
            }
            function setTitle() {
                $scope.title = bpTask.Title;
            }

            var rootPromiseNode = {
                promises: [getBPTask()],
                getChildNode: function () {
                    if (bpTask != undefined) {
                        fieldValues = bpTask.TaskData != undefined ? bpTask.TaskData.FieldValues : undefined;
                        setTitle();
                        return {
                            promises: [getBPTaskType(bpTask.TypeId)],
                            getChildNode: function () {
                                if (bpTaskType != undefined && bpTaskType.Settings != undefined && bpTaskType.Settings.EditorSettings != undefined) {
                                    $scope.scopeModel.runtimeEditor = bpTaskType.Settings.EditorSettings.RuntimeEditor;
                                    return {
                                        promises: [loadEditorRuntimeDirective()]
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

        function getFieldValues() {
            var fieldValuesObj = {};
            runtimeEditorAPI.setData(fieldValuesObj);
            return fieldValuesObj;
        }

        function loadEditorRuntimeDirective() {
            var runtimeEditorLoadDeferred = UtilsService.createPromiseDeferred();
            runtimeEditorReadyDeferred.promise.then(function () {
                var defaultValues = {};
                for (var prop in fieldValues) {
                    if (prop != "$type")
                        defaultValues[prop] = fieldValues[prop];
                }

                var runtimeEditorPayload = {
                    selectedValues: defaultValues,
                    dataRecordTypeId: bpTaskType != undefined && bpTaskType.Settings != undefined ? bpTaskType.Settings.RecordTypeId : undefined,
                    definitionSettings: bpTaskType != undefined && bpTaskType.Settings != undefined ? bpTaskType.Settings.EditorSettings : undefined
                };
                VRUIUtilsService.callDirectiveLoad(runtimeEditorAPI, runtimeEditorPayload, runtimeEditorLoadDeferred);
            });

            return runtimeEditorLoadDeferred.promise;
        }

        function getActionsDictionary() {
            var dictionay = {};
            var actions = bpTaskType.Settings.TaskTypeActions;
            for (var i = 0; i < actions.length; i++) {
                var taskTypeAction = actions[i];
                var buttonType = UtilsService.getEnum(VRButtonTypeEnum, "value", taskTypeAction.ButtonType);

                if (dictionay[buttonType.value] == undefined) {
                    dictionay[buttonType.value] = [];
                }
                dictionay[buttonType.value].push({
                    buttonType: buttonType,
                    taskTypeAction: taskTypeAction,
                });
            }
            return dictionay;
        }

        function buildActionsFromDictionary(actionsDictionary) {
            $scope.scopeModel.actions.length = 0;
            if (actionsDictionary != undefined) {
                for (var prop in actionsDictionary) {

                    if (actionsDictionary[prop].length > 1) {
                        var menuActions = [];
                        for (var i = 0; i < actionsDictionary[prop].length; i++) {
                            if (menuActions == undefined)
                                menuActions = [];
                            var object = actionsDictionary[prop][i];
                            addMenuAction(object.taskTypeAction);
                        }
                        function addMenuAction(taskTypeAction) {
                            menuActions.push({
                                name: taskTypeAction.Name,
                                clicked: function () {
                                    return callActionMethod(taskTypeAction);
                                },
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

            function addActionToList(buttonType, clickEvent, menuActions) {
                var type = buttonType != undefined ? buttonType.type : undefined;
                $scope.scopeModel.actions.push({
                    type: type,
                    onclick: clickEvent,
                    menuActions: menuActions
                });
            }

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
