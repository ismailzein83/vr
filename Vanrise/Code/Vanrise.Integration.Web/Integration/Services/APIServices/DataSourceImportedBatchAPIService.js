app.service('DataSourceImportedBatchAPIService', function (BaseAPIService) {

    return ({
        GetFilteredDataSourceImportedBatches: GetFilteredDataSourceImportedBatches,
        GetQueueItemHeaders: GetQueueItemHeaders
    });

    function GetFilteredDataSourceImportedBatches(input) {
        return BaseAPIService.post('/api/DataSourceImportedBatch/GetFilteredDataSourceImportedBatches', input);
    }

    function GetQueueItemHeaders(input) {
        return BaseAPIService.post('/api/DataSourceImportedBatch/GetQueueItemHeaders', input);
    }
});