(function (appControllers) {

    "use strict";

    businessProcessService.$inject = ['LabelColorsEnum', 'BPInstanceStatusEnum', 'VRModalService', 'UtilsService'];
    
    function businessProcessService(LabelColorsEnum, BPInstanceStatusEnum, VRModalService, UtilsService) {

        function getStatusColor(status) {

            if (status === BPInstanceStatusEnum.New.value) return LabelColorsEnum.Primary.Color;
            if (status === BPInstanceStatusEnum.Running.value) return LabelColorsEnum.Info.Color;
            if (status === BPInstanceStatusEnum.ProcessFailed.value) return LabelColorsEnum.Error.Color;
            if (status === BPInstanceStatusEnum.Completed.value) return LabelColorsEnum.Success.Color;
            if (status === BPInstanceStatusEnum.Aborted.value) return LabelColorsEnum.Warning.Color;
            if (status === BPInstanceStatusEnum.Suspended.value) return LabelColorsEnum.Warning.Color;
            if (status === BPInstanceStatusEnum.Terminated.value) return LabelColorsEnum.Error.Color;

            return LabelColorsEnum.Info.Color;
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