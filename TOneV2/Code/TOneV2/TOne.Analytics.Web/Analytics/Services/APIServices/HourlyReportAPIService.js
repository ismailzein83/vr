
app.service('HourlyReportAPIService', function (BaseAPIService) {

    return ({
        GetHourlyReportData: GetHourlyReportData,

    });


    function GetHourlyReportData(hourlyReportDataInput) {
        return BaseAPIService.post("/api/HourlyReport/GetHourlyReportData", hourlyReportDataInput);
    }


});