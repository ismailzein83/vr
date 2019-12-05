﻿(function (appControllers) {

    "use strict";
    BusinessProcess_BPTaskService.$inject = ['LabelColorsEnum', 'BPTaskStatusEnum', 'VRModalService', 'BusinessProcess_BPTaskAPIService', 'BusinessProcess_BPTaskTypeAPIService', 'VRCommon_ModalWidthEnum', 'UtilsService', 'VR_GenericData_GenericBusinessEntityAPIService', 'VR_GenericData_GenericBEActionService'];
    function BusinessProcess_BPTaskService(LabelColorsEnum, BPTaskStatusEnum, VRModalService, BusinessProcess_BPTaskAPIService, BusinessProcess_BPTaskTypeAPIService, VRCommon_ModalWidthEnum, UtilsService, VR_GenericData_GenericBusinessEntityAPIService, VR_GenericData_GenericBEActionService) {
        function getStatusColor(status) {
            if (status === BPTaskStatusEnum.New.value) return LabelColorsEnum.Primary.color;
            if (status === BPTaskStatusEnum.Started.value) return LabelColorsEnum.Info.color;
            if (status === BPTaskStatusEnum.Completed.value) return LabelColorsEnum.Success.color;
            if (status === BPTaskStatusEnum.Cancelled.value) return LabelColorsEnum.Error.color;
            return LabelColorsEnum.Info.color;
        };

        function openTask(bpTaskId) {

            return BusinessProcess_BPTaskTypeAPIService.GetBPTaskTypeByTaskId(bpTaskId).then(function (bpTaskType) {
                var url = bpTaskType.Settings.Editor;

                var parameters = {
                    TaskId: bpTaskId
                };

                var editorEnum = UtilsService.getEnum(VRCommon_ModalWidthEnum, "value", bpTaskType.Settings.EditorSize);

                var settings = {
                    size: editorEnum != undefined ? editorEnum.modalAttr : "medium",
                    onScopeReady: function (modalScope) {
                        modalScope.title = "Task";
                    }
                };

                //VRModalService.showModal('/Client/Modules/BusinessProcess/Views/BPTask/BPTaskEditor.html', {
                VRModalService.showModal(url, parameters, settings);
            });

        };

        function assignTask(onUserAssigned, userIds) {
            var modalParameters = { userIds: userIds };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onUserAssigned = onUserAssigned;
            };

            return VRModalService.showModal('/Client/Modules/BusinessProcess/Views/BPTask/BPTaskAssignEditor.html', modalParameters, modalSettings);
        }

        function getTaskIdFieldType() {
            return {
                $type: "Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityType,Vanrise.GenericData.MainExtensions",
                ConfigId: "2e16c3d4-837b-4433-b80e-7c02f6d71467",
                RuntimeEditor: "vr-genericdata-fieldtype-businessentity-runtimeeditor",
                ViewerEditor: "vr-genericdata-fieldtype-businessentity-viewereditor",
                BusinessEntityDefinitionId: "d33fd65a-721f-4ae1-9d41-628be9425796"
            };
        }

        function registerOpenBPTaskBEAction() {
            var openBPTaskActionType = {
                ActionTypeName: "OpenBPTaskGenericBEAction",
                ExecuteAction: function (payload) {
                    if (payload == undefined)
                        return;

                    var fieldValues;
                    var getGenericBusinessEntityPromise = VR_GenericData_GenericBusinessEntityAPIService.GetGenericBusinessEntity(payload.businessEntityDefinitionId, payload.genericBusinessEntityId).then(function (response) {
                        if (response != undefined && response.FieldValues != undefined)
                            fieldValues = response.FieldValues;
                    });

                    return UtilsService.waitPromiseNode({
                        promises: [getGenericBusinessEntityPromise],
                        getChildNode: function () {
                            var childPromises = [];

                            if (fieldValues != undefined) {
                                var genericBEAction = payload.genericBEAction;
                                var taskIdFieldName = genericBEAction != undefined && genericBEAction.Settings != undefined ? genericBEAction.Settings.TaskIdFieldName : undefined;
                                var taskId = fieldValues[taskIdFieldName];

                                if (taskId != undefined) {
                                    childPromises.push(openTask(taskId));
                                }
                            }
                            return { promises: childPromises };
                        }
                    });
                }
            };

            VR_GenericData_GenericBEActionService.registerActionType(openBPTaskActionType);
        }
        return ({
            getStatusColor: getStatusColor,
            openTask: openTask,
            assignTask: assignTask,
            getTaskIdFieldType: getTaskIdFieldType,
            registerOpenBPTaskBEAction: registerOpenBPTaskBEAction
        });
    }
    appControllers.service('BusinessProcess_BPTaskService', BusinessProcess_BPTaskService);

})(appControllers);