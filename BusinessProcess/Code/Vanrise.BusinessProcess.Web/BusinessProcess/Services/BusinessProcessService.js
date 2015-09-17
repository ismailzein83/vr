(function (appControllers) {

    "use strict";

    businessProcessService.$inject = ['LabelColorsEnum', 'BPInstanceStatusEnum', 'VRModalService', 'UtilsService'];
    
    function businessProcessService(LabelColorsEnum, BPInstanceStatusEnum, VRModalService, UtilsService) {

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

        function getStatusDescription(status) {
            if (status) {
                var statusEnum = UtilsService.getEnum(BPInstanceStatusEnum, 'value', status);

                if (statusEnum) return statusEnum.description;
            }
            return undefined;
        }

        function openProcessTracking(processInstanceId) {
            
            VRModalService.showModal('/Client/Modules/BusinessProcess/Views/BPTrackingModal.html', {
                BPInstanceID: processInstanceId
            }, {
                onScopeReady: function (modalScope) {
                    modalScope.title = "Tracking";
                }
            });

        }

        return ({
            getStatusColor: getStatusColor,
            getStatusDescription: getStatusDescription,
            openProcessTracking: openProcessTracking
        });
    }
    appControllers.service('BusinessProcessService', businessProcessService);

})(appControllers);