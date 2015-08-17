app.service('DailyReportAPIService', function (BaseAPIService) {

    return ({
        GetDailyReportCalls: GetDailyReportCalls
    });

    function GetDailyReportCalls(input) {
        return BaseAPIService.get('/api/DailyReport/GetDailyReportCalls', input);
    }
});