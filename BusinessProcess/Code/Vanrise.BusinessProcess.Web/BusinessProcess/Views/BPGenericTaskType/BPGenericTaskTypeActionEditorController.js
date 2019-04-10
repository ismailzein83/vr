﻿(function (appControllers) {

    "use strict";

    BPGenericTaskTypeActionEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService','BusinessProcess_BPTaskTypeAPIService'];

    function BPGenericTaskTypeActionEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, BusinessProcess_BPTaskTypeAPIService) {

        var taskTypeActionEntity;
        var isEditMode;

        var buttonTypesSelectorAPI;
        var buttonTypesSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var actionSettingsSelectorAPI;
        var actionSettingsSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var actionFilterConditionSelectorAPI;
        var actionFilterConditionSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var requiredPermissionAPI;
        var requiredPermissionReadyDeferred = UtilsService.createPromiseDeferred();

        var context;
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            $scope.scopeModel = {};
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                taskTypeActionEntity = parameters.taskTypeActionEntity;
                context = parameters.context;
            }
            isEditMode = (taskTypeActionEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onButtonTypesSelectorReady = function (api) {
                buttonTypesSelectorAPI = api;
                buttonTypesSelectorReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onActionSettingsSelectorReady = function (api) {
                actionSettingsSelectorAPI = api;
                actionSettingsSelectorReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onActionFilterConditionSelectorReady = function (api) {
                actionFilterConditionSelectorAPI = api;
                actionFilterConditionSelectorReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onRequiredPermissionReady = function (api) {
                requiredPermissionAPI = api;
                requiredPermissionReadyDeferred.resolve();
            };

            $scope.scopeModel.save = function () {
                if (isEditMode) {
                    return update();
                }
                else {
                    return insert();
                }
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;

            loadAllControls()
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
                .finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
        }

        function loadAllControls() {

            var initialPromises = [];

            function setTitle() {
                if (taskTypeActionEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(taskTypeActionEntity.Name, 'Task Type Action');
                else
                    $scope.title = UtilsService.buildTitleForAddEditor('Task Type Action');
            }

            function loadStaticData() {
                if (!isEditMode)
                    return;

                $scope.scopeModel.Name = taskTypeActionEntity.Name;
            }

            function loadButtonTypesSelector() {
                var buttonTypesSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                buttonTypesSelectorReadyPromiseDeferred.promise.then(function () {
                    var buttonTypesSelectorPayload = { };
                    if (taskTypeActionEntity != undefined) {
                        buttonTypesSelectorPayload.selectedIds = taskTypeActionEntity.ButtonType;
                    }
                    VRUIUtilsService.callDirectiveLoad(buttonTypesSelectorAPI, buttonTypesSelectorPayload, buttonTypesSelectorLoadPromiseDeferred);
                });
                return buttonTypesSelectorLoadPromiseDeferred.promise;
            }

            function loadActionSettingsSelector() {
                var actionSettingsSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                actionSettingsSelectorReadyPromiseDeferred.promise.then(function () {
                    var actionSettingsSelectorPayload = { context: getContext() };
                    if (taskTypeActionEntity != undefined) {
                        actionSettingsSelectorPayload.settings = taskTypeActionEntity.Settings;
                    }
                    VRUIUtilsService.callDirectiveLoad(actionSettingsSelectorAPI, actionSettingsSelectorPayload, actionSettingsSelectorLoadPromiseDeferred);
                });
                return actionSettingsSelectorLoadPromiseDeferred.promise;
            }

            function loadActionFilterConditionSelector() {
                var actionFilterConditionSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                actionFilterConditionSelectorReadyPromiseDeferred.promise.then(function () {
                    var actionFilterConditionSelectorPayload = { context: getContext()};
                    if (taskTypeActionEntity != undefined) {
                        actionFilterConditionSelectorPayload.filterCondition = taskTypeActionEntity.FilterCondition;
                    }
                    VRUIUtilsService.callDirectiveLoad(actionFilterConditionSelectorAPI, actionFilterConditionSelectorPayload, actionFilterConditionSelectorLoadPromiseDeferred);
                });
                return actionFilterConditionSelectorLoadPromiseDeferred.promise;
            }

            function loadRequiredPermission() {
                var requiredPermissionLoadDeferred = UtilsService.createPromiseDeferred();

                requiredPermissionReadyDeferred.promise.then(function () {
                    var payload;

                    if (taskTypeActionEntity != undefined && taskTypeActionEntity.RequiredPermission != undefined) {
                        payload = {
                            data: taskTypeActionEntity.RequiredPermission
                        };
                    }

                    VRUIUtilsService.callDirectiveLoad(requiredPermissionAPI, payload, requiredPermissionLoadDeferred);
                });

                return requiredPermissionLoadDeferred.promise;
            }


            var rootPromiseNode = {
                promises: initialPromises,
                getChildNode: function () {
                    return {
                        promises: [UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadButtonTypesSelector, loadActionSettingsSelector, loadActionFilterConditionSelector, loadRequiredPermission])]
                    };
                }
            };

            return UtilsService.waitPromiseNode(rootPromiseNode);
        }


        function insert() {
            var taskTypeActionObject = buildTaskTypeActionObjFromScope();

            if ($scope.onBPTaskTypeActionAdded != undefined && typeof ($scope.onBPTaskTypeActionAdded) == 'function') {
                $scope.onBPTaskTypeActionAdded(taskTypeActionObject);
            }
            $scope.modalContext.closeModal();
        }

        function update() {
            var taskTypeActionObject = buildTaskTypeActionObjFromScope();

            if ($scope.onBPTaskTypeActionUpdated != undefined && typeof ($scope.onBPTaskTypeActionUpdated) == 'function') {
                $scope.onBPTaskTypeActionUpdated(taskTypeActionObject);
            }
            $scope.modalContext.closeModal();
        }

        function buildTaskTypeActionObjFromScope() {
            var obj = {};
            obj.TaskTypeActionId = taskTypeActionEntity != undefined ? taskTypeActionEntity.TaskTypeActionId : UtilsService.guid();
            obj.Name = $scope.scopeModel.Name;
            obj.ButtonType = buttonTypesSelectorAPI.getSelectedIds();
            obj.Settings = actionSettingsSelectorAPI.getData();
            obj.FilterCondition = actionFilterConditionSelectorAPI.getData();
            obj.RequiredPermission = requiredPermissionAPI.getData();

            return obj;
        }

        function getContext() {
            var currentContext = context;
            if (currentContext == undefined)
                currentContext = {};
            return currentContext;
        }

    }

    appControllers.controller('BPGenericTaskType_ActionEditorController', BPGenericTaskTypeActionEditorController);
})(appControllers);