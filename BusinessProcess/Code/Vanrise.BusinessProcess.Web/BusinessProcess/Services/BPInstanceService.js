﻿(function (appControllers) {

    "use strict";
    BusinessProcess_BPInstanceService.$inject = ['LabelColorsEnum', 'BPInstanceStatusEnum','VRModalService'];
    function BusinessProcess_BPInstanceService(LabelColorsEnum, BPInstanceStatusEnum, VRModalService) {
        function getStatusColor(status) {

            if (status === BPInstanceStatusEnum.New.value) return LabelColorsEnum.Primary.color;
            if (status === BPInstanceStatusEnum.Running.value) return LabelColorsEnum.Info.color;
            if (status === BPInstanceStatusEnum.ProcessFailed.value) return LabelColorsEnum.Error.color;
            if (status === BPInstanceStatusEnum.Completed.value) return LabelColorsEnum.Success.color;
            if (status === BPInstanceStatusEnum.Aborted.value) return LabelColorsEnum.Warning.color;
            if (status === BPInstanceStatusEnum.Suspended.value) return LabelColorsEnum.Warning.color;
            if (status === BPInstanceStatusEnum.Terminated.value) return LabelColorsEnum.Error.color;

            return LabelColorsEnum.Info.color;
        };

        function openProcessTracking(processInstanceId) {

            VRModalService.showModal('/Client/Modules/BusinessProcess/Views/BPTrackingModal.html', {
                BPInstanceID: processInstanceId
            }, {
                onScopeReady: function (modalScope) {
                    modalScope.title = "Tracking";
                }
            });
        };

        return ({
            getStatusColor: getStatusColor,
            openProcessTracking: openProcessTracking
        });
    }
    appControllers.service('BusinessProcess_BPInstanceService', BusinessProcess_BPInstanceService);

})(appControllers);