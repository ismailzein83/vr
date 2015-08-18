
app.service('HourlyReportAPIService', function (BaseAPIService) {

    return ({
        GetHourlyReportData: GetHourlyReportData,
        GetHourlyReport: GetHourlyReport
    });


    function GetHourlyReportData(hourlyReportDataInput) {
        return BaseAPIService.post("/api/HourlyReport/GetHourlyReportData", hourlyReportDataInput);
    }
    function GetHourlyReport(filterByColumn, columnFilterValue, from, to) {
        return BaseAPIService.get("/api/HourlyReport/GetHourlyReport",
           {
               filterByColumn: filterByColumn,
               columnFilterValue: columnFilterValue,
               from: from,
               to: to
           }
          );
    }

});