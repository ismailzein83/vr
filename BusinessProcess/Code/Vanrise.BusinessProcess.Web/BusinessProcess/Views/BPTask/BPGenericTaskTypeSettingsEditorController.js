(function (appControllers) {

    "use strict";

    BPGenericTaskTypeSettingsEditorController.$inject = ['$scope', 'BusinessProcess_BPTaskAPIService', 'BusinessProcess_BPTaskTypeAPIService', 'VRNavigationService', 'UtilsService','VRUIUtilsService'];

    function BPGenericTaskTypeSettingsEditorController($scope, BusinessProcess_BPTaskAPIService, BusinessProcess_BPTaskTypeAPIService, VRNavigationService, UtilsService, VRUIUtilsService) {

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

            $scope.scopeModel.onEditorRuntimeDirectiveReady = function (api) {
                runtimeEditorAPI = api;
                runtimeEditorReadyDeferred.resolve();
            };

            $scope.scopeModel.continueTask = function () {
                return executeTask();
            };

        }

        function executeTask() {
            var input = {
                $type: "Vanrise.BusinessProcess.Entities.ExecuteBPTaskInput, Vanrise.BusinessProcess.Entities",
                TaskId: bpTaskId,
                TaskData: {
                    $type: "Vanrise.BusinessProcess.MainExtensions.BPTaskTypes.BPGenericTaskData, Vanrise.BusinessProcess.MainExtensions",
                    FieldValues: getFieldValues()
                }
            };

            return BusinessProcess_BPTaskAPIService.ExecuteTask(input).then(function (response) {
                $scope.modalContext.closeModal();
            }).catch(function (error) {
                VRNotificationService.notifyException(error);
            });
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            BusinessProcess_BPTaskAPIService.GetTask(bpTaskId).then(function (bpTask) {
                if (bpTask != undefined) {
                    fieldValues = bpTask.TaskData != undefined ? bpTask.TaskData.FieldValues : undefined;
                    BusinessProcess_BPTaskTypeAPIService.GetBPTaskType(bpTask.TypeId).then(function (taskType) {
                        bpTaskType = taskType;
                        if (bpTaskType != undefined && bpTaskType.Settings != undefined && bpTaskType.Settings.EditorSettings != undefined) {
                            $scope.scopeModel.runtimeEditor = bpTaskType.Settings.EditorSettings.RuntimeEditor;
                            loadEditorRuntimeDirective();
                        }
                    }).catch(function (error) {
                        VRNotificationService.notifyException(error);
                    }).finally(function () {
                        $scope.scopeModel.isLoading = false;
                    });
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error);
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
    }

    appControllers.controller('BusinessProcess_BP_BPGenericTaskTypeSettingsEditorController', BPGenericTaskTypeSettingsEditorController);
})(appControllers);
