app.service('DashboardAPIService', function (BaseAPIService) {

    return ({
        GetCasesSummary: GetCasesSummary,
        GetStrategyCases: GetStrategyCases,
        GetBTSCases: GetBTSCases,
        GetCellCases: GetCellCases
    });


    function GetCasesSummary(fromDate, toDate) {
        return BaseAPIService.get("/api/Dashboard/GetCasesSummary",
            {
                fromDate: fromDate,
                toDate: toDate
            }
           );
    }

    function GetStrategyCases(fromDate, toDate) {
        return BaseAPIService.get("/api/Dashboard/GetStrategyCases",
            {
                fromDate: fromDate,
                toDate: toDate
            }
           );
    }

    function GetBTSCases(fromDate, toDate) {
        return BaseAPIService.get("/api/Dashboard/GetBTSCases",
            {
                fromDate: fromDate,
                toDate: toDate
            }
           );
    }

    function GetCellCases(fromDate, toDate) {
        return BaseAPIService.get("/api/Dashboard/GetCellCases",
            {
                fromDate: fromDate,
                toDate: toDate
            }
           );
    }




   



});