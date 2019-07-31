(function (appControllers) {

    "use strict";
    BusinessProcess_BPTaskService.$inject = ['LabelColorsEnum', 'BPTaskStatusEnum', 'VRModalService', 'BusinessProcess_BPTaskAPIService', 'BusinessProcess_BPTaskTypeAPIService', 'VRCommon_ModalWidthEnum','UtilsService'];
    function BusinessProcess_BPTaskService(LabelColorsEnum, BPTaskStatusEnum, VRModalService, BusinessProcess_BPTaskAPIService, BusinessProcess_BPTaskTypeAPIService, VRCommon_ModalWidthEnum, UtilsService) {
        function getStatusColor(status) {
            if (status === BPTaskStatusEnum.New.value) return LabelColorsEnum.Primary.color;
            if (status === BPTaskStatusEnum.Started.value) return LabelColorsEnum.Info.color;
            if (status === BPTaskStatusEnum.Completed.value) return LabelColorsEnum.Success.color;
            if (status === BPTaskStatusEnum.Cancelled.value) return LabelColorsEnum.Error.color;
            return LabelColorsEnum.Info.color;
        };

        function openTask(bpTaskId) {

            BusinessProcess_BPTaskTypeAPIService.GetBPTaskTypeByTaskId(bpTaskId).then(function (bpTaskType) {
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
            var modalParameters = { userIds: userIds};

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onUserAssigned = onUserAssigned;
            };

            return VRModalService.showModal('/Client/Modules/BusinessProcess/Views/BPTask/BPTaskAssignEditor.html', modalParameters, modalSettings);
        }

        function getTaskIdFieldType() {
            return {
                $type: "Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType,Vanrise.GenericData.MainExtensions",
                ConfigId: "75aef329-27bd-4108-b617-f5cc05ff2aa3",
                RuntimeEditor: "vr-genericdata-fieldtype-number-runtimeeditor",
                ViewerEditor: "vr-genericdata-fieldtype-number-viewereditor",
                DataType: 2
            }
        }
        return ({
            getStatusColor: getStatusColor,
            openTask: openTask,
            assignTask: assignTask,
            getTaskIdFieldType: getTaskIdFieldType
        });
    }
    appControllers.service('BusinessProcess_BPTaskService', BusinessProcess_BPTaskService);

})(appControllers);