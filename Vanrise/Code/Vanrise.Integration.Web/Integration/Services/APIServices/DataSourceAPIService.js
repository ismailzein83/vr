(function (appControllers) {

    'use strict';

    DataSourceAPIService.$inject = ['BaseAPIService', 'VR_Integration_ModuleConfig', 'UtilsService'];

    function DataSourceAPIService(BaseAPIService, VR_Integration_ModuleConfig, UtilsService) {
        return ({
            GetDataSources: GetDataSources,
            GetFilteredDataSources: GetFilteredDataSources,
            GetDataSource: GetDataSource,
            GetDataSourceAdapterTypes: GetDataSourceAdapterTypes,
            GetExecutionFlows: GetExecutionFlows,
            AddExecutionFlow: AddExecutionFlow,
            GetExecutionFlowDefinitions: GetExecutionFlowDefinitions,
            AddDataSource: AddDataSource,
            DeleteDataSource: DeleteDataSource,
            UpdateDataSource: UpdateDataSource
        });
        function GetDataSources(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Integration_ModuleConfig.moduleName, 'DataSource', 'GetDataSources'), { filter: filter });
        }

        function GetFilteredDataSources(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Integration_ModuleConfig.moduleName, 'DataSource', 'GetFilteredDataSources'), input);
        }

        function GetDataSource(dataSourceId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Integration_ModuleConfig.moduleName, 'DataSource', 'GetDataSource'),
                {
                    dataSourceId: dataSourceId
                });
        }

        function GetDataSourceAdapterTypes() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Integration_ModuleConfig.moduleName, 'DataSource', 'GetDataSourceAdapterTypes'));
        }

        function GetExecutionFlows() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Integration_ModuleConfig.moduleName, 'DataSource', 'GetExecutionFlows'));
        }

        function AddExecutionFlow(execFlowObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Integration_ModuleConfig.moduleName, 'DataSource', 'AddExecutionFlow'), execFlowObject);
        }

        function GetExecutionFlowDefinitions() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Integration_ModuleConfig.moduleName, 'DataSource', 'GetExecutionFlowDefinitions'));
        }

        function AddDataSource(dataSource) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Integration_ModuleConfig.moduleName, 'DataSource', 'AddDataSource'), dataSource);
        }

        function DeleteDataSource(dataSourceId, taskId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Integration_ModuleConfig.moduleName, 'DataSource', 'DeleteDataSource'), {
                dataSourceId: dataSourceId,
                taskId: taskId
            });
        }

        function UpdateDataSource(dataSource) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Integration_ModuleConfig.moduleName, 'DataSource', 'UpdateDataSource'), dataSource);
        }

    }

    appControllers.service('VR_Integration_DataSourceAPIService', DataSourceAPIService);

})(appControllers);
