﻿app.service('DataSourceLogsAPIService', function (BaseAPIService) {

    return ({
        GetFilteredDataSourceLogs: GetFilteredDataSourceLogs
    });

    function GetFilteredDataSourceLogs(input) {
        return BaseAPIService.post('/api/DataSourceLog/GetFilteredDataSourceLogs', input);
    }
});