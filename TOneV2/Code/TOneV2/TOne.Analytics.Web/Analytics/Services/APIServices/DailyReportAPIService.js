app.service('DailyReportAPIService', function (BaseAPIService) {

    return ({
        GetFilteredDailyReportCalls: GetFilteredDailyReportCalls
    });

    function GetFilteredDailyReportCalls(input) {
        return BaseAPIService.post('/api/DailyReport/GetFilteredDailyReportCalls', input);
    }
});