(function (appControllers) {

    "use strict";

    businessProcessService.$inject = ['LabelColorsEnum', 'BPInstanceStatusEnum'];
    appControllers.service('BusinessProcessService', businessProcessService);

    function businessProcessService(LabelColorsEnum, BPInstanceStatusEnum) {

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
                if (status === BPInstanceStatusEnum.New.value) return BPInstanceStatusEnum.New.description;
                if (status === BPInstanceStatusEnum.Running.value) return BPInstanceStatusEnum.Running.description;
                if (status === BPInstanceStatusEnum.ProcessFailed.value) return BPInstanceStatusEnum.ProcessFailed.description;
                if (status === BPInstanceStatusEnum.Completed.value) return BPInstanceStatusEnum.Completed.description;
                if (status === BPInstanceStatusEnum.Aborted.value) return BPInstanceStatusEnum.Aborted.description;
                if (status === BPInstanceStatusEnum.Suspended.value) return BPInstanceStatusEnum.Suspended.description;
                if (status === BPInstanceStatusEnum.Terminated.value) return BPInstanceStatusEnum.Terminated.description;
            }
            return '';
        }

        return ({
            getStatusColor: getStatusColor,
            getStatusDescription: getStatusDescription
        });
    }


})(appControllers);