app.service('DataSourceAPIService', function (BaseAPIService) {

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

    function GetDataSources() {
        return BaseAPIService.get("/api/DataSource/GetDataSources");
    }

    function GetFilteredDataSources(input) {
        return BaseAPIService.post("/api/DataSource/GetFilteredDataSources", input);
    }

    function GetDataSource(dataSourceId) {
        return BaseAPIService.get("/api/DataSource/GetDataSource",
            {
                dataSourceId: dataSourceId
            });
    }

    function GetDataSourceAdapterTypes() {
        return BaseAPIService.get("/api/DataSource/GetDataSourceAdapterTypes");
    }

    function GetExecutionFlows() {
        return BaseAPIService.get("/api/DataSource/GetExecutionFlows");
    }

    function AddExecutionFlow(execFlowObject) {
        return BaseAPIService.post("/api/DataSource/AddExecutionFlow", execFlowObject);
    }

    function GetExecutionFlowDefinitions() {
        return BaseAPIService.get("/api/DataSource/GetExecutionFlowDefinitions");
    }

    function AddDataSource(dataSource) {
        return BaseAPIService.post("/api/DataSource/AddDataSource", dataSource);
    }

    function DeleteDataSource(dataSourceId, taskId) {
        return BaseAPIService.get("/api/DataSource/DeleteDataSource", {
            dataSourceId: dataSourceId,
            taskId: taskId
        });
    }

    function UpdateDataSource(dataSource) {
        return BaseAPIService.post("/api/DataSource/UpdateDataSource", dataSource);
    }
});