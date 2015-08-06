app.service('DataSourceImportedBatchAPIService', function (BaseAPIService) {

    return ({
        GetFilteredDataSourceImportedBatches: GetFilteredDataSourceImportedBatches,
        GetBatchNames: GetBatchNames
    });

    function GetFilteredDataSourceImportedBatches(input) {
        return BaseAPIService.post('/api/DataSourceImportedBatch/GetFilteredDataSourceImportedBatches', input);
    }

    function GetBatchNames() {
        return BaseAPIService.get('/api/DataSourceImportedBatch/GetBatchNames');
    }
});