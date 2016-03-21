(function (appControllers) {

    "use strict";
    BusinessProcess_BPValidationMessageService.$inject = ['LabelColorsEnum', 'BPActionSeverityEnum'];
    function BusinessProcess_BPValidationMessageService(LabelColorsEnum, BPActionSeverityEnum) {
        function getSeverityColor(severity) {
            if (severity === BPActionSeverityEnum.Information.value) return LabelColorsEnum.Info.color;
            if (severity === BPActionSeverityEnum.Warning.value) return LabelColorsEnum.Warning.color;
            if (severity === BPActionSeverityEnum.Error.value) return LabelColorsEnum.Error.color;
            return LabelColorsEnum.Info.color;
        };

        return ({
            getSeverityColor: getSeverityColor
        });
    }
    appControllers.service('BusinessProcess_BPValidationMessageService', BusinessProcess_BPValidationMessageService);

})(appControllers);