(function (appControllers) {

    "use strict";
    BusinessProcess_BPTaskService.$inject = ['LabelColorsEnum', 'BPTaskStatusEnum', 'VRModalService', 'BusinessProcess_BPTaskAPIService', 'BusinessProcess_BPTaskTypeAPIService'];
    function BusinessProcess_BPTaskService(LabelColorsEnum, BPTaskStatusEnum, VRModalService, BusinessProcess_BPTaskAPIService, BusinessProcess_BPTaskTypeAPIService) {
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

                //VRModalService.showModal('/Client/Modules/BusinessProcess/Views/BPTask/BPTaskEditor.html', {
                VRModalService.showModal(url, {
                    TaskId: bpTaskId
                }, {
                    onScopeReady: function (modalScope) {
                        modalScope.title = "Task";
                    }
                });
            });

        };

        function assignTask(onUserAssigned, userIds) {
            var modalParameters = { userIds: userIds};

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onUserAssigned = onUserAssigned;
            };

            VRModalService.showModal('/Client/Modules/BusinessProcess/Views/BPTask/BPTaskAssignEditor.html', modalParameters, modalSettings);
        }

        return ({
            getStatusColor: getStatusColor,
            openTask: openTask,
            assignTask: assignTask
        });
    }
    appControllers.service('BusinessProcess_BPTaskService', BusinessProcess_BPTaskService);

})(appControllers);