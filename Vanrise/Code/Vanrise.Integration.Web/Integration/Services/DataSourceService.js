﻿(function (appControllers) {

    'use strict';

    DataSourceService.$inject = ['UtilsService', 'VRModalService', 'VR_Integration_ExecutionStatusEnum', 'VR_Integration_MappingResultEnum', 'LabelColorsEnum'];

    function DataSourceService(UtilsService, VRModalService, VR_Integration_ExecutionStatusEnum, VR_Integration_MappingResultEnum, LabelColorsEnum) {
        return {
            getExecutionStatusDescription: getExecutionStatusDescription,
            getMappingResultDescription: getMappingResultDescription,
            getExecutionStatusColor: getExecutionStatusColor,
            editDataSource: editDataSource,
            addDataSource: addDataSource,
            deleteDataSource: deleteDataSource
        };

        function getExecutionStatusDescription(executionStatusValue) {
            return UtilsService.getEnumDescription(VR_Integration_ExecutionStatusEnum, executionStatusValue);
        }

        function getMappingResultDescription(mappingResultValue) {
            return UtilsService.getEnumDescription(VR_Integration_MappingResultEnum, mappingResultValue);
        }

        function getExecutionStatusColor(executionStatusValue) {
            var color = undefined;

            switch (executionStatusValue) {
                case VR_Integration_ExecutionStatusEnum.New.value:
                    color = LabelColorsEnum.New.color;
                    break;
                case VR_Integration_ExecutionStatusEnum.Processing.value:
                    color = LabelColorsEnum.Processing.color;
                    break;
                case VR_Integration_ExecutionStatusEnum.Failed.value:
                    color = LabelColorsEnum.Failed.color;
                    break;
                case VR_Integration_ExecutionStatusEnum.Processed.value:
                    color = LabelColorsEnum.Processed.color;
                    break;
            }

            return color;
        }

        function editDataSource(dataSourceObj, onDataSourceUpdated) {
            var modalSettings = {
            };
            var parameters = {
                dataSourceId: dataSourceObj.DataSourceId
            };

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.title = "Edit Data Source";
                modalScope.onDataSourceUpdated = onDataSourceUpdated;
            };
            VRModalService.showModal('Client/Modules/Integration/Views/DataSource/DataSourceEditor.html', parameters, modalSettings);
        }

        function addDataSource(onDataSourceAdded) {
            var modalSettings = {
            };

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.title = "Add a Data Source";
                modalScope.onDataSourceAdded = onDataSourceAdded;
            };

            VRModalService.showModal('Client/Modules/Integration/Views/DataSource/DataSourceEditor.html', null, modalSettings);
        }

        function deleteDataSource(scope, dataSourceObj, onDataSourceDeleted) {
            VRNotificationService.showConfirmation().then(function (response) {

                    if (response) {
                        return VR_Integration_DataSourceAPIService.DeleteDataSource(dataSourceObj.DataSourceId, dataSourceObj.TaskId)
                            .then(function (deletionResponse) {
                                VRNotificationService.notifyOnItemDeleted("DataSource", deletionResponse);
                                onDataSourceDeleted(dataSourceObj);
                            })
                            .catch(function (error) {
                                VRNotificationService.notifyException(error, scope);
                            });
                    }
                });
        }
    };

    appControllers.service('VR_Integration_DataSourceService', DataSourceService);

})(appControllers);
