(function (appControllers) {

    "use strict";
    BusinessProcess_BPTaskService.$inject = ['LabelColorsEnum', 'BPTaskStatusEnum', 'VRModalService','BusinessProcess_BPTaskAPIService'];
    function BusinessProcess_BPTaskService(LabelColorsEnum, BPTaskStatusEnum, VRModalService, BusinessProcess_BPTaskAPIService) {
        function getStatusColor(status) {
            if (status === BPTaskStatusEnum.New.value) return LabelColorsEnum.Primary.color;
            if (status === BPTaskStatusEnum.Started.value) return LabelColorsEnum.Info.color;
            if (status === BPTaskStatusEnum.Completed.value) return LabelColorsEnum.Success.color;
            if (status === BPTaskStatusEnum.Cancelled.value) return LabelColorsEnum.Error.color;
            return LabelColorsEnum.Info.color;
        };

        function openTask(bpTaskId, bpTaskTypeId) {
            VRModalService.showModal('/Client/Modules/BusinessProcess/Views/BPTask/BPTaskEditor.html', {
                TaskId: bpTaskId,
                TaskTypeId: bpTaskTypeId
            }, {
                onScopeReady: function (modalScope) {
                    modalScope.title = "Task";
                }
            });
        };

        return ({
            getStatusColor: getStatusColor,
            openTask: openTask
        });
    }
    appControllers.service('BusinessProcess_BPTaskService', BusinessProcess_BPTaskService);

})(appControllers);