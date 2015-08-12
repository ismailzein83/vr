app.service('DataSourceAPIService', function (BaseAPIService) {

    return ({
        GetDataSources: GetDataSources,
        GetDataSource: GetDataSource,
        GetDataSourceAdapterTypes: GetDataSourceAdapterTypes,
        GetExecutionFlows: GetExecutionFlows,
        AddExecutionFlow: AddExecutionFlow,
        GetExecutionFlowDefinitions: GetExecutionFlowDefinitions,
        AddDataSource: AddDataSource,
        UpdateDataSource: UpdateDataSource
    });


    function GetDataSources() {
        return BaseAPIService.get("/api/DataSource/GetDataSources");
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

    function UpdateDataSource(dataSource) {
        return BaseAPIService.post("/api/DataSource/UpdateDataSource", dataSource);
    }
});