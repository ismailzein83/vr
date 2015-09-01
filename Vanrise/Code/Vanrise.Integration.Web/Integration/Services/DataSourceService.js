'use strict';

app.service('DataSourceService', ['UtilsService', 'Integration_ExecutionStatusEnum', 'Integration_MappingResultEnum', 'LabelColorsEnum',
    function (UtilsService, Integration_ExecutionStatusEnum, Integration_MappingResultEnum, LabelColorsEnum) {

    return ({
        getExecutionStatusDescription: getExecutionStatusDescription,
        getMappingResultDescription: getMappingResultDescription,
        getExecutionStatusColor: getExecutionStatusColor
    });

    function getExecutionStatusDescription(executionStatusValue) {
        return UtilsService.getEnumDescription(Integration_ExecutionStatusEnum, executionStatusValue);
    }

    function getMappingResultDescription(mappingResultValue) {
        return UtilsService.getEnumDescription(Integration_MappingResultEnum, mappingResultValue);
    }

    function getExecutionStatusColor(executionStatusValue) {
        var color = undefined;

        switch (executionStatusValue) {
            case Integration_ExecutionStatusEnum.New.value:
                color = LabelColorsEnum.New.color;
                break;
            case Integration_ExecutionStatusEnum.Processing.value:
                color = LabelColorsEnum.Processing.color;
                break;
            case Integration_ExecutionStatusEnum.Failed.value:
                color = LabelColorsEnum.Failed.color;
                break;
            case Integration_ExecutionStatusEnum.Processed.value:
                color = LabelColorsEnum.Processed.color;
                break;
        }

        return color;
    }

}]);