(function (appControllers) {

    "use strict";

    analyticsServiceObj.$inject = ['LabelColorsEnum', 'GenericAnalyticMeasureEnum'];

    function analyticsServiceObj(LabelColorsEnum, GenericAnalyticMeasureEnum) {
   
        function getACDColor(acdValue, attemptsValue, parameters) {
            if (attemptsValue > parameters.attempts && acdValue < parameters.acd)
                return LabelColorsEnum.WarningLevel1.color;
            return undefined;
        }

        function getASRColor(asrValue, attemptsValue, parameters) {
            if (attemptsValue > parameters.attempts && asrValue < parameters.asr)
                return LabelColorsEnum.WarningLevel2.color;
            return undefined;
        }

        function getMeasureColor(dataItem, coldef,parameters) {
            if (coldef.tag.value === GenericAnalyticMeasureEnum.ACD.value)
                return getACDColor(dataItem.MeasureValues[GenericAnalyticMeasureEnum.ACD.name], dataItem.MeasureValues[GenericAnalyticMeasureEnum.Attempts.name], parameters);
            else if (coldef.tag.value === GenericAnalyticMeasureEnum.ASR.value)
                return getASRColor(dataItem.MeasureValues[GenericAnalyticMeasureEnum.ASR.name], dataItem.MeasureValues[GenericAnalyticMeasureEnum.Attempts.name], parameters);
            return undefined;
        }


        return {
            getMeasureColor: getMeasureColor
        };
    }

    appControllers.service('GenericAnalyticService', analyticsServiceObj);


})(appControllers);