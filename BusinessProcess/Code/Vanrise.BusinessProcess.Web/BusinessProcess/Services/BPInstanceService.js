﻿(function (appControllers) {

    "use strict";

    BusinessProcess_BPInstanceService.$inject = ['LabelColorsEnum', 'BPInstanceStatusEnum', 'VRModalService', 'BusinessProcess_BPInstanceAPIService', 'UtilsService', 'VR_Runtime_SchedulerTaskService', 'VR_GenericData_GenericBusinessEntityAPIService', 'VR_GenericData_GenericBEActionService', 'BusinessProcess_BPDefinitionAPIService', 'VRCommon_ModalWidthEnum'];

    function BusinessProcess_BPInstanceService(LabelColorsEnum, BPInstanceStatusEnum, VRModalService, BusinessProcess_BPInstanceAPIService, UtilsService, VR_Runtime_SchedulerTaskService, VR_GenericData_GenericBusinessEntityAPIService, VR_GenericData_GenericBEActionService, BusinessProcess_BPDefinitionAPIService, VRCommon_ModalWidthEnum) {
        function getStatusColor(status) {

            if (status === BPInstanceStatusEnum.New.value) return LabelColorsEnum.New.color;
            if (status === BPInstanceStatusEnum.Postponed.value) return LabelColorsEnum.WarningLevel1.color;
            if (status === BPInstanceStatusEnum.Running.value) return LabelColorsEnum.Processing.color;
            if (status === BPInstanceStatusEnum.Waiting.value) return LabelColorsEnum.WarningLevel1.color;
            if (status === BPInstanceStatusEnum.Cancelling.value) return LabelColorsEnum.WarningLevel2.color;
            if (status === BPInstanceStatusEnum.Completed.value) return LabelColorsEnum.Success.color;
            if (status === BPInstanceStatusEnum.Aborted.value) return LabelColorsEnum.Error.color;
            if (status === BPInstanceStatusEnum.Suspended.value) return LabelColorsEnum.Error.color;
            if (status === BPInstanceStatusEnum.Terminated.value) return LabelColorsEnum.Error.color;
            if (status === BPInstanceStatusEnum.Cancelled.value) return LabelColorsEnum.Error.color;

            return LabelColorsEnum.Info.color;
        };

        function openProcessTracking(processInstanceId, context) {

            //VRModalService.showModal('/Client/Modules/BusinessProcess/Views/BPTrackingModal.html', {
            VRModalService.showModal('/Client/Modules/BusinessProcess/Views/BPInstance/BPInstanceTrackingModal.html', {
                BPInstanceID: processInstanceId,
                context: context
            }, {
                    onScopeReady: function (modalScope) {
                        modalScope.title = "Business Process Progress: ";
                    }
                });
        };

        function startNewInstance(bpDefinitionObj, onProcessInputCreated, onProcessInputsCreated) {
            var editorEnum = UtilsService.getEnum(VRCommon_ModalWidthEnum, "value", bpDefinitionObj.Entity.Configuration.EditorSize);
            var editorSize = editorEnum != undefined ? editorEnum.modalAttr : undefined;

            var modalSettings = {
                size: editorSize != undefined ? editorSize : "medium"
            };
            modalSettings.onScopeReady = function (modalScope) {
                modalScope.title = 'Start New Instance';
                modalScope.onProcessInputCreated = onProcessInputCreated;
                modalScope.onProcessInputsCreated = onProcessInputsCreated;
            };
            var parameters = {
                BPDefinitionID: bpDefinitionObj.Entity.BPDefinitionID
            };

            VRModalService.showModal('/Client/Modules/BusinessProcess/Views/NewInstanceEditor/NewInstanceEditor.html', parameters, modalSettings);
        }

        function registerDrillDownToSchdeulerTask() {
            var drillDownDefinition = {};

            drillDownDefinition.title = "Recent Instances";
            drillDownDefinition.directive = "businessprocess-bp-instance-monitor-grid";
            drillDownDefinition.hideDrillDownFunction = function (bpInstanceItem) {
                return bpInstanceItem.Entity.ActionTypeId.toUpperCase() != '7A35F562-319B-47B3-8258-EC1A704A82EB';
            };
            drillDownDefinition.loadDirective = function (directiveAPI, bpInstanceItem) {
                bpInstanceItem.bpInstanceGridAPI = directiveAPI;

                var payload = { TaskId: bpInstanceItem.Entity.TaskId };
                return bpInstanceItem.bpInstanceGridAPI.loadGrid(payload);
            };

            VR_Runtime_SchedulerTaskService.addDrillDownDefinition(drillDownDefinition);
        }

        function displayRunningInstancesIfExist(definitionId, entityIds, runningInstanceEditorSettings) {
            var displayRunningInstancePromiseDeferred = UtilsService.createPromiseDeferred();
            var hasRunningInstancesInput = {
                definitionId: definitionId,
                entityIds: entityIds
            };
            BusinessProcess_BPInstanceAPIService.HasRunningInstances(hasRunningInstancesInput).then(
                function (response) {
                    var result = { hasRunningProcesses: false };
                    if (response == true) {
                        var parameters = {
                            EntityIds: entityIds,
                            context: {
                                onClose: function () {
                                    BusinessProcess_BPInstanceAPIService.HasRunningInstances(hasRunningInstancesInput).then(
                                        function (response) {
                                            result.hasRunningProcesses = response;
                                            displayRunningInstancePromiseDeferred.resolve(result);
                                        });
                                }
                            },
                            runningInstanceEditorSettings: runningInstanceEditorSettings
                        };
                        VRModalService.showModal('/Client/Modules/BusinessProcess/Views/BPInstance/BPInstanceRunningProcesses.html', parameters, null);
                    }
                    else {
                        displayRunningInstancePromiseDeferred.resolve(result);
                    }

                });
            return displayRunningInstancePromiseDeferred.promise;
        }

        function registerOpenBPInstanceViewerAction() {
            var openBPInstanceViewerAction = {
                ActionTypeName: "OpenBPInstanceViewer",
                ExecuteAction: function (payload) {
                    if (payload != undefined) {
                        var businessEntityDefinitionId = payload.businessEntityDefinitionId;
                        var genericBusinessEntityId = payload.genericBusinessEntityId;
                        var promiseDeferred = UtilsService.createPromiseDeferred();

                        VR_GenericData_GenericBusinessEntityAPIService.GetGenericBusinessEntityDetail(genericBusinessEntityId, businessEntityDefinitionId).then(function (response) {
                            if (payload.genericBEAction != undefined && payload.genericBEAction.Settings != undefined) {
                                var processInstanceFieldName = payload.genericBEAction.Settings.ProcessInstanceIdFieldName;
                                if (processInstanceFieldName != undefined && response.FieldValues != undefined && response.FieldValues[processInstanceFieldName] != undefined) {
                                    var context = {
                                        onBPInstanceViewerClosed : function () {
                                            promiseDeferred.resolve(true);
                                        }
                                    };
                                    openProcessTracking(response.FieldValues[processInstanceFieldName].Value, context);
                                } else
                                    promiseDeferred.reject();
                            } else
                                promiseDeferred.reject();
                        }).catch(function () {
                            promiseDeferred.reject();
                            });
                        return promiseDeferred.promise;

                    }
                }
            };
            VR_GenericData_GenericBEActionService.registerActionType(openBPInstanceViewerAction);
        }

        function registerStartBPProcessAction() {
            var actionType = {
                ActionTypeName: "StartBPProcess",
                ExecuteAction: function (payload) {
                    if (payload == undefined)
                        return;

                    var promiseDeferred = UtilsService.createPromiseDeferred();
                    var businessEntityDefinitionId = payload.businessEntityDefinitionId;
                    var genericBusinessEntityId = payload.genericBusinessEntityId;
                    if (payload.genericBEAction != undefined && payload.genericBEAction.Settings != undefined) {
                        var genericBEActionSettings = payload.genericBEAction.Settings;
                        BusinessProcess_BPDefinitionAPIService.GetBPDefintion(genericBEActionSettings.BPDefinitionId).then(function (response) {
                            var editorEnum = UtilsService.getEnum(VRCommon_ModalWidthEnum, "value", response.Configuration.EditorSize);
                            var editorSize = editorEnum != undefined ? editorEnum.modalAttr : undefined;
                            var context = {
                                onBPInstanceViewerClosed: function () {
                                    promiseDeferred.resolve(true);
                                }
                            };
                            startBPProcessAction(businessEntityDefinitionId, genericBusinessEntityId, genericBEActionSettings, context, editorSize).then(function (response) {
                                if (response)
                                    promiseDeferred.resolve();
                            });
                        }).catch(function () {
                            promiseDeferred.reject();
                        });
                    }
                    return promiseDeferred.promise;
                   
                }
            };
            VR_GenericData_GenericBEActionService.registerActionType(actionType);
        }

        function startBPProcessAction(businessEntityDefinitionId, genericBusinessEntityId, genericBEActionSettings, context, editorSize) {
            var parameters = {
                businessEntityDefinitionId: businessEntityDefinitionId,
                genericBusinessEntityId: genericBusinessEntityId,
                genericBEActionSettings: genericBEActionSettings,
                context: context
            };
            var settings = {
                size: editorSize != undefined ? editorSize : "medium"
            };

            settings.onScopeReady = function (modalScope) {
            };
           
           return VRModalService.showModal('/Client/Modules/BusinessProcess/Views/BPProcess/Templates/StartBPProcessEditor.html', parameters, settings);
        }


        return ({
            getStatusColor: getStatusColor,
            openProcessTracking: openProcessTracking,
            startNewInstance: startNewInstance,
            registerDrillDownToSchdeulerTask: registerDrillDownToSchdeulerTask,
            displayRunningInstancesIfExist: displayRunningInstancesIfExist,
            registerOpenBPInstanceViewerAction: registerOpenBPInstanceViewerAction,
            registerStartBPProcessAction: registerStartBPProcessAction
        });
    }

    appControllers.service('BusinessProcess_BPInstanceService', BusinessProcess_BPInstanceService);
})(appControllers);