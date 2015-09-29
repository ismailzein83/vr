'use strict';

app.service('DataSourceService', ['UtilsService', 'VRModalService', 'Integration_ExecutionStatusEnum', 'Integration_MappingResultEnum', 'LabelColorsEnum',
    function (UtilsService, VRModalService, Integration_ExecutionStatusEnum, Integration_MappingResultEnum, LabelColorsEnum) {

        return ({
            getExecutionStatusDescription: getExecutionStatusDescription,
            getMappingResultDescription: getMappingResultDescription,
            getExecutionStatusColor: getExecutionStatusColor,
            openDataSourceEditor: openDataSourceEditor
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

        function openDataSourceEditor(dataSourceId, taskId, onDataSourceUpdated) {
            var modalSettings = {
            };
            var parameters = {
                dataSourceId: dataSourceId,
                taskId: taskId
            };

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.title = "Edit Data Source";
                modalScope.onDataSourceUpdated = onDataSourceUpdated;
            };
            VRModalService.showModal('/Client/Modules/Integration/Views/DataSourceEditor.html', parameters, modalSettings);
        }
}]);
